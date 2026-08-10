using System;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using frm_Intel_RST_Auto_Installer.Models;

namespace frm_Intel_RST_Auto_Installer.Services
{
    public static class ChipsetDetector
    {
        public static ChipsetInfo Detect(Action<string> log)
        {
            ChipsetInfo info = DetectViaWmi(log);

            if (NeedsFallback(info))
            {
                if (log != null) log("WMI detection insufficient, falling back to SetupAPI...");
                var fallback = DetectViaSetupApi(log);
                if (!string.IsNullOrEmpty(fallback.RawName))
                {
                    info = fallback;
                }
            }

            int? generation;
            string label;
            bool matched = TryParseGeneration(info.RawName, out generation, out label);
            info.Generation = generation;
            info.GenerationLabel = matched ? label : "Unknown";

            info.IsSupported = matched && string.Equals(info.Manufacturer, "GenuineIntel", StringComparison.OrdinalIgnoreCase);

            if (!string.Equals(info.Manufacturer, "GenuineIntel", StringComparison.OrdinalIgnoreCase))
            {
                info.IsSupported = false;
            }

            return info;
        }

        private static ChipsetInfo DetectViaWmi(Action<string> log)
        {
            var info = new ChipsetInfo { DetectionMethod = "Detected via WMI" };
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Name, Manufacturer FROM Win32_Processor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        info.RawName = SafeToString(obj["Name"]).Trim();
                        info.Manufacturer = SafeToString(obj["Manufacturer"]).Trim();
                        obj.Dispose();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                if (log != null) log("WMI query failed: " + ex.Message);
            }
            return info;
        }

        private static bool NeedsFallback(ChipsetInfo info)
        {
            if (string.IsNullOrEmpty(info.RawName)) return true;
            if (info.RawName.IndexOf("To Be Filled By O.E.M.", StringComparison.OrdinalIgnoreCase) >= 0) return true;

            int? gen;
            string label;
            return !TryParseGeneration(info.RawName, out gen, out label);
        }

        private static bool TryParseGeneration(string rawName, out int? generation, out string label)
        {
            generation = null;
            label = null;
            if (string.IsNullOrEmpty(rawName)) return false;

            var genMatch = Regex.Match(rawName, @"(\d{1,2})th Gen", RegexOptions.IgnoreCase);
            if (genMatch.Success)
            {
                int gen = int.Parse(genMatch.Groups[1].Value);
                generation = gen;
                label = gen + "th Gen";
                return true;
            }

            bool isUltra = Regex.IsMatch(rawName, @"Core\s*\(TM\)?\s*Ultra", RegexOptions.IgnoreCase)
                           || Regex.IsMatch(rawName, @"Core\s+Ultra", RegexOptions.IgnoreCase);
            if (isUltra)
            {
                // Model prefix (200 vs 300) roughly maps to Series 2 vs Series 3.
                var modelMatch = Regex.Match(rawName, @"Ultra\s+[A-Z]?\d\s+(\d{3})", RegexOptions.IgnoreCase);
                string series = "Series 1";
                if (modelMatch.Success)
                {
                    string modelNum = modelMatch.Groups[1].Value;
                    if (modelNum.StartsWith("3")) series = "Series 3";
                    else if (modelNum.StartsWith("2")) series = "Series 2";
                }
                generation = null;
                label = "Core Ultra " + series;
                return true;
            }

            // Real-world Win32_Processor.Name strings (e.g. "Intel(R) Core(TM) i7-10700 CPU @ 2.90GHz",
            // "Intel(R) Core(TM) i5-1135G7 CPU @ 2.40GHz") don't contain the literal "Nth Gen" text —
            // the generation has to be derived from the model number after i3/i5/i7/i9.
            var modelNumMatch = Regex.Match(rawName, @"i[3579]-(\d{3,6})", RegexOptions.IgnoreCase);
            if (modelNumMatch.Success)
            {
                int? gen = ParseGenerationFromModelNumber(modelNumMatch.Groups[1].Value);
                if (gen.HasValue)
                {
                    generation = gen;
                    label = gen + "th Gen";
                    return true;
                }
            }

            return false;
        }

        private static int? ParseGenerationFromModelNumber(string digits)
        {
            if (digits.Length < 4) return null;

            // Two-leading-digit generations (10th+) look like "10700", "1065G7", "1260P", "14700K", "15700".
            // No upper bound here: DriverMapping.GetBucketKey() re-validates the generation against the
            // known bucket list, so an unrecognized future generation simply falls through as unsupported
            // instead of being silently misparsed as a single-digit (2nd-9th gen) model.
            int first2;
            if (int.TryParse(digits.Substring(0, 2), out first2) && first2 >= 10 && first2 <= 19)
            {
                return first2;
            }

            // Single-leading-digit generations (2nd-9th) look like "2500K", "9700K".
            int first1;
            if (int.TryParse(digits.Substring(0, 1), out first1) && first1 >= 2 && first1 <= 9)
            {
                return first1;
            }

            return null;
        }

        private static string SafeToString(object value)
        {
            return value == null ? string.Empty : value.ToString();
        }

        #region SetupAPI fallback

        private static readonly Guid ProcessorClassGuid = new Guid("50127DC3-0F36-415E-A6CC-4CB3BE910B65");
        private const int DIGCF_PRESENT = 0x00000002;
        private const int SPDRP_DEVICEDESC = 0x00000000;
        private const int SPDRP_MFG = 0x0000000B;

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVINFO_DATA
        {
            public int cbSize;
            public Guid ClassGuid;
            public int DevInst;
            public IntPtr Reserved;
        }

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, IntPtr enumerator, IntPtr hwndParent, int flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet, int memberIndex, ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData,
            int property, out int propertyRegDataType, byte[] propertyBuffer, int propertyBufferSize, out int requiredSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        private static ChipsetInfo DetectViaSetupApi(Action<string> log)
        {
            var info = new ChipsetInfo { DetectionMethod = "Detected via SetupAPI (fallback)" };
            IntPtr deviceInfoSet = IntPtr.Zero;
            try
            {
                Guid guid = ProcessorClassGuid;
                deviceInfoSet = SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT);
                if (deviceInfoSet == IntPtr.Zero || deviceInfoSet.ToInt64() == -1)
                {
                    if (log != null) log("SetupDiGetClassDevs failed.");
                    return info;
                }

                var devData = new SP_DEVINFO_DATA();
                devData.cbSize = Marshal.SizeOf(typeof(SP_DEVINFO_DATA));

                if (SetupDiEnumDeviceInfo(deviceInfoSet, 0, ref devData))
                {
                    string desc = GetDeviceProperty(deviceInfoSet, ref devData, SPDRP_DEVICEDESC);
                    string mfg = GetDeviceProperty(deviceInfoSet, ref devData, SPDRP_MFG);

                    info.RawName = desc;
                    if (!string.IsNullOrEmpty(mfg) && mfg.IndexOf("Intel", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        info.Manufacturer = "GenuineIntel";
                    }
                    else if (!string.IsNullOrEmpty(desc) && desc.IndexOf("Intel", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        info.Manufacturer = "GenuineIntel";
                    }
                    else
                    {
                        info.Manufacturer = string.IsNullOrEmpty(mfg) ? "Unknown" : mfg;
                    }
                }
                else
                {
                    if (log != null) log("SetupDiEnumDeviceInfo found no processor devices.");
                }
            }
            catch (Exception ex)
            {
                if (log != null) log("SetupAPI fallback failed: " + ex.Message);
            }
            finally
            {
                if (deviceInfoSet != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }
            return info;
        }

        private static string GetDeviceProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devData, int property)
        {
            int requiredSize;
            int dataType;
            byte[] buffer = new byte[1024];
            if (SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref devData, property, out dataType, buffer, buffer.Length, out requiredSize))
            {
                return Encoding.Unicode.GetString(buffer, 0, requiredSize).TrimEnd('\0');
            }
            return string.Empty;
        }

        #endregion
    }
}
