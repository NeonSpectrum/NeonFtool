using NeonFtool.Classes;
using System;
using System.IO;
using System.Windows.Forms;

namespace NeonFtool.Forms
{
    public partial class DumpCleanerForm : Form
    {
        public delegate void CloseEvent();
        public CloseEvent WindowClosed;

        private Settings _settings;

        public DumpCleanerForm()
        {
            InitializeComponent();
            _settings = Settings.Get();
        }

        private void DumpCleanerForm_Load(object sender, EventArgs e)
        {
            Text = Constants.DUMP_CLEANER_TITLE;
            LoadSettings();
        }

        private void DumpCleanerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void DumpCleanerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            WindowClosed?.Invoke();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK) return;

            string path = folderBrowserDialog.SelectedPath;

            if (!File.Exists(Path.Combine(path, "Client.exe")))
            {
                MessageBox.Show("Cannot find Client.exe in this folder.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            insanityFlyffPathTextBox.Text = path;
        }

        private void cleanDumpButton_Click(object sender, EventArgs e)
        {
            string path = insanityFlyffPathTextBox.Text;

            if (path == Constants.BROWSE_LABEL || string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("Please select the Insanity Flyff folder.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                CleanDumpInsanityFlyff(path, rptCheckBox.Checked, dmpCheckBox.Checked, txtCheckBox.Checked);
                MessageBox.Show("Dump cleared!", "Success");
            }
            catch
            {
                MessageBox.Show("Delete dump failed. Insufficient permissions.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void executeStartupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            string path = insanityFlyffPathTextBox.Text;

            if (path == Constants.BROWSE_LABEL || string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("Please select the Insanity Flyff folder.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Temporarily detach the handler to prevent re-entry.
                executeStartupCheckBox.CheckedChanged -= executeStartupCheckBox_CheckedChanged;
                executeStartupCheckBox.Checked = false;
                executeStartupCheckBox.CheckedChanged += executeStartupCheckBox_CheckedChanged;
            }
        }

        public void CleanDumpInsanityFlyff(string path, bool rpt, bool dmp, bool txt)
        {
            foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
            {
                if (!file.Name.StartsWith("Client_")) continue;

                bool shouldDelete = (rpt && file.Name.EndsWith(".rpt"))
                                 || (dmp && file.Name.EndsWith(".dmp"))
                                 || (txt && file.Name.EndsWith(".txt"));

                if (shouldDelete) file.Delete();
            }
        }

        private void LoadSettings()
        {
            executeStartupCheckBox.Checked = (bool)Settings.GetOrDefault(_settings.DumpCleaner, "autoCleanDumpOnStartup", false);
            rptCheckBox.Checked            = (bool)Settings.GetOrDefault(_settings.DumpCleaner, "rpt",  true);
            dmpCheckBox.Checked            = (bool)Settings.GetOrDefault(_settings.DumpCleaner, "dmp",  true);
            txtCheckBox.Checked            = (bool)Settings.GetOrDefault(_settings.DumpCleaner, "txt",  true);
            insanityFlyffPathTextBox.Text   = Settings.GetOrDefault(_settings.DumpCleaner, "path", Constants.BROWSE_LABEL).ToString();
        }

        private void SaveSettings()
        {
            _settings.DumpCleaner["autoCleanDumpOnStartup"] = executeStartupCheckBox.Checked;
            _settings.DumpCleaner["rpt"]  = rptCheckBox.Checked;
            _settings.DumpCleaner["dmp"]  = dmpCheckBox.Checked;
            _settings.DumpCleaner["txt"]  = txtCheckBox.Checked;
            _settings.DumpCleaner["path"] = insanityFlyffPathTextBox.Text != Constants.BROWSE_LABEL
                ? insanityFlyffPathTextBox.Text
                : "";
            _settings.Save();
        }
    }
}
