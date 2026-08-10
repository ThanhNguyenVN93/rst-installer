using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows.Forms;
using frm_Intel_RST_Auto_Installer.Models;
using frm_Intel_RST_Auto_Installer.Services;

namespace frm_Intel_RST_Auto_Installer
{
    public partial class Form1 : Form
    {
        private class DetectionResult
        {
            public ChipsetInfo Chipset;
            public VmdStatus Vmd = VmdStatus.Unknown;
            public string SataMode = "Unknown";
            public Dictionary<string, DriverEntry> Mapping;
            public DriverEntry Recommended;
            public string BucketKey;
            public string InstalledVersion;

            // Single source of truth for "is this machine actually supported end-to-end":
            // Chipset.IsSupported coming in (generation/manufacturer recognized) AND a bucket
            // exists AND that bucket has a verified download URL. Sets Chipset.IsSupported,
            // BucketKey and Recommended together so nothing else has to mutate them piecemeal.
            public bool ResolveRecommended(Action<string> log)
            {
                BucketKey = null;
                Recommended = null;

                bool resolved = false;
                if (Chipset.IsSupported && Mapping != null)
                {
                    BucketKey = DriverMapping.GetBucketKey(Chipset);
                    DriverEntry entry;
                    if (BucketKey != null && Mapping.TryGetValue(BucketKey, out entry) && !string.IsNullOrEmpty(entry.Url))
                    {
                        Recommended = entry;
                        resolved = true;
                    }
                    else if (BucketKey != null && log != null)
                    {
                        log("Driver link for bucket '" + BucketKey + "' is not verified yet.");
                    }
                }

                Chipset.IsSupported = resolved;
                return resolved;
            }
        }

        private BackgroundWorker _detectWorker;
        private BackgroundWorker _installWorker;
        private DownloadService _downloadService;

        private ChipsetInfo _chipsetInfo;
        private DriverEntry _recommended;
        private string _downloadedFilePath;
        private string _extractDirToCleanUp;
        private bool _usingFallbackMirror;

        public Form1()
        {
            InitializeComponent();

            btnDonate.Image = LoadDonateButtonImage();

            _detectWorker = new BackgroundWorker();
            _detectWorker.DoWork += DetectWorker_DoWork;
            _detectWorker.RunWorkerCompleted += DetectWorker_RunWorkerCompleted;

            _installWorker = new BackgroundWorker();
            _installWorker.DoWork += InstallWorker_DoWork;
            _installWorker.RunWorkerCompleted += InstallWorker_RunWorkerCompleted;

            _downloadService = new DownloadService();
            _downloadService.ProgressChanged += DownloadService_ProgressChanged;
            _downloadService.Completed += DownloadService_Completed;
            _downloadService.Failed += DownloadService_Failed;
            _downloadService.Cancelled += DownloadService_Cancelled;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            lblAppVersion.Text = "v" + version.ToString(3);
            UpdateConnectionStatus();

            RunDetection();
        }

        private void UpdateConnectionStatus()
        {
            bool online = NetworkInterface.GetIsNetworkAvailable();
            lblConnectionStatus.Text = online ? "Ready" : "No internet connection";
        }

        #region Logging

        private void Log(string message)
        {
            string line = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message;
            if (txtLog.InvokeRequired)
            {
                txtLog.BeginInvoke(new Action(() => AppendLog(line)));
            }
            else
            {
                AppendLog(line);
            }
        }

        private void AppendLog(string line)
        {
            txtLog.AppendText(line + Environment.NewLine);
            FileLogger.Append(line);
        }

        #endregion

        #region Detection

        private void RunDetection()
        {
            SetStatus("Detecting...", Color.SteelBlue);
            btnDownloadInstall.Enabled = false;
            btnRescan.Enabled = false;
            pnlVmdWarning.Visible = false;
            progressBarDownload.Style = ProgressBarStyle.Continuous;
            progressBarDownload.Value = 0;
            lblDownloadPercent.Text = "0%";
            lblDownloadSize.Text = "";
            lblDownloadSpeed.Text = "";

            Log("Rescan requested. Detecting CPU...");
            _detectWorker.RunWorkerAsync();
        }

        private void DetectWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var result = new DetectionResult();

            result.Chipset = ChipsetDetector.Detect(Log);
            Log("CPU: " + result.Chipset.RawName + " (" + result.Chipset.DetectionMethod + ")");
            Log("Manufacturer: " + result.Chipset.Manufacturer + " | Generation: " + result.Chipset.GenerationLabel);

            if (!string.Equals(result.Chipset.Manufacturer, "GenuineIntel", StringComparison.OrdinalIgnoreCase))
            {
                Log("Non-Intel CPU detected; skipping VMD/SATA/driver checks.");
                result.InstalledVersion = RstVersionChecker.GetInstalledVersion();
                e.Result = result;
                return;
            }

            result.Vmd = VmdDetector.Check(Log);
            Log("VMD status: " + result.Vmd);

            result.SataMode = SataModeDetector.Detect(Log);
            Log("SATA mode: " + result.SataMode);

            result.Mapping = DriverMapping.Load(Log);
            result.ResolveRecommended(Log);

            result.InstalledVersion = RstVersionChecker.GetInstalledVersion();
            Log("Installed RST version: " + (result.InstalledVersion ?? "Not installed"));

            e.Result = result;
        }

        private void DetectWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Log("Detection failed: " + e.Error.Message);
                SetStatus("Error", Color.Red);
                btnRescan.Enabled = true;
                return;
            }

            var result = (DetectionResult)e.Result;
            _chipsetInfo = result.Chipset;
            _recommended = result.Recommended;

            lblCpuNameValue.Text = string.IsNullOrEmpty(result.Chipset.RawName) ? "Unknown" : result.Chipset.RawName;
            lblGenerationValue.Text = result.Chipset.GenerationLabel;
            lblManufacturerValue.Text = string.IsNullOrEmpty(result.Chipset.Manufacturer) ? "Unknown" : result.Chipset.Manufacturer;
            lblDetectionMethod.Text = result.Chipset.DetectionMethod;

            lblSataModeValue.Text = result.SataMode;
            lblVmdStatusValue.Text = result.Vmd.ToString();
            pnlVmdWarning.Visible = result.Vmd == VmdStatus.Enabled;

            lblInstalledVersionValue.Text = result.InstalledVersion ?? "Not installed";
            lblRecommendedVersionValue.Text = result.Recommended != null ? result.Recommended.Version : "-";
            txtDownloadSource.Text = result.Recommended != null ? result.Recommended.Url : "";

            btnRescan.Enabled = true;

            if (!string.Equals(result.Chipset.Manufacturer, "GenuineIntel", StringComparison.OrdinalIgnoreCase))
            {
                SetStatus("Not supported (non-Intel CPU)", Color.Red);
                btnDownloadInstall.Enabled = false;
                return;
            }

            if (!result.Chipset.IsSupported || result.Recommended == null)
            {
                SetStatus("Unrecognized CPU generation", Color.Red);
                btnDownloadInstall.Enabled = false;
                return;
            }

            if (RstVersionChecker.IsUpToDate(result.InstalledVersion, result.Recommended.Version))
            {
                SetStatus("Already up to date", Color.Green);
                btnDownloadInstall.Enabled = false;
                return;
            }

            SetStatus("Idle", Color.Gray);
            btnDownloadInstall.Enabled = true;
        }

        #endregion

        #region Download & Install

        private void btnDownloadInstall_Click(object sender, EventArgs e)
        {
            if (_recommended == null || string.IsNullOrEmpty(_recommended.Url)) return;

            _usingFallbackMirror = false;
            SetBusyState(true);
            SetStatus("Downloading...", Color.SteelBlue);
            progressBarDownload.Style = ProgressBarStyle.Continuous;
            progressBarDownload.Value = 0;
            lblDownloadPercent.Text = "0%";
            lblDownloadSize.Text = "";
            lblDownloadSpeed.Text = "";

            StartDownload(_recommended.Url, false);
        }

        private void StartDownload(string url, bool isFallback)
        {
            _usingFallbackMirror = isFallback;
            string extension = isFallback ? ".zip" : ".exe";
            string fileName = "SetupRST_" + Guid.NewGuid().ToString("N").Substring(0, 8) + extension;
            _downloadedFilePath = Path.Combine(Path.GetTempPath(), fileName);

            Log((isFallback ? "Starting fallback mirror download from " : "Starting download from ") + url);
            _downloadService.Start(url, _downloadedFilePath);
        }

        private void DownloadService_ProgressChanged(object sender, DownloadProgressInfo p)
        {
            progressBarDownload.Value = Math.Min(100, Math.Max(0, p.PercentComplete));
            lblDownloadPercent.Text = p.PercentComplete + "%";
            lblDownloadSize.Text = FormatBytes(p.BytesReceived) + " / " + FormatBytes(p.TotalBytesToReceive);
            lblDownloadSpeed.Text = FormatBytes((long)p.BytesPerSecond) + "/s";
        }

        private void DownloadService_Completed(object sender, EventArgs e)
        {
            Log("Download completed: " + _downloadedFilePath);

            if (_usingFallbackMirror)
            {
                string extractedExePath, extractError;
                if (!TryExtractSingleExe(_downloadedFilePath, out extractedExePath, out extractError))
                {
                    Log("Fallback package extraction failed: " + extractError);
                    SetStatus("Download failed", Color.Red);
                    SetBusyState(false);
                    return;
                }

                TryDeleteFile(_downloadedFilePath);
                _downloadedFilePath = extractedExePath;
                Log("Extracted installer from fallback package: " + extractedExePath);
            }

            string verifyError;
            if (!DownloadVerifier.Verify(_downloadedFilePath, _recommended.Checksum, out verifyError))
            {
                Log("Verification failed: " + verifyError);
                SetStatus("Download failed", Color.Red);
                SetBusyState(false);
                return;
            }

            Log("Download verified.");
            StartInstall();
        }

        // The primary URL is Intel's own mirror, which is the freshest source but whose numeric
        // IDs occasionally go dead. FallbackUrl points at a self-hosted mirror of the same file
        // (packaged as a .zip) so a dead Intel ID doesn't block installation entirely.
        private bool TryExtractSingleExe(string zipPath, out string exePath, out string error)
        {
            exePath = null;
            error = null;
            try
            {
                string extractDir = Path.Combine(Path.GetTempPath(), "IntelRSTAutoInstaller_" + Guid.NewGuid().ToString("N").Substring(0, 8));
                Directory.CreateDirectory(extractDir);
                ZipFile.ExtractToDirectory(zipPath, extractDir);
                _extractDirToCleanUp = extractDir;

                string[] exeFiles = Directory.GetFiles(extractDir, "*.exe", SearchOption.AllDirectories);
                if (exeFiles.Length != 1)
                {
                    error = "Expected exactly 1 .exe inside the fallback package, found " + exeFiles.Length + ".";
                    return false;
                }

                exePath = exeFiles[0];
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private void DownloadService_Failed(object sender, Exception ex)
        {
            if (!_usingFallbackMirror && _recommended != null && !string.IsNullOrEmpty(_recommended.FallbackUrl))
            {
                Log("Primary download failed (" + ex.Message + "); trying fallback mirror...");
                StartDownload(_recommended.FallbackUrl, true);
                return;
            }

            Log("Download failed: " + ex.Message);
            SetStatus("Download failed", Color.Red);
            SetBusyState(false);
        }

        private void DownloadService_Cancelled(object sender, EventArgs e)
        {
            Log("Download cancelled by user.");
            SetStatus("Idle", Color.Gray);
            SetBusyState(false);
        }

        private void StartInstall()
        {
            SetStatus("Installing...", Color.SteelBlue);
            progressBarDownload.Style = ProgressBarStyle.Marquee;
            // Cancel is only meaningful during Download. Once SetupRST.exe is actually running,
            // killing it mid-install risks leaving the storage driver in a half-installed state.
            btnCancel.Enabled = false;

            bool silent = chkSilentInstall.Checked;
            Log("Starting installer (silent=" + silent + ")...");
            _installWorker.RunWorkerAsync(silent);
        }

        private void InstallWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool silent = (bool)e.Argument;
            var psi = InstallService.BuildStartInfo(_downloadedFilePath, silent);
            using (var installProcess = Process.Start(psi))
            {
                installProcess.WaitForExit();
                e.Result = installProcess.ExitCode;
            }
        }

        private void InstallWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBarDownload.Style = ProgressBarStyle.Continuous;

            try
            {
                if (e.Error != null)
                {
                    Log("Installation error: " + e.Error.Message);
                    SetStatus("Installation failed", Color.Red);
                    return;
                }

                int exitCode = (int)e.Result;
                Log("Installer exit code: " + exitCode);

                if (InstallService.IsSuccessExitCode(exitCode))
                {
                    if (InstallService.RequiresReboot(exitCode))
                    {
                        Log("A reboot is required to complete installation.");
                    }
                    SetStatus("Done", Color.Green);
                }
                else
                {
                    SetStatus("Installation failed", Color.Red);
                }
            }
            finally
            {
                SetBusyState(false);
                TryDeleteDownloadedFile();
            }
        }

        private void TryDeleteDownloadedFile()
        {
            TryDeleteFile(_downloadedFilePath);

            if (!string.IsNullOrEmpty(_extractDirToCleanUp))
            {
                try
                {
                    if (Directory.Exists(_extractDirToCleanUp))
                    {
                        Directory.Delete(_extractDirToCleanUp, true);
                    }
                }
                catch (Exception ex)
                {
                    Log("Could not delete temporary extract folder: " + ex.Message);
                }
                _extractDirToCleanUp = null;
            }
        }

        private void TryDeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                Log("Could not delete temporary file: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Cancel is disabled once Installing starts (see StartInstall) — only the Download
            // step can be cancelled here.
            if (_downloadService.IsBusy)
            {
                _downloadService.Cancel();
            }
        }

        #endregion

        #region Donate

        private const string DonateUrl = "https://ko-fi.com/thanhnguyen150993";

        private Image LoadDonateButtonImage()
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream("donate.jpg"))
            {
                if (stream == null) return null;

                using (var original = Image.FromStream(stream))
                {
                    // Scale to fit the 24px-tall status strip while keeping the source aspect ratio.
                    int targetHeight = 20;
                    int targetWidth = original.Width * targetHeight / original.Height;
                    var scaled = new Bitmap(targetWidth, targetHeight);
                    using (var g = Graphics.FromImage(scaled))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(original, 0, 0, targetWidth, targetHeight);
                    }
                    return scaled;
                }
            }
        }

        private void btnDonate_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(DonateUrl);
            }
            catch (Exception ex)
            {
                Log("Could not open donate link: " + ex.Message);
            }
        }

        #endregion

        #region UI helpers

        private void SetStatus(string text, Color color)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = color;
        }

        private void SetBusyState(bool busy)
        {
            btnDownloadInstall.Enabled = !busy;
            btnCancel.Enabled = busy;
            btnRescan.Enabled = !busy;
            chkSilentInstall.Enabled = !busy;
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes < 0) bytes = 0;
            double b = bytes;
            string[] units = { "B", "KB", "MB", "GB" };
            int unitIndex = 0;
            while (b >= 1024 && unitIndex < units.Length - 1)
            {
                b /= 1024;
                unitIndex++;
            }
            return b.ToString(unitIndex == 0 ? "0" : "0.0") + " " + units[unitIndex];
        }

        #endregion

        #region Log actions

        private void btnRescan_Click(object sender, EventArgs e)
        {
            RunDetection();
        }

        private void btnCopyLog_Click(object sender, EventArgs e)
        {
            if (txtLog.Text.Length > 0)
            {
                Clipboard.SetText(txtLog.Text);
            }
        }

        private void btnExportLog_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                dlg.FileName = "IntelRST_Install_Log_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    File.WriteAllText(dlg.FileName, txtLog.Text);
                }
            }
        }

        #endregion

        #region Report an issue

        private const string SupportEmail = "minhthanhitx93@gmail.com";
        private const string GitHubIssuesUrl = "https://github.com/ThanhNguyenVN93/rst-installer/issues/new";
        private const bool GitHubIssuesEnabled = true;
        private const int MaxLogCharsInEmailBody = 3000;

        private void lnkReportIssue_Click(object sender, EventArgs e)
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("Email support", null, ReportIssue_EmailSupport_Click);
            if (GitHubIssuesEnabled)
            {
                menu.Items.Add("Open GitHub Issues", null, ReportIssue_GitHubIssues_Click);
            }
            menu.Show(Cursor.Position);
        }

        private string BuildDetectionSummary()
        {
            return
                "App version: " + lblAppVersion.Text + Environment.NewLine +
                "CPU: " + lblCpuNameValue.Text + Environment.NewLine +
                "Manufacturer: " + lblManufacturerValue.Text + Environment.NewLine +
                "Generation: " + lblGenerationValue.Text + Environment.NewLine +
                "Detection method: " + lblDetectionMethod.Text + Environment.NewLine +
                "SATA Controller Mode: " + lblSataModeValue.Text + Environment.NewLine +
                "VMD (NVMe) Status: " + lblVmdStatusValue.Text + Environment.NewLine +
                "Installed RST version: " + lblInstalledVersionValue.Text + Environment.NewLine +
                "Recommended RST version: " + lblRecommendedVersionValue.Text + Environment.NewLine +
                "Status: " + lblStatus.Text;
        }

        private void ReportIssue_EmailSupport_Click(object sender, EventArgs e)
        {
            string subject = "Intel RST Auto Installer - Issue Report";
            string log = txtLog.Text;
            if (log.Length > MaxLogCharsInEmailBody)
            {
                log = "...(truncated)...\r\n" + log.Substring(log.Length - MaxLogCharsInEmailBody);
            }

            string body = BuildDetectionSummary() + Environment.NewLine + Environment.NewLine +
                "--- Log ---" + Environment.NewLine + log;

            string mailto = "mailto:" + SupportEmail +
                "?subject=" + Uri.EscapeDataString(subject) +
                "&body=" + Uri.EscapeDataString(body);

            try
            {
                Process.Start(mailto);
            }
            catch (Exception ex)
            {
                Log("Could not open mail client: " + ex.Message);
            }
        }

        private void ReportIssue_GitHubIssues_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(GitHubIssuesUrl);
            }
            catch (Exception ex)
            {
                Log("Could not open GitHub Issues: " + ex.Message);
            }
        }

        #endregion
    }
}
