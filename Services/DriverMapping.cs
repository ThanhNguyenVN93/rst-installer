using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web.Script.Serialization;
using frm_Intel_RST_Auto_Installer.Models;

namespace frm_Intel_RST_Auto_Installer.Services
{
    public static class DriverMapping
    {
        private const string RemoteUrl = "https://raw.githubusercontent.com/ThanhNguyenVN93/rst-installer/main/driver-mapping.json";
        private const string EmbeddedResourceName = "driver-mapping.json";
        private const int RemoteTimeoutMs = 8000;
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

        private static readonly object CacheLock = new object();
        private static string _cachedJson;
        private static DateTime _cacheTimestampUtc = DateTime.MinValue;

        public static Dictionary<string, DriverEntry> Load(Action<string> log)
        {
            string json = TryDownloadRemote(log);

            if (string.IsNullOrEmpty(json))
            {
                json = LoadEmbedded();
                if (log != null) log("Using embedded driver mapping (offline or remote unavailable).");
            }
            else
            {
                if (log != null) log("Driver mapping updated from remote source.");
            }

            return Parse(json);
        }

        private static string TryDownloadRemote(Action<string> log)
        {
            lock (CacheLock)
            {
                if (_cachedJson != null && DateTime.UtcNow - _cacheTimestampUtc < CacheTtl)
                {
                    if (log != null) log("Using cached remote driver mapping (fetched " + (int)(DateTime.UtcNow - _cacheTimestampUtc).TotalSeconds + "s ago).");
                    return _cachedJson;
                }
            }

            try
            {
                using (var wc = new TimeoutWebClient(RemoteTimeoutMs))
                {
                    wc.Headers.Add("User-Agent", "IntelRSTAutoInstaller");
                    wc.Proxy = null;
                    string json = wc.DownloadString(RemoteUrl);
                    lock (CacheLock)
                    {
                        _cachedJson = json;
                        _cacheTimestampUtc = DateTime.UtcNow;
                    }
                    return json;
                }
            }
            catch (Exception ex)
            {
                if (log != null) log("Remote driver mapping fetch failed: " + ex.Message);
                return null;
            }
        }

        // WebClient has no direct Timeout property; the underlying HttpWebRequest defaults to
        // ~100s, which made a dead/black-holed network stall Rescan for nearly two minutes
        // before falling back to the embedded mapping.
        private sealed class TimeoutWebClient : WebClient
        {
            private readonly int _timeoutMs;

            public TimeoutWebClient(int timeoutMs)
            {
                _timeoutMs = timeoutMs;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                if (request != null) request.Timeout = _timeoutMs;
                return request;
            }
        }

        private static string LoadEmbedded()
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream(EmbeddedResourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("Embedded driver mapping resource not found.");
                }
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static Dictionary<string, DriverEntry> Parse(string json)
        {
            var serializer = new JavaScriptSerializer();
            var raw = serializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
            var result = new Dictionary<string, DriverEntry>();
            foreach (var kvp in raw)
            {
                string version = kvp.Value.ContainsKey("version") ? kvp.Value["version"] : null;
                string url = kvp.Value.ContainsKey("url") ? kvp.Value["url"] : null;
                string checksum = kvp.Value.ContainsKey("checksum") ? kvp.Value["checksum"] : null;
                string fallbackUrl = kvp.Value.ContainsKey("fallbackUrl") ? kvp.Value["fallbackUrl"] : null;
                result[kvp.Key] = new DriverEntry { Version = version, Url = url, Checksum = checksum, FallbackUrl = fallbackUrl };
            }
            return result;
        }

        public static string GetBucketKey(ChipsetInfo info)
        {
            if (!string.IsNullOrEmpty(info.GenerationLabel) &&
                info.GenerationLabel.IndexOf("Ultra", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (info.GenerationLabel.IndexOf("Series 3", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "ultra-series-3";
                }
                return null;
            }

            if (info.Generation.HasValue)
            {
                int g = info.Generation.Value;
                if (g == 8 || g == 9) return "8-9";
                if (g == 10 || g == 11) return "10-11";
                if (g == 12 || g == 13) return "12-13";
                if (g == 14 || g == 15) return "14-15";
            }

            return null;
        }
    }
}
