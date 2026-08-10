using System;
using System.Management;
using frm_Intel_RST_Auto_Installer.Models;

namespace frm_Intel_RST_Auto_Installer.Services
{
    public static class VmdDetector
    {
        public static VmdStatus Check(Action<string> log)
        {
            try
            {
                // Real-world display names come from the Intel driver's INF and are like
                // "Intel RST VMD Controller A77F" / "Intel RST VMD Managed Controller 09AB" —
                // not the generic "Volume Management Device" name used in Intel's docs.
                // Multiple entries can show up at once (one Controller + N Managed Controllers),
                // so treat VMD as enabled if ANY of them is active (ConfigManagerErrorCode == 0)
                // rather than trusting whichever entry WMI happens to return first.
                bool foundAny = false;
                bool anyEnabled = false;
                using (var searcher = new ManagementObjectSearcher(
                    "SELECT Name, ConfigManagerErrorCode FROM Win32_PnPEntity WHERE Name LIKE '%VMD%'"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        foundAny = true;
                        object errorCodeObj = obj["ConfigManagerErrorCode"];
                        string name = obj["Name"] as string;
                        obj.Dispose();

                        bool enabled = errorCodeObj != null && Convert.ToInt32(errorCodeObj) == 0;
                        if (log != null) log("VMD detection: found '" + name + "' ConfigManagerErrorCode=" + (errorCodeObj == null ? "(none)" : errorCodeObj.ToString()));
                        if (enabled) anyEnabled = true;
                    }
                }

                if (!foundAny)
                {
                    LogUnmatchedIntelDevices(log);
                    return VmdStatus.Disabled;
                }

                return anyEnabled ? VmdStatus.Enabled : VmdStatus.Disabled;
            }
            catch (Exception ex)
            {
                if (log != null) log("VMD detection failed: " + ex.Message);
                return VmdStatus.Unknown;
            }
        }

        private static void LogUnmatchedIntelDevices(Action<string> log)
        {
            if (log == null) return;
            try
            {
                using (var searcher = new ManagementObjectSearcher(
                    "SELECT Name FROM Win32_PnPEntity WHERE Name LIKE '%Intel%'"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string name = obj["Name"] as string;
                        obj.Dispose();
                        log("VMD detection: no 'VMD' match; Intel PnP entity present: '" + name + "'");
                    }
                }
            }
            catch (Exception ex)
            {
                log("VMD detection: failed to enumerate Intel PnP entities: " + ex.Message);
            }
        }
    }
}
