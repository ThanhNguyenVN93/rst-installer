using System;
using System.IO;
using System.Security.Cryptography;

namespace frm_Intel_RST_Auto_Installer.Services
{
    public static class DownloadVerifier
    {
        public static bool Verify(string filePath, string expectedSha256, out string error)
        {
            error = null;

            if (!File.Exists(filePath))
            {
                error = "Downloaded file not found.";
                return false;
            }

            var size = new FileInfo(filePath).Length;
            if (size <= 0)
            {
                error = "Downloaded file is empty.";
                return false;
            }

            if (string.IsNullOrEmpty(expectedSha256))
            {
                return true;
            }

            string actualSha256 = ComputeSha256(filePath);
            if (!string.Equals(actualSha256, expectedSha256, StringComparison.OrdinalIgnoreCase))
            {
                error = "Checksum mismatch: expected " + expectedSha256 + " but got " + actualSha256 + ".";
                return false;
            }

            return true;
        }

        private static string ComputeSha256(string filePath)
        {
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
    }
}
