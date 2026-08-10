using System;
using System.ComponentModel;
using System.Net;
using frm_Intel_RST_Auto_Installer.Models;

namespace frm_Intel_RST_Auto_Installer.Services
{
    public class DownloadService
    {
        private const int TimeoutSeconds = 60;

        private WebClient _webClient;
        private DateTime _lastProgressTime;
        private DateTime _lastSampleTime;
        private long _lastSampleBytes;
        private System.Windows.Forms.Timer _timeoutTimer;
        private bool _cancelledByTimeout;

        public event Action<object, DownloadProgressInfo> ProgressChanged;
        public event EventHandler Completed;
        public event Action<object, Exception> Failed;
        public event EventHandler Cancelled;

        public bool IsBusy
        {
            get { return _webClient != null; }
        }

        public void Start(string url, string destinationPath)
        {
            if (IsBusy) throw new InvalidOperationException("A download is already in progress.");

            _cancelledByTimeout = false;
            _lastProgressTime = DateTime.Now;
            _lastSampleTime = DateTime.Now;
            _lastSampleBytes = 0;

            _webClient = new WebClient();
            _webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            _webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

            _timeoutTimer = new System.Windows.Forms.Timer();
            _timeoutTimer.Interval = 5000;
            _timeoutTimer.Tick += TimeoutTimer_Tick;
            _timeoutTimer.Start();

            _webClient.DownloadFileAsync(new Uri(url), destinationPath);
        }

        public void Cancel()
        {
            if (_webClient == null) return;
            _webClient.CancelAsync();
        }

        private void TimeoutTimer_Tick(object sender, EventArgs e)
        {
            if ((DateTime.Now - _lastProgressTime).TotalSeconds >= TimeoutSeconds)
            {
                _cancelledByTimeout = true;
                if (_webClient != null) _webClient.CancelAsync();
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _lastProgressTime = DateTime.Now;

            double elapsed = (DateTime.Now - _lastSampleTime).TotalSeconds;
            double speed = 0;
            if (elapsed >= 0.5)
            {
                speed = (e.BytesReceived - _lastSampleBytes) / elapsed;
                _lastSampleTime = DateTime.Now;
                _lastSampleBytes = e.BytesReceived;
            }

            if (ProgressChanged != null)
            {
                ProgressChanged(this, new DownloadProgressInfo
                {
                    PercentComplete = e.ProgressPercentage,
                    BytesReceived = e.BytesReceived,
                    TotalBytesToReceive = e.TotalBytesToReceive,
                    BytesPerSecond = speed
                });
            }
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Cleanup();

            if (e.Cancelled)
            {
                if (_cancelledByTimeout)
                {
                    if (Failed != null) Failed(this, new TimeoutException("Download timed out (no traffic for " + TimeoutSeconds + "s)."));
                }
                else if (Cancelled != null)
                {
                    Cancelled(this, EventArgs.Empty);
                }
                return;
            }

            if (e.Error != null)
            {
                if (Failed != null) Failed(this, e.Error);
                return;
            }

            if (Completed != null) Completed(this, EventArgs.Empty);
        }

        private void Cleanup()
        {
            if (_timeoutTimer != null)
            {
                _timeoutTimer.Stop();
                _timeoutTimer.Dispose();
                _timeoutTimer = null;
            }
            if (_webClient != null)
            {
                _webClient.Dispose();
                _webClient = null;
            }
        }
    }
}
