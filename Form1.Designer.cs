namespace frm_Intel_RST_Auto_Installer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        // System Information
        private System.Windows.Forms.GroupBox grpSystemInfo;
        private System.Windows.Forms.Label lblCpu;
        private System.Windows.Forms.Label lblCpuNameValue;
        private System.Windows.Forms.Label lblGeneration;
        private System.Windows.Forms.Label lblGenerationValue;
        private System.Windows.Forms.Label lblManufacturer;
        private System.Windows.Forms.Label lblManufacturerValue;
        private System.Windows.Forms.Label lblDetectionMethod;
        private System.Windows.Forms.Button btnRescan;

        // Storage Configuration
        private System.Windows.Forms.GroupBox grpStorage;
        private System.Windows.Forms.Label lblSataMode;
        private System.Windows.Forms.Label lblSataModeValue;
        private System.Windows.Forms.Label lblVmdStatus;
        private System.Windows.Forms.Label lblVmdStatusValue;
        private System.Windows.Forms.Panel pnlVmdWarning;
        private System.Windows.Forms.PictureBox picWarningIcon;
        private System.Windows.Forms.Label lblWarningText;

        // Intel RST Driver
        private System.Windows.Forms.GroupBox grpDriver;
        private System.Windows.Forms.Label lblInstalledVersion;
        private System.Windows.Forms.Label lblInstalledVersionValue;
        private System.Windows.Forms.Label lblRecommendedVersion;
        private System.Windows.Forms.Label lblRecommendedVersionValue;
        private System.Windows.Forms.Label lblDownloadSourceCaption;
        private System.Windows.Forms.TextBox txtDownloadSource;
        private System.Windows.Forms.ProgressBar progressBarDownload;
        private System.Windows.Forms.Label lblDownloadPercent;
        private System.Windows.Forms.Label lblDownloadSize;
        private System.Windows.Forms.Label lblDownloadSpeed;
        private System.Windows.Forms.Label lblStatus;

        // Actions
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.CheckBox chkSilentInstall;
        private System.Windows.Forms.Button btnDownloadInstall;
        private System.Windows.Forms.Button btnCancel;

        // Log
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnCopyLog;
        private System.Windows.Forms.Button btnExportLog;

        // Donate
        private System.Windows.Forms.Panel pnlDonate;

        // Status strip
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblAppVersion;
        private System.Windows.Forms.ToolStripStatusLabel lblConnectionStatus;
        private System.Windows.Forms.ToolStripStatusLabel lnkReportIssue;
        private System.Windows.Forms.ToolStripButton btnDonate;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.grpSystemInfo = new System.Windows.Forms.GroupBox();
            this.lblCpu = new System.Windows.Forms.Label();
            this.lblCpuNameValue = new System.Windows.Forms.Label();
            this.lblGeneration = new System.Windows.Forms.Label();
            this.lblGenerationValue = new System.Windows.Forms.Label();
            this.lblManufacturer = new System.Windows.Forms.Label();
            this.lblManufacturerValue = new System.Windows.Forms.Label();
            this.lblDetectionMethod = new System.Windows.Forms.Label();
            this.btnRescan = new System.Windows.Forms.Button();

            this.grpStorage = new System.Windows.Forms.GroupBox();
            this.lblSataMode = new System.Windows.Forms.Label();
            this.lblSataModeValue = new System.Windows.Forms.Label();
            this.lblVmdStatus = new System.Windows.Forms.Label();
            this.lblVmdStatusValue = new System.Windows.Forms.Label();
            this.pnlVmdWarning = new System.Windows.Forms.Panel();
            this.picWarningIcon = new System.Windows.Forms.PictureBox();
            this.lblWarningText = new System.Windows.Forms.Label();

            this.grpDriver = new System.Windows.Forms.GroupBox();
            this.lblInstalledVersion = new System.Windows.Forms.Label();
            this.lblInstalledVersionValue = new System.Windows.Forms.Label();
            this.lblRecommendedVersion = new System.Windows.Forms.Label();
            this.lblRecommendedVersionValue = new System.Windows.Forms.Label();
            this.lblDownloadSourceCaption = new System.Windows.Forms.Label();
            this.txtDownloadSource = new System.Windows.Forms.TextBox();
            this.progressBarDownload = new System.Windows.Forms.ProgressBar();
            this.lblDownloadPercent = new System.Windows.Forms.Label();
            this.lblDownloadSize = new System.Windows.Forms.Label();
            this.lblDownloadSpeed = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();

            this.grpActions = new System.Windows.Forms.GroupBox();
            this.chkSilentInstall = new System.Windows.Forms.CheckBox();
            this.btnDownloadInstall = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            this.grpLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnCopyLog = new System.Windows.Forms.Button();
            this.btnExportLog = new System.Windows.Forms.Button();

            this.pnlDonate = new System.Windows.Forms.Panel();

            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblAppVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblConnectionStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lnkReportIssue = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnDonate = new System.Windows.Forms.ToolStripButton();

            this.grpStorage.SuspendLayout();
            this.pnlVmdWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWarningIcon)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();

            // grpSystemInfo
            this.grpSystemInfo.Text = "System Information";
            this.grpSystemInfo.Location = new System.Drawing.Point(10, 10);
            this.grpSystemInfo.Size = new System.Drawing.Size(536, 115);

            this.lblCpu.Text = "CPU:";
            this.lblCpu.Location = new System.Drawing.Point(10, 25);
            this.lblCpu.Size = new System.Drawing.Size(90, 18);

            this.lblCpuNameValue.Text = "-";
            this.lblCpuNameValue.Location = new System.Drawing.Point(110, 25);
            this.lblCpuNameValue.Size = new System.Drawing.Size(410, 18);
            this.lblCpuNameValue.AutoEllipsis = true;

            this.lblGeneration.Text = "Generation:";
            this.lblGeneration.Location = new System.Drawing.Point(10, 48);
            this.lblGeneration.Size = new System.Drawing.Size(90, 18);

            this.lblGenerationValue.Text = "-";
            this.lblGenerationValue.Location = new System.Drawing.Point(110, 48);
            this.lblGenerationValue.Size = new System.Drawing.Size(300, 18);

            this.lblManufacturer.Text = "Manufacturer:";
            this.lblManufacturer.Location = new System.Drawing.Point(10, 71);
            this.lblManufacturer.Size = new System.Drawing.Size(90, 18);

            this.lblManufacturerValue.Text = "-";
            this.lblManufacturerValue.Location = new System.Drawing.Point(110, 71);
            this.lblManufacturerValue.Size = new System.Drawing.Size(300, 18);

            this.lblDetectionMethod.Text = "";
            this.lblDetectionMethod.ForeColor = System.Drawing.Color.Gray;
            this.lblDetectionMethod.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.lblDetectionMethod.Location = new System.Drawing.Point(10, 92);
            this.lblDetectionMethod.Size = new System.Drawing.Size(300, 16);

            this.btnRescan.Text = "Rescan";
            this.btnRescan.Location = new System.Drawing.Point(436, 86);
            this.btnRescan.Size = new System.Drawing.Size(90, 25);
            this.btnRescan.Click += new System.EventHandler(this.btnRescan_Click);

            this.grpSystemInfo.Controls.Add(this.lblCpu);
            this.grpSystemInfo.Controls.Add(this.lblCpuNameValue);
            this.grpSystemInfo.Controls.Add(this.lblGeneration);
            this.grpSystemInfo.Controls.Add(this.lblGenerationValue);
            this.grpSystemInfo.Controls.Add(this.lblManufacturer);
            this.grpSystemInfo.Controls.Add(this.lblManufacturerValue);
            this.grpSystemInfo.Controls.Add(this.lblDetectionMethod);
            this.grpSystemInfo.Controls.Add(this.btnRescan);

            // grpStorage
            this.grpStorage.Text = "Storage Configuration";
            this.grpStorage.Location = new System.Drawing.Point(10, 135);
            this.grpStorage.Size = new System.Drawing.Size(536, 115);

            this.lblSataMode.Text = "SATA Controller Mode:";
            this.lblSataMode.Location = new System.Drawing.Point(10, 25);
            this.lblSataMode.Size = new System.Drawing.Size(130, 18);

            this.lblSataModeValue.Text = "-";
            this.lblSataModeValue.Location = new System.Drawing.Point(150, 25);
            this.lblSataModeValue.Size = new System.Drawing.Size(260, 18);

            this.lblVmdStatus.Text = "VMD (NVMe) Status:";
            this.lblVmdStatus.Location = new System.Drawing.Point(10, 48);
            this.lblVmdStatus.Size = new System.Drawing.Size(130, 18);

            this.lblVmdStatusValue.Text = "-";
            this.lblVmdStatusValue.Location = new System.Drawing.Point(150, 48);
            this.lblVmdStatusValue.Size = new System.Drawing.Size(260, 18);

            this.pnlVmdWarning.Location = new System.Drawing.Point(10, 72);
            this.pnlVmdWarning.Size = new System.Drawing.Size(516, 36);
            this.pnlVmdWarning.Visible = false;

            this.picWarningIcon.Image = System.Drawing.SystemIcons.Warning.ToBitmap();
            this.picWarningIcon.Location = new System.Drawing.Point(4, 4);
            this.picWarningIcon.Size = new System.Drawing.Size(24, 24);
            this.picWarningIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;

            this.lblWarningText.Text = "VMD is enabled. Third-party SSD tools (e.g. Samsung Magician) may not read drive info correctly after installing RST.";
            this.lblWarningText.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblWarningText.Location = new System.Drawing.Point(34, 2);
            this.lblWarningText.Size = new System.Drawing.Size(478, 32);

            this.pnlVmdWarning.Controls.Add(this.picWarningIcon);
            this.pnlVmdWarning.Controls.Add(this.lblWarningText);

            this.grpStorage.Controls.Add(this.lblSataMode);
            this.grpStorage.Controls.Add(this.lblSataModeValue);
            this.grpStorage.Controls.Add(this.lblVmdStatus);
            this.grpStorage.Controls.Add(this.lblVmdStatusValue);
            this.grpStorage.Controls.Add(this.pnlVmdWarning);

            // grpDriver
            this.grpDriver.Text = "Intel RST Driver";
            this.grpDriver.Location = new System.Drawing.Point(10, 260);
            this.grpDriver.Size = new System.Drawing.Size(536, 175);

            this.lblInstalledVersion.Text = "Installed Version:";
            this.lblInstalledVersion.Location = new System.Drawing.Point(10, 25);
            this.lblInstalledVersion.Size = new System.Drawing.Size(130, 18);

            this.lblInstalledVersionValue.Text = "-";
            this.lblInstalledVersionValue.Location = new System.Drawing.Point(145, 25);
            this.lblInstalledVersionValue.Size = new System.Drawing.Size(280, 18);

            this.lblRecommendedVersion.Text = "Recommended Version:";
            this.lblRecommendedVersion.Location = new System.Drawing.Point(10, 48);
            this.lblRecommendedVersion.Size = new System.Drawing.Size(130, 18);

            this.lblRecommendedVersionValue.Text = "-";
            this.lblRecommendedVersionValue.Location = new System.Drawing.Point(145, 48);
            this.lblRecommendedVersionValue.Size = new System.Drawing.Size(280, 18);

            this.lblDownloadSourceCaption.Text = "Download Source:";
            this.lblDownloadSourceCaption.Location = new System.Drawing.Point(10, 71);
            this.lblDownloadSourceCaption.Size = new System.Drawing.Size(130, 18);

            this.txtDownloadSource.Location = new System.Drawing.Point(145, 69);
            this.txtDownloadSource.Size = new System.Drawing.Size(380, 20);
            this.txtDownloadSource.ReadOnly = true;

            this.progressBarDownload.Location = new System.Drawing.Point(10, 96);
            this.progressBarDownload.Size = new System.Drawing.Size(440, 20);
            this.progressBarDownload.Style = System.Windows.Forms.ProgressBarStyle.Continuous;

            this.lblDownloadPercent.Text = "0%";
            this.lblDownloadPercent.Location = new System.Drawing.Point(456, 98);
            this.lblDownloadPercent.Size = new System.Drawing.Size(60, 18);

            this.lblDownloadSize.Text = "";
            this.lblDownloadSize.ForeColor = System.Drawing.Color.Gray;
            this.lblDownloadSize.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.lblDownloadSize.Location = new System.Drawing.Point(10, 120);
            this.lblDownloadSize.Size = new System.Drawing.Size(220, 16);

            this.lblDownloadSpeed.Text = "";
            this.lblDownloadSpeed.ForeColor = System.Drawing.Color.Gray;
            this.lblDownloadSpeed.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.lblDownloadSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblDownloadSpeed.Location = new System.Drawing.Point(300, 120);
            this.lblDownloadSpeed.Size = new System.Drawing.Size(216, 16);

            this.lblStatus.Text = "Idle";
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(10, 144);
            this.lblStatus.Size = new System.Drawing.Size(500, 20);

            this.grpDriver.Controls.Add(this.lblInstalledVersion);
            this.grpDriver.Controls.Add(this.lblInstalledVersionValue);
            this.grpDriver.Controls.Add(this.lblRecommendedVersion);
            this.grpDriver.Controls.Add(this.lblRecommendedVersionValue);
            this.grpDriver.Controls.Add(this.lblDownloadSourceCaption);
            this.grpDriver.Controls.Add(this.txtDownloadSource);
            this.grpDriver.Controls.Add(this.progressBarDownload);
            this.grpDriver.Controls.Add(this.lblDownloadPercent);
            this.grpDriver.Controls.Add(this.lblDownloadSize);
            this.grpDriver.Controls.Add(this.lblDownloadSpeed);
            this.grpDriver.Controls.Add(this.lblStatus);

            // grpActions
            this.grpActions.Text = "Actions";
            this.grpActions.Location = new System.Drawing.Point(10, 445);
            this.grpActions.Size = new System.Drawing.Size(536, 65);

            this.chkSilentInstall.Text = "Silent install (no UI)";
            this.chkSilentInstall.Checked = true;
            this.chkSilentInstall.Location = new System.Drawing.Point(10, 27);
            this.chkSilentInstall.Size = new System.Drawing.Size(160, 22);

            this.btnDownloadInstall.Text = "Download && Install";
            this.btnDownloadInstall.Location = new System.Drawing.Point(200, 22);
            this.btnDownloadInstall.Size = new System.Drawing.Size(180, 30);
            this.btnDownloadInstall.Enabled = false;
            this.btnDownloadInstall.Click += new System.EventHandler(this.btnDownloadInstall_Click);

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(390, 22);
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.Enabled = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            this.grpActions.Controls.Add(this.chkSilentInstall);
            this.grpActions.Controls.Add(this.btnDownloadInstall);
            this.grpActions.Controls.Add(this.btnCancel);

            // grpLog
            this.grpLog.Text = "Log";
            this.grpLog.Location = new System.Drawing.Point(10, 520);
            this.grpLog.Size = new System.Drawing.Size(536, 150);

            this.txtLog.Multiline = true;
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 8F);
            this.txtLog.Location = new System.Drawing.Point(10, 20);
            this.txtLog.Size = new System.Drawing.Size(516, 95);

            this.btnCopyLog.Text = "Copy Log";
            this.btnCopyLog.Location = new System.Drawing.Point(10, 120);
            this.btnCopyLog.Size = new System.Drawing.Size(90, 23);
            this.btnCopyLog.Click += new System.EventHandler(this.btnCopyLog_Click);

            this.btnExportLog.Text = "Export Log";
            this.btnExportLog.Location = new System.Drawing.Point(110, 120);
            this.btnExportLog.Size = new System.Drawing.Size(90, 23);
            this.btnExportLog.Click += new System.EventHandler(this.btnExportLog_Click);

            this.grpLog.Controls.Add(this.txtLog);
            this.grpLog.Controls.Add(this.btnCopyLog);
            this.grpLog.Controls.Add(this.btnExportLog);

            // pnlDonate
            this.pnlDonate.Location = new System.Drawing.Point(10, 680);
            this.pnlDonate.Size = new System.Drawing.Size(536, 50);
            this.pnlDonate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // statusStrip1
            this.lblAppVersion.Text = "v1.0.0";
            this.lblConnectionStatus.Text = "Ready";
            this.lblConnectionStatus.Spring = true;
            this.lblConnectionStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAppVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.lnkReportIssue.Text = "Report an issue";
            this.lnkReportIssue.IsLink = true;
            this.lnkReportIssue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lnkReportIssue.Click += new System.EventHandler(this.lnkReportIssue_Click);

            this.btnDonate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDonate.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnDonate.ToolTipText = "Donate";
            this.btnDonate.Margin = new System.Windows.Forms.Padding(6, 1, 0, 1);
            this.btnDonate.Click += new System.EventHandler(this.btnDonate_Click);

            this.statusStrip1.Items.Add(this.lblConnectionStatus);
            this.statusStrip1.Items.Add(this.lnkReportIssue);
            this.statusStrip1.Items.Add(this.btnDonate);
            this.statusStrip1.Items.Add(this.lblAppVersion);
            this.statusStrip1.Location = new System.Drawing.Point(0, 745);
            this.statusStrip1.Size = new System.Drawing.Size(556, 24);

            // Form1
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 769);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Intel RST Auto Installer";

            this.Controls.Add(this.grpSystemInfo);
            this.Controls.Add(this.grpStorage);
            this.Controls.Add(this.grpDriver);
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.grpLog);
            this.Controls.Add(this.pnlDonate);
            this.Controls.Add(this.statusStrip1);

            this.Load += new System.EventHandler(this.Form1_Load);

            this.grpStorage.ResumeLayout(false);
            this.pnlVmdWarning.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picWarningIcon)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
