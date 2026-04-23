using NeonFtool.Classes;
using NeonFtool.Forms;
using NeonFtool.Libraries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace NeonFtool
{
    public partial class NeonFtool : Form
    {
        private readonly ProcessManager _processManager;
        private readonly Controller _controller;
        private readonly Events _events;
        private readonly Hotkey _hotkey;
        private Settings _settings;
        private readonly DumpCleanerForm _dumpCleanerForm;
        private readonly WindowManagerForm _windowManagerForm;
        private AboutForm _aboutForm;

        private bool _isLoaded;

        public NeonFtool()
        {
            _processManager   = new ProcessManager();
            _hotkey           = new Hotkey();
            _settings         = Settings.Get();
            _dumpCleanerForm  = new DumpCleanerForm();
            _windowManagerForm = new WindowManagerForm();
            _aboutForm        = new AboutForm();

            // Controller needs the Controls collection, so must come after InitializeComponent.
            InitializeComponent();

            _controller = new Controller(Controls);
            _events     = new Events(_processManager, _hotkey);

            _windowManagerForm.WindowClosed += ReloadSettings;
            _dumpCleanerForm.WindowClosed   += ReloadSettings;

            ExecuteStartup();
        }

        private void Ftool_Load(object sender, EventArgs e)
        {
            Text = Constants.MAIN_TITLE;
        }

        private void ExecuteStartup()
        {
            RefreshWindowList();
            LoadComboBoxValues();
            SetEventHandlers();
            LoadSettings();
            ExecuteDumpCleaner();

            _isLoaded = true;
        }

        // ──────────────────────────────────────────────────────────
        //  Window list
        // ──────────────────────────────────────────────────────────

        private void RefreshWindowList()
        {
            Process[] processes = _processManager.RefreshProcessList().GetProcessList();

            foreach (ComboBox comboBox in _controller.GetAllWindowComboBox())
            {
                // Remember previously selected title so we can restore it.
                string lastTitle = (comboBox.SelectedItem as ComboBoxItem)?.Text ?? "";

                comboBox.Items.Clear();
                comboBox.Items.Add(new ComboBoxItem { Text = Constants.SELECT_WINDOW, Value = -1 });

                int restoreIndex = 0;

                foreach (Process process in processes)
                {
                    comboBox.Items.Add(new ComboBoxItem { Text = process.MainWindowTitle, Value = process.Id });

                    if (process.MainWindowTitle == lastTitle)
                        restoreIndex = comboBox.Items.Count - 1;
                }

                comboBox.SelectedIndex = restoreIndex;
            }
        }

        // ──────────────────────────────────────────────────────────
        //  ComboBox population
        // ──────────────────────────────────────────────────────────

        private void LoadComboBoxValues()
        {
            foreach (GroupBox groupBox in _controller.GetAllGroupBox())
            {
                PopulateHotkeyComboBox(groupBox);
                PopulateKeyComboBox(groupBox, "fKeyComboBox",  Constants.FKeys);
                PopulateKeyComboBox(groupBox, "skillComboBox", Constants.SkillKeys);
            }
        }

        private static void PopulateHotkeyComboBox(GroupBox groupBox)
        {
            ComboBox comboBox = (ComboBox)Controller.GetControlOnGroupBox(groupBox, "hotkeyComboBox");

            foreach ((_, Keys key, string label) in Constants.Hotkeys)
            {
                comboBox.Items.Add(new ComboBoxItem { Text = label, Value = key });
            }

            comboBox.SelectedIndex = 0;
        }

        private static void PopulateKeyComboBox(GroupBox groupBox, string controlName,
                                                 IReadOnlyDictionary<int, string> source)
        {
            ComboBox comboBox = (ComboBox)Controller.GetControlOnGroupBox(groupBox, controlName);

            foreach (KeyValuePair<int, string> entry in source)
            {
                comboBox.Items.Add(new ComboBoxItem { Text = entry.Value, Value = entry.Key });
            }

            comboBox.SelectedIndex = 0;
        }

        // ──────────────────────────────────────────────────────────
        //  Event wiring
        // ──────────────────────────────────────────────────────────

        private void SetEventHandlers()
        {
            foreach (GroupBox groupBox in _controller.GetAllGroupBox())
            {
                ((Button)Controller.GetControlOnGroupBox(groupBox, "startButton")).Click
                    += _events.OnStartStopClick;

                ((LinkLabel)Controller.GetControlOnGroupBox(groupBox, "renameLabel")).Click
                    += _events.OnRenameClick;

                ((ComboBox)Controller.GetControlOnGroupBox(groupBox, "hotkeyComboBox")).SelectedIndexChanged
                    += _events.OnHotkeyChanged;
            }
        }

        // ──────────────────────────────────────────────────────────
        //  Settings persistence
        // ──────────────────────────────────────────────────────────

        private void LoadSettings()
        {
            if (_settings.Spammer.Count == 0) return;

            foreach (GroupBox groupBox in _controller.GetAllGroupBox())
            {
                int index = Controller.GetIndex(groupBox);

                if (!_settings.Spammer.ContainsKey(index))
                    _settings.Spammer[index] = new Dictionary<string, object>();

                Dictionary<string, object> slot = _settings.Spammer[index];

                groupBox.Text = Settings.GetOrDefault(slot, "spammerGroupBox", groupBox.Text).ToString();

                // Window ComboBox
                ComboBox windowCb = (ComboBox)Controller.GetControlOnGroupBox(groupBox, "windowComboBox");
                if (slot.TryGetValue("windowComboBox", out object savedWindow))
                {
                    for (int i = 0; i < windowCb.Items.Count; i++)
                    {
                        if (((ComboBoxItem)windowCb.Items[i]).Text == savedWindow.ToString())
                        {
                            windowCb.SelectedIndex = i;
                            break;
                        }
                    }
                }

                ((ComboBox)Controller.GetControlOnGroupBox(groupBox, "hotkeyComboBox")).SelectedIndex
                    = (int)(long)Settings.GetOrDefault(slot, "hotkeyComboBox", 0L);

                ((ComboBox)Controller.GetControlOnGroupBox(groupBox, "fKeyComboBox")).SelectedIndex
                    = (int)(long)Settings.GetOrDefault(slot, "fKeyComboBox", 0L);

                ((ComboBox)Controller.GetControlOnGroupBox(groupBox, "skillComboBox")).SelectedIndex
                    = (int)(long)Settings.GetOrDefault(slot, "skillComboBox", 0L);

                ((NumericUpDown)Controller.GetControlOnGroupBox(groupBox, "intervalNumeric")).Value
                    = (decimal)(double)Settings.GetOrDefault(slot, "intervalNumeric", 50.0);
            }
        }

        private void SaveSettings()
        {
            foreach (GroupBox groupBox in _controller.GetAllGroupBox())
            {
                int index = Controller.GetIndex(groupBox);
                Dictionary<string, object> slot = new();

                slot["spammerGroupBox"] = groupBox.Text;
                slot["windowComboBox"]  = ((ComboBox)Controller.GetControlOnGroupBox(groupBox, "windowComboBox")).SelectedItem is ComboBoxItem wi ? wi.Text : "";
                slot["hotkeyComboBox"]  = ((ComboBox)Controller.GetControlOnGroupBox(groupBox, "hotkeyComboBox")).SelectedIndex;
                slot["fKeyComboBox"]    = ((ComboBox)Controller.GetControlOnGroupBox(groupBox, "fKeyComboBox")).SelectedIndex;
                slot["skillComboBox"]   = ((ComboBox)Controller.GetControlOnGroupBox(groupBox, "skillComboBox")).SelectedIndex;
                slot["intervalNumeric"] = (double)((NumericUpDown)Controller.GetControlOnGroupBox(groupBox, "intervalNumeric")).Value;

                _settings.Spammer[index] = slot;
            }

            _settings.Save();
        }

        private void ReloadSettings()
        {
            _settings = Settings.Get();
        }

        // ──────────────────────────────────────────────────────────
        //  Dump cleaner
        // ──────────────────────────────────────────────────────────

        private void ExecuteDumpCleaner()
        {
            var dc = _settings.DumpCleaner;

            if (dc.Count == 0) return;
            if (Settings.GetOrDefault(dc, "autoCleanDumpOnStartup", false) is not true) return;

            try
            {
                _dumpCleanerForm.CleanDumpInsanityFlyff(
                    dc["path"].ToString(),
                    (bool)dc["rpt"],
                    (bool)dc["dmp"],
                    (bool)dc["txt"]
                );
            }
            catch { /* Silently ignore startup clean failures */ }
        }

        // ──────────────────────────────────────────────────────────
        //  Form events
        // ──────────────────────────────────────────────────────────

        private void Ftool_Activated(object sender, EventArgs e)
        {
            if (_isLoaded) RefreshWindowList();
        }

        private void Ftool_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();

            // Restore any hidden windows before exit.
            _windowManagerForm.RightProcesses.ForEach(p => ProcessManager.ShowWindow(p.MainWindowHandle));
        }

        private void Ftool_FormClosed(object sender, FormClosedEventArgs e)
        {
            _hotkey.Dispose();
            Environment.Exit(Environment.ExitCode);
        }

        // ──────────────────────────────────────────────────────────
        //  Menu strip handlers
        // ──────────────────────────────────────────────────────────

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e) =>
            RefreshWindowList();

        private void windowManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_windowManagerForm.Visible)
                _windowManagerForm.Activate();
            else
                _windowManagerForm.Show();
        }

        private void dumpCleanerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_dumpCleanerForm.IsDisposed) return; // already disposed — shouldn't happen with shared instance

            _dumpCleanerForm.ShowDialog();
        }

        private void aboutMenuStrip_Click(object sender, EventArgs e)
        {
            if (_aboutForm.IsDisposed)
                _aboutForm = new AboutForm();

            _aboutForm.ShowDialog();
        }

        // ──────────────────────────────────────────────────────────
        //  Composited window (eliminates flicker)
        // ──────────────────────────────────────────────────────────

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                return cp;
            }
        }
    }
}
