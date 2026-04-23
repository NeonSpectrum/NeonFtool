using System;
using System.Threading;
using System.Windows.Forms;

namespace NeonFtool
{
    internal static class Program
    {
        private static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new NeonFtool());
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("An instance of NeonFtool is already running.", "NeonFtool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
