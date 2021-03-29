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

        Settings settings;

        public DumpCleanerForm()
        {
            InitializeComponent();

            this.settings = Settings.Get();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();

            string path = folderBrowserDialog.SelectedPath;

            if (File.Exists(path + "\\Client.exe") == false)
            {
                MessageBox.Show("Cannot find Client.exe on this folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            insanityFlyffPathTextBox.Text = path;
        }

        private void cleanDumpButton_Click(object sender, EventArgs e)
        {
            string path = insanityFlyffPathTextBox.Text;

            if (path == Constants.BROWSE_LABEL)
            {
                MessageBox.Show("Please select Insanity Flyff folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                CleanDumpInsanityFlyff(path, rptCheckBox.Checked, dmpCheckBox.Checked, txtCheckBox.Checked);
            }
            catch
            {
                MessageBox.Show("Delete Dump Failed. Insufficient Permission.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            MessageBox.Show("Dump Cleared!", "Success");
        }

        private void executeStartupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (insanityFlyffPathTextBox.Text == Constants.BROWSE_LABEL)
            {
                MessageBox.Show("Please select Insanity Flyff folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                executeStartupCheckBox.CheckedChanged -= executeStartupCheckBox_CheckedChanged;
                executeStartupCheckBox.Checked = false;
                executeStartupCheckBox.CheckedChanged += executeStartupCheckBox_CheckedChanged;

            }
        }

        public void CleanDumpInsanityFlyff(string path, bool rpt, bool dmp, bool txt)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                if (
                    file.Name.StartsWith("Client_") &&
                    (
                        rpt && file.Name.EndsWith(".rpt") ||
                        dmp && file.Name.EndsWith(".dmp") ||
                        txt && file.Name.EndsWith(".txt")
                    )
                )
                {
                    file.Delete();
                }
            }
        }

        private void LoadSettings()
        {
            executeStartupCheckBox.Checked = (bool)Settings.GetOrDefault(settings.DumpCleaner, "autoCleanDumpOnStartup", false);
            rptCheckBox.Checked = (bool)Settings.GetOrDefault(settings.DumpCleaner, "rpt", true);
            dmpCheckBox.Checked = (bool)Settings.GetOrDefault(settings.DumpCleaner, "dmp", true);
            txtCheckBox.Checked = (bool)Settings.GetOrDefault(settings.DumpCleaner, "txt", true);
            insanityFlyffPathTextBox.Text = Settings.GetOrDefault(settings.DumpCleaner, "path", Constants.BROWSE_LABEL).ToString();

            settings.Save();
        }

        private void SaveSettings()
        {
            settings.DumpCleaner["autoCleanDumpOnStartup"] = executeStartupCheckBox.Checked;
            settings.DumpCleaner["rpt"] = rptCheckBox.Checked;
            settings.DumpCleaner["dmp"] = dmpCheckBox.Checked;
            settings.DumpCleaner["txt"] = txtCheckBox.Checked;
            settings.DumpCleaner["path"] = insanityFlyffPathTextBox.Text != Constants.BROWSE_LABEL ? insanityFlyffPathTextBox.Text : "";


            settings.Save();
        }
        private void DumpCleanerForm_Load(object sender, EventArgs e)
        {
            this.Text = Constants.DUMP_CLEANER_TITLE;

            LoadSettings();
        }

        private void DumpCleanerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void DumpCleanerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            WindowClosed();
        }
    }
}
