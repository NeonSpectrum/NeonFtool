using NeonFtool.Libraries;
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
                
                NeonFtool mainForm = new NeonFtool();
                mainForm.Show();
                mainForm.Activate();

                // Force the icon at the Win32 level as soon as the form is shown
                IntPtr hIcon = mainForm.Icon.Handle;
                Function.SendMessage(mainForm.Handle, Function.WM_SETICON, (IntPtr)Function.ICON_SMALL, hIcon);
                Function.SendMessage(mainForm.Handle, Function.WM_SETICON, (IntPtr)Function.ICON_BIG, hIcon);

                Application.Run(mainForm);
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("An instance of NeonFtool is already running.", "NeonFtool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
