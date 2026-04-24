using NeonFtool.Libraries;
using System;
using System.Diagnostics;
using System.Linq;

namespace NeonFtool.Classes
{
    internal class ProcessManager
    {
        private Process[] _processes = Array.Empty<Process>();
        private readonly Settings _settings;

        public ProcessManager()
        {
            _settings = Settings.Get();
        }

        public Process[] GetProcessList() => _processes;

        /// <summary>
        /// Refreshes the internal process list, filtering to only target processes,
        /// and appends the PID to each window title for unique identification.
        /// </summary>
        public ProcessManager RefreshProcessList()
        {
            _processes = Process.GetProcesses()
                .Where(p => _settings.TargetProcess.Any(name => name == p.ProcessName + ".exe"))
                .OrderBy(SafeStartTime)
                .ToArray();

            foreach (Process process in _processes)
            {
                string title = process.MainWindowTitle;
                string pidTag = $" PID -> {process.Id}";

                if (!title.Contains(pidTag))
                {
                    Function.SetWindowText(process.MainWindowHandle, title + pidTag);
                    process.Refresh(); // Update cached MainWindowTitle
                }
            }

            return this;
        }

        public Process GetProcessByWindowTitle(string name)
        {
            return _processes.FirstOrDefault(p => p.MainWindowTitle == name);
        }

        public Process GetProcessByRegex(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return null;

            try
            {
                // Escape common special characters if they are not part of a valid regex? 
                // No, let's assume the user knows it's regex.
                var regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                return _processes.FirstOrDefault(p => regex.IsMatch(p.MainWindowTitle));
            }
            catch
            {
                // Fallback to exact match if regex is invalid
                return GetProcessByWindowTitle(pattern);
            }
        }

        public Process GetProcessByPID(int pid)
        {
            return _processes.FirstOrDefault(p => p.Id == pid);
        }

        public static bool ShowWindow(IntPtr handle) =>
            Function.ShowWindow(handle, Constants.SW_SHOWNOACTIVATE);

        public static bool HideWindow(IntPtr handle) =>
            Function.ShowWindow(handle, Constants.SW_HIDE);

        /// <summary>
        /// Safe wrapper for StartTime — some system processes throw on access.
        /// </summary>
        private static DateTime SafeStartTime(Process p)
        {
            try { return p.StartTime; }
            catch { return DateTime.MinValue; }
        }
    }
}
