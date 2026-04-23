using NeonFtool.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace NeonFtool.Forms
{
    public partial class WindowManagerForm : Form
    {
        public delegate void CloseEvent();
        public CloseEvent WindowClosed;

        private Settings _settings;
        private readonly ProcessManager _processManager;
        private readonly List<Process> _leftProcesses  = new();
        public  readonly List<Process> RightProcesses  = new();

        public WindowManagerForm()
        {
            InitializeComponent();

            _settings      = Settings.Get();
            _processManager = new ProcessManager();
        }

        private void WindowManagementForm_Load(object sender, EventArgs e)
        {
            Text = Constants.WINDOW_MANAGER_TITLE;

            LoadSettings();
            refreshButton.PerformClick();

            cpuPriorityCheckBox.CheckedChanged += CheckBoxChanged;
            hideWindowCheckBox.CheckedChanged  += CheckBoxChanged;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            GetClients();
            RefreshData();
        }

        private void RefreshData()
        {
            leftListBox.DataSource  = _leftProcesses .OrderBy(SafeStartTime).Select(p => p.MainWindowTitle).ToList();
            rightListBox.DataSource = RightProcesses.OrderBy(SafeStartTime).Select(p => p.MainWindowTitle).ToList();
        }

        private void GetClients()
        {
            _leftProcesses.Clear();

            foreach (Process process in _processManager.RefreshProcessList().GetProcessList())
            {
                if (!RightProcesses.Any(p => p.MainWindowTitle == process.MainWindowTitle))
                    _leftProcesses.Add(process);
            }
        }

        private void SetStatus(bool toRight, Process process)
        {
            if (toRight)
            {
                if (hideWindowCheckBox.Checked)
                    ProcessManager.HideWindow(process.MainWindowHandle);
                else
                    ProcessManager.ShowWindow(process.MainWindowHandle);

                process.PriorityClass = cpuPriorityCheckBox.Checked
                    ? ProcessPriorityClass.Idle
                    : ProcessPriorityClass.Normal;
            }
            else
            {
                ProcessManager.ShowWindow(process.MainWindowHandle);
                process.PriorityClass = ProcessPriorityClass.Normal;
            }
        }

        private void CheckBoxChanged(object sender, EventArgs e)
        {
            foreach (string item in rightListBox.SelectedItems)
            {
                Process process = _processManager.GetProcessByWindowTitle(item);
                if (process != null) SetStatus(toRight: true, process);
            }
        }

        private void changeToHiddenButton_Click(object sender, EventArgs e)
        {
            foreach (string item in leftListBox.SelectedItems.Cast<string>().ToList())
            {
                Process process = _processManager.GetProcessByWindowTitle(item);
                if (process == null) continue;

                SetStatus(toRight: true, process);
                RightProcesses.Add(process);
                _leftProcesses.Remove(process);
            }

            RefreshData();
        }

        private void changeToShownButton_Click(object sender, EventArgs e)
        {
            foreach (string item in rightListBox.SelectedItems.Cast<string>().ToList())
            {
                Process process = _processManager.GetProcessByWindowTitle(item);
                if (process == null) continue;

                SetStatus(toRight: false, process);
                RightProcesses.Remove(process);
                _leftProcesses.Add(process);
            }

            RefreshData();
        }

        private void LoadSettings()
        {
            cpuPriorityCheckBox.Checked = (bool)Settings.GetOrDefault(_settings.WindowManager, "cpuPriorityToIdle", false);
            hideWindowCheckBox.Checked  = (bool)Settings.GetOrDefault(_settings.WindowManager, "hideWindow",        false);
        }

        private void SaveSettings()
        {
            _settings.WindowManager["cpuPriorityToIdle"] = cpuPriorityCheckBox.Checked;
            _settings.WindowManager["hideWindow"]        = hideWindowCheckBox.Checked;
            _settings.Save();
        }

        private void WindowManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            WindowClosed?.Invoke();
            Hide();
            e.Cancel = true;
        }

        private static DateTime SafeStartTime(Process p)
        {
            try { return p.StartTime; }
            catch { return DateTime.MinValue; }
        }
    }
}
