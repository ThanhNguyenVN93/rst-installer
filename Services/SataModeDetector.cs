using System;
using Microsoft.Win32;

namespace frm_Intel_RST_Auto_Installer.Services
{
    public static class SataModeDetector
    {
        // Heuristic: inspect the Start value of the known SATA controller driver services.
        // A service with Start == 0 (Boot) is the one actually driving the boot volume.
        private static readonly string[] AhciServiceNames = { "storahci" };
        private static readonly string[] RaidServiceNames = { "iaStorAVC", "iaStorAC", "iaStorVD" };
        // Not treated as a match yet — VMD uses its own driver stack and the real service name
        // varies by Windows/driver version. Logged so real-world names can be confirmed and
        // folded into RaidServiceNames/AhciServiceNames once verified on a VMD-enabled machine.
        private static readonly string[] VmdCandidateServiceNames = { "iaStorAVC", "VMD", "vmd", "iaVmd" };

        public static string Detect(Action<string> log)
        {
            try
            {
                LogServiceState(AhciServiceNames, log);
                LogServiceState(RaidServiceNames, log);
                LogServiceState(VmdCandidateServiceNames, log);

                if (AnyServiceBootStart(AhciServiceNames, log)) return "AHCI";
                if (AnyServiceBootStart(RaidServiceNames, log)) return "RAID";
                return "Unknown";
            }
            catch (Exception ex)
            {
                if (log != null) log("SATA mode detection failed: " + ex.Message);
                return "Unknown";
            }
        }

        private static bool AnyServiceBootStart(string[] serviceNames, Action<string> log)
        {
            foreach (var name in serviceNames)
            {
                if (IsServiceBootStart(name, log)) return true;
            }
            return false;
        }

        private static void LogServiceState(string[] serviceNames, Action<string> log)
        {
            if (log == null) return;
            foreach (var name in serviceNames)
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + name))
                {
                    if (key == null)
                    {
                        log("SATA mode detection: service '" + name + "' not present.");
                        continue;
                    }
                    object start = key.GetValue("Start");
                    log("SATA mode detection: service '" + name + "' Start=" + (start == null ? "(none)" : start.ToString()));
                }
            }
        }

        private static bool IsServiceBootStart(string serviceName, Action<string> log)
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName))
            {
                if (key == null) return false;
                object start = key.GetValue("Start");
                bool isBoot = start != null && Convert.ToInt32(start) == 0;
                if (isBoot && log != null) log("SATA mode detection: matched boot-start service '" + serviceName + "'.");
                return isBoot;
            }
        }
    }
}
