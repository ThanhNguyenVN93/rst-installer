using System;
using System.IO;

namespace frm_Intel_RST_Auto_Installer.Services
{
    public static class FileLogger
    {
        private static readonly object Lock = new object();
        private static string _logFilePath;

        public static string LogFilePath
        {
            get
            {
                lock (Lock)
                {
                    if (_logFilePath == null)
                    {
                        string dir = Path.Combine(Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "IntelRSTAutoInstaller"), "logs");
                        Directory.CreateDirectory(dir);
                        _logFilePath = Path.Combine(dir, "log_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt");
                    }
                    return _logFilePath;
                }
            }
        }

        public static void Append(string line)
        {
            try
            {
                lock (Lock)
                {
                    File.AppendAllText(LogFilePath, line + Environment.NewLine);
                }
            }
            catch
            {
                // Best-effort: a failing log write must never crash the caller.
            }
        }
    }
}
