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

        Settings settings;

        ProcessManager processManager;
        List<Process> leftProcesses;
        public List<Process> rightProcesses;

        public WindowManagerForm()
        {
            InitializeComponent();

            settings = Settings.Get();
            processManager = new ProcessManager();
            leftProcesses = new List<Process>();
            rightProcesses = new List<Process>();
        }

        private void WindowManagementForm_Load(object sender, EventArgs e)
        {
            this.Text = Constants.WINDOW_MANAGER_TITLE;

            LoadSettings();
            refreshButton.PerformClick();

            cpuPriorityCheckBox.CheckedChanged += CheckBoxChanged;
            hideWindowCheckBox.CheckedChanged += CheckBoxChanged;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            GetClients();
            RefreshData();
        }

        private void RefreshData()
        {
            leftListBox.DataSource = leftProcesses.OrderBy(p => p.StartTime).Select(p => p.MainWindowTitle).ToList();
            rightListBox.DataSource = rightProcesses.OrderBy(p => p.StartTime).Select(p => p.MainWindowTitle).ToList();
        }

        private void GetClients()
        {
            leftProcesses.Clear();

            foreach (Process process in processManager.RefreshProcessList().GetProcessList())
            {
                if (!rightProcesses.Any(p => p.MainWindowTitle == process.MainWindowTitle))
                {
                    leftProcesses.Add(process);
                }
            }
        }

        private void SetStatus(string direction, Process process)
        {
            if (direction == "left")
            {
                ProcessManager.ShowWindow(process.MainWindowHandle);
                process.PriorityClass = ProcessPriorityClass.Normal;
            }
            else if (direction == "right")
            {
                if (hideWindowCheckBox.Checked)
                {
                    ProcessManager.HideWindow(process.MainWindowHandle);
                }
                else
                {
                    ProcessManager.ShowWindow(process.MainWindowHandle);
                }

                process.PriorityClass = cpuPriorityCheckBox.Checked ? ProcessPriorityClass.Idle : ProcessPriorityClass.Normal;
            }
        }

        private void CheckBoxChanged(object sender, EventArgs e)
        {
            var list = rightListBox.SelectedItems;

            foreach (string item in list)
            {
                Process process = processManager.GetProcessByWindowTitle(item);

                SetStatus("right", process);
            }
        }

        private void changeToHiddenButton_Click(object sender, EventArgs e)
        {
            var list = leftListBox.SelectedItems;

            foreach (string item in list)
            {
                Process process = processManager.GetProcessByWindowTitle(item);

                SetStatus("right", process);

                rightProcesses.Add(process);
                leftProcesses.Remove(process);
            }

            RefreshData();
        }

        private void changeToShownButton_Click(object sender, EventArgs e)
        {
            var list = rightListBox.SelectedItems;

            foreach (string item in list)
            {
                Process process = processManager.GetProcessByWindowTitle(item);

                SetStatus("left", process);

                rightProcesses.Remove(process);
                leftProcesses.Add(process);
            }

            RefreshData();
        }

        private void LoadSettings()
        {
            cpuPriorityCheckBox.Checked = (bool)Settings.GetOrDefault(settings.WindowManager, "cpuPriorityToIdle", false);
            hideWindowCheckBox.Checked = (bool)Settings.GetOrDefault(settings.WindowManager, "hideWindow", false);

            settings.Save();
        }

        private void SaveSettings()
        {
            settings.WindowManager["cpuPriorityToIdle"] = cpuPriorityCheckBox.Checked;
            settings.WindowManager["hideWindow"] = hideWindowCheckBox.Checked;

            settings.Save();
        }

        private void WindowManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            WindowClosed();
            Hide();
            e.Cancel = true;
        }
    }
}
