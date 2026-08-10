using System;

namespace frm_Intel_RST_Auto_Installer.Models
{
    public struct ChipsetInfo
    {
        public string RawName;
        public string Manufacturer;
        public int? Generation;
        public string GenerationLabel;
        public string DetectionMethod;
        public bool IsSupported;
    }

    public enum VmdStatus
    {
        Enabled,
        Disabled,
        Unknown
    }

    public class DownloadProgressInfo
    {
        public int PercentComplete;
        public long BytesReceived;
        public long TotalBytesToReceive;
        public double BytesPerSecond;
    }

    public class DriverEntry
    {
        public string Version;
        public string Url;
        public string Checksum;
        // Mirror used when Url fails (e.g. Intel's mirror ID goes dead). Packaged as a .zip
        // containing exactly one .exe, since that's how the mirror host serves it.
        public string FallbackUrl;
    }
}
