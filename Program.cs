using System;
using System.Threading;
using System.Windows.Forms;
using frm_Intel_RST_Auto_Installer.Services;

namespace frm_Intel_RST_Auto_Installer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.Run(new Form1());
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleFatalException(e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleFatalException(e.ExceptionObject as Exception);
        }

        private static void HandleFatalException(Exception ex)
        {
            string details = ex != null ? ex.ToString() : "Unknown fatal error (no exception object available).";
            FileLogger.Append("[FATAL] " + details);

            MessageBox.Show(
                "An unexpected error occurred and the operation could not continue.\r\n\r\n" +
                (ex != null ? ex.Message : "Unknown error") +
                "\r\n\r\nDetails were written to:\r\n" + FileLogger.LogFilePath,
                "Intel RST Auto Installer - Unexpected Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
