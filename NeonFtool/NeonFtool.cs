using Ftool.Libraries;
using NeonFtool.Classes;
using NeonFtool.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace NeonFtool
{
    public partial class NeonFtool : Form
    {
        private ProcessManager processManager;
        private Controller controller;
        private Events events;
        private Hotkey hotkey;
        private Settings settings;
        private DumpCleanerForm dumpCleanerForm;
        private WindowManagerForm windowManagerForm;
        private AboutForm aboutForm;

        private bool isLoaded = false;

        public NeonFtool()
        {
            processManager = new ProcessManager();
            controller = new Controller(Controls);
            hotkey = new Hotkey();
            events = new Events(processManager, hotkey);
            settings = Settings.Get();

            dumpCleanerForm = new DumpCleanerForm();
            windowManagerForm = new WindowManagerForm();
            aboutForm = new AboutForm();

            windowManagerForm.WindowClosed += ReloadSettings;

            InitializeComponent();
            ExecuteStartup();
        }

        private void Ftool_Load(object sender, EventArgs e)
        {
            this.Text = Constants.MAIN_TITLE;
        }

        private void ExecuteStartup()
        {
            RefreshWindowList();
            LoadComboBoxValues();
            SetEventHandler();
            LoadSettings();
            ExecuteDumpCleaner();

            isLoaded = true;
        }

        private void RefreshWindowList()
        {
            Process[] processes = processManager.RefreshProcessList().GetProcessList();
            ComboBox[] comboBoxes = controller.GetAllWindowComboBox();

            foreach (ComboBox comboBox in comboBoxes)
            {
                int lastIndex = 0;
                string lastValue = "";

                if (comboBox.SelectedItem != null)
                {
                    lastValue = (comboBox.SelectedItem as ComboBoxItem).Text;
                }

                comboBox.Items.Clear();

                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Text = Constants.SELECT_WINDOW;
                comboBoxItem.Value = -1;

                comboBox.Items.Add(comboBoxItem);

                foreach (Process process in processes)
                {
                    comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Text = process.MainWindowTitle;
                    comboBoxItem.Value = process.Id;

                    comboBox.Items.Add(comboBoxItem);

                    if (lastValue == process.MainWindowTitle)
                    {
                        lastIndex = comboBox.Items.Count - 1;
                    }
                }

                comboBox.SelectedIndex = lastIndex;
            }
        }

        private void SetEventHandler()
        {
            GroupBox[] groupBoxes = controller.GetAllGroupBox().ToArray();

            foreach (GroupBox groupBox in groupBoxes)
            {
                (Controller.GetControlOnGroupBox(groupBox, "startButton") as Button).Click += new EventHandler(events.OnStartStopClick);
                (Controller.GetControlOnGroupBox(groupBox, "renameLabel") as LinkLabel).Click += new EventHandler(events.OnRenameClick);
                (Controller.GetControlOnGroupBox(groupBox, "hotkeyComboBox") as ComboBox).SelectedIndexChanged += new EventHandler(events.OnHotkeyChanged);
            }
        }

        private void LoadComboBoxValues()
        {
            GroupBox[] groupBoxes = controller.GetAllGroupBox().ToArray();

            foreach (GroupBox groupBox in groupBoxes)
            {
                // Hotkeys
                ComboBox comboBox = Controller.GetControlOnGroupBox(groupBox, "hotkeyComboBox") as ComboBox;

                foreach (KeyValuePair<Keys, string> entry in Constants.Get().hotkeys)
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Text = entry.Value;
                    comboBoxItem.Value = entry.Key;

                    comboBox.Items.Add(comboBoxItem);
                }

                comboBox.SelectedIndex = 0;

                // F-Key
                comboBox = Controller.GetControlOnGroupBox(groupBox, "fKeyComboBox") as ComboBox;

                foreach (KeyValuePair<int, string> entry in Constants.Get().fKeys)
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Text = entry.Value;
                    comboBoxItem.Value = entry.Key;

                    comboBox.Items.Add(comboBoxItem);
                }

                comboBox.SelectedIndex = 0;

                // SkillBar
                comboBox = Controller.GetControlOnGroupBox(groupBox, "skillComboBox") as ComboBox;

                foreach (KeyValuePair<int, string> entry in Constants.Get().skillKeys)
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Text = entry.Value;
                    comboBoxItem.Value = entry.Key;

                    comboBox.Items.Add(comboBoxItem);
                }

                comboBox.SelectedIndex = 0;
            }
        }

        private void ExecuteDumpCleaner()
        {
            var insanitySettings = settings.DumpCleaner;

            if (insanitySettings.Count != 0 && (bool)Settings.GetOrDefault<string, object>(insanitySettings, "autoCleanDumpOnStartup"))
            {
                try
                {
                    dumpCleanerForm.CleanDumpInsanityFlyff(
                        settings.DumpCleaner["path"].ToString(),
                        (bool)settings.DumpCleaner["rpt"],
                        (bool)settings.DumpCleaner["dmp"],
                        (bool)settings.DumpCleaner["txt"]
                    );
                }
                catch
                { }
            }
        }

        private void LoadSettings()
        {
            if (settings.Spammer.Count == 0) return;

            GroupBox[] groupBoxes = controller.GetAllGroupBox().ToArray();

            foreach (GroupBox groupBox in groupBoxes)
            {
                int index = Controller.GetIndex(groupBox);

                if (settings.Spammer.ContainsKey(index) == false)
                {
                    settings.Spammer[index] = new Dictionary<string, object>();
                }

                groupBox.Text = Settings.GetOrDefault(settings.Spammer[index], "spammerGroupBox", groupBox.Text).ToString();

                ComboBox comboBox = Controller.GetControlOnGroupBox(groupBox, "windowComboBox") as ComboBox;

                if (settings.Spammer[index].ContainsKey("windowComboBox"))
                {
                    int counter = 0;
                    foreach (ComboBoxItem item in comboBox.Items)
                    {
                        if (item.Text == settings.Spammer[index]["windowComboBox"].ToString())
                        {
                            comboBox.SelectedIndex = counter;
                            break;
                        }

                        counter++;
                    }
                }
                else
                {
                    comboBox.SelectedIndex = 0;
                }

                comboBox = Controller.GetControlOnGroupBox(groupBox, "hotkeyComboBox") as ComboBox;
                comboBox.SelectedIndex = (int)(long)Settings.GetOrDefault(settings.Spammer[index], "hotkeyComboBox", 0);

                comboBox = Controller.GetControlOnGroupBox(groupBox, "fKeyComboBox") as ComboBox;
                comboBox.SelectedIndex = (int)(long)Settings.GetOrDefault(settings.Spammer[index], "fKeyComboBox", 0);

                comboBox = Controller.GetControlOnGroupBox(groupBox, "skillComboBox") as ComboBox;
                comboBox.SelectedIndex = (int)(long)Settings.GetOrDefault(settings.Spammer[index], "skillComboBox", 0);

                NumericUpDown intervalNumeric = Controller.GetControlOnGroupBox(groupBox, "intervalNumeric") as NumericUpDown;
                intervalNumeric.Value = (int)(double)Settings.GetOrDefault(settings.Spammer[index], "intervalNumeric", 0);
            }
        }

        private void SaveSettings()
        {
            GroupBox[] groupBoxes = controller.GetAllGroupBox().ToArray();

            foreach (GroupBox groupBox in groupBoxes)
            {
                int index = Controller.GetIndex(groupBox);

                settings.Spammer[index] = new Dictionary<string, object>();

                settings.Spammer[index]["spammerGroupBox"] = groupBox.Text;

                ComboBox comboBox = Controller.GetControlOnGroupBox(groupBox, "windowComboBox") as ComboBox;
                settings.Spammer[index]["windowComboBox"] = (comboBox.SelectedItem as ComboBoxItem).Text;

                comboBox = Controller.GetControlOnGroupBox(groupBox, "hotkeyComboBox") as ComboBox;
                settings.Spammer[index]["hotkeyComboBox"] = comboBox.SelectedIndex;

                comboBox = Controller.GetControlOnGroupBox(groupBox, "fKeyComboBox") as ComboBox;
                settings.Spammer[index]["fKeyComboBox"] = comboBox.SelectedIndex;

                comboBox = Controller.GetControlOnGroupBox(groupBox, "skillComboBox") as ComboBox;
                settings.Spammer[index]["skillComboBox"] = comboBox.SelectedIndex;

                NumericUpDown intervalNumeric = Controller.GetControlOnGroupBox(groupBox, "intervalNumeric") as NumericUpDown;
                settings.Spammer[index]["intervalNumeric"] = intervalNumeric.Value;
            }

            settings.Save();
        }

        private void ReloadSettings()
        {
            this.settings = Settings.Get();
        }

        private void Ftool_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();

            windowManagerForm.rightProcesses.ForEach(p =>
            {
                ProcessManager.ShowWindow(p.MainWindowHandle);
            });
        }

        private void Ftool_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.hotkey.Dispose();
            Environment.Exit(Environment.ExitCode);
        }

        private void Ftool_Activated(object sender, EventArgs e)
        {
            if (isLoaded == true) RefreshWindowList();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshWindowList();
        }

        private void windowManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (windowManagerForm.Visible)
            {
                windowManagerForm.Activate();
            }

            windowManagerForm.Show();
        }

        private void dumpCleanerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dumpCleanerForm.IsDisposed)
            {
                dumpCleanerForm = new DumpCleanerForm();
            }

            dumpCleanerForm.WindowClosed += ReloadSettings;

            dumpCleanerForm.ShowDialog();
        }

        private void aboutMenuStrip_Click(object sender, EventArgs e)
        {
            if (aboutForm.IsDisposed)
            {
                aboutForm = new AboutForm();
            }

            aboutForm.ShowDialog();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | 0x2000000;
                return cp;
            }
        }
    }
}
