using System;
using Microsoft.Win32;

namespace frm_Intel_RST_Auto_Installer.Services
{
    public static class RstVersionChecker
    {
        public static string GetInstalledVersion()
        {
            string version = ReadVersion(@"SOFTWARE\Intel\IRST");
            if (version == null)
            {
                // 32-bit registry view redirection on 64-bit Windows when running as x86.
                version = ReadVersion(@"SOFTWARE\Wow6432Node\Intel\IRST");
            }
            return version;
        }

        private static string ReadVersion(string subKeyPath)
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(subKeyPath))
                {
                    if (key == null) return null;
                    object value = key.GetValue("Version");
                    return value == null ? null : value.ToString();
                }
            }
            catch
            {
                return null;
            }
        }

        public static bool IsUpToDate(string installed, string recommended)
        {
            int[] vInstalled, vRecommended;
            if (!TryParseVersionParts(installed, out vInstalled)) return false;
            if (!TryParseVersionParts(recommended, out vRecommended)) return false;
            return CompareVersionParts(vInstalled, vRecommended) >= 0;
        }

        // Real RST versions have 5 parts (e.g. "20.2.6.1025.3"), which System.Version can't
        // represent (max 4 parts) — comparing as strings/ints per-part avoids silently
        // dropping the 5th component and treating differing builds as identical.
        private static bool TryParseVersionParts(string s, out int[] parts)
        {
            parts = null;
            if (string.IsNullOrEmpty(s)) return false;

            string[] rawParts = s.Split('.');
            var result = new int[rawParts.Length];
            for (int i = 0; i < rawParts.Length; i++)
            {
                int n;
                if (!int.TryParse(rawParts[i], out n)) return false;
                result[i] = n;
            }
            parts = result;
            return true;
        }

        private static int CompareVersionParts(int[] a, int[] b)
        {
            int len = Math.Max(a.Length, b.Length);
            for (int i = 0; i < len; i++)
            {
                int av = i < a.Length ? a[i] : 0;
                int bv = i < b.Length ? b[i] : 0;
                if (av != bv) return av.CompareTo(bv);
            }
            return 0;
        }
    }
}
