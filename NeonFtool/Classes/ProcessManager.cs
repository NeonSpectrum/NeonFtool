using NeonFtool.Libraries;
using System;
using System.Diagnostics;
using System.Linq;

namespace NeonFtool.Classes
{
    internal class ProcessManager
    {
        private Process[] processes;
        private Settings settings;

        public ProcessManager()
        {
            settings = Settings.Get();
        }

        public Process[] GetProcessList()
        {
            return processes;
        }

        public ProcessManager RefreshProcessList()
        {
            processes = (from process in Process.GetProcesses()
                         where settings.TargetProcess.Any(name => name == process.ProcessName + ".exe")
                         select process).OrderBy(p => p.StartTime).ToArray();

            foreach (Process process in processes)
            {
                string title = process.MainWindowTitle;

                if (!title.Contains("PID -> " + process.Id))
                {
                    title += " PID -> " + process.Id;
                    Function.SetWindowText(process.MainWindowHandle, title);
                }
            }

            return this;
        }

        public Process GetProcessByWindowTitle(string name)
        {
            try
            {
                return (from process in processes
                        where process.MainWindowTitle == name
                        select process).First();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public Process GetProcessByPID(int pid)
        {
            try
            {
                return (from process in processes
                        where process.Id == pid
                        select process).First();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public static bool ShowWindow(IntPtr handler)
        {
            return Function.ShowWindow(handler, Constants.SW_SHOWNOACTIVATE);
        }
        public static bool HideWindow(IntPtr handler)
        {
            return Function.ShowWindow(handler, Constants.SW_HIDE);
        }
    }
}
