using System;
using System.Diagnostics;

namespace frm_Intel_RST_Auto_Installer.Services
{
    public static class InstallService
    {
        private static readonly int[] SuccessExitCodes = { 0, 3010, 1641 };

        public static ProcessStartInfo BuildStartInfo(string filePath, bool silent)
        {
            return new ProcessStartInfo(filePath)
            {
                Arguments = silent ? "-silent -accepteula" : "",
                UseShellExecute = true
            };
        }

        public static bool IsSuccessExitCode(int exitCode)
        {
            return Array.IndexOf(SuccessExitCodes, exitCode) >= 0;
        }

        public static bool RequiresReboot(int exitCode)
        {
            return exitCode == 3010 || exitCode == 1641;
        }
    }
}
