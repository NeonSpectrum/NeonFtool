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
        private readonly WindowManagerForm _windowManagerForm;
        private AboutForm _aboutForm;

        private bool _isLoaded;

        public NeonFtool()
        {
            _processManager   = new ProcessManager();
            _hotkey           = new Hotkey(_processManager);
            _settings         = Settings.Get();
            _windowManagerForm = new WindowManagerForm();
            _aboutForm        = new AboutForm();

            // Controller needs the Controls collection, so must come after InitializeComponent.
            InitializeComponent();

            _controller = new Controller(Controls);
            _events     = new Events(_processManager, _hotkey, _controller);

            _windowManagerForm.WindowClosed += ReloadSettings;

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

            _isLoaded = true;
        }

        // ──────────────────────────────────────────────────────────
        //  Window list
        // ──────────────────────────────────────────────────────────

        private void RefreshWindowList()
        {
            _processManager.RefreshProcessList();
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

                // Window TextBox
                TextBox windowTb = (TextBox)Controller.GetControlOnGroupBox(groupBox, "windowTextBox");
                if (slot.TryGetValue("windowTextBox", out object savedWindow))
                {
                    windowTb.Text = savedWindow.ToString();
                }
                else if (slot.TryGetValue("windowComboBox", out object legacyWindow))
                {
                    // Migration: try to use the old selection as a starting regex/title
                    string savedText = legacyWindow.ToString();
                    if (savedText != Constants.SELECT_WINDOW)
                    {
                        // Strip PID tag if it exists to make it a cleaner regex/match
                        windowTb.Text = savedText.Split(new[] { " PID ->" }, StringSplitOptions.None)[0];
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

                ((CheckBox)Controller.GetControlOnGroupBox(groupBox, "parallelCheckbox")).Checked
                    = (bool)Settings.GetOrDefault(slot, "parallelCheckbox", false);
            }

            lockOverlayToolStripMenuItem.Checked = _settings.LockOverlay;
        }

        private void SaveSettings()
        {
            ReloadSettings(); // Load latest offsets/settings from disk before saving UI changes
            foreach (GroupBox groupBox in _controller.GetAllGroupBox())
            {
                int index = Controller.GetIndex(groupBox);
                Dictionary<string, object> slot = new();

                slot["spammerGroupBox"] = groupBox.Text;
                slot["windowTextBox"]   = ((TextBox)Controller.GetControlOnGroupBox(groupBox, "windowTextBox")).Text;
                slot["hotkeyComboBox"]  = ((ComboBox)Controller.GetControlOnGroupBox(groupBox, "hotkeyComboBox")).SelectedIndex;
                slot["fKeyComboBox"]    = ((ComboBox)Controller.GetControlOnGroupBox(groupBox, "fKeyComboBox")).SelectedIndex;
                slot["skillComboBox"]   = ((ComboBox)Controller.GetControlOnGroupBox(groupBox, "skillComboBox")).SelectedIndex;
                slot["intervalNumeric"] = (double)((NumericUpDown)Controller.GetControlOnGroupBox(groupBox, "intervalNumeric")).Value;
                slot["parallelCheckbox"] = ((CheckBox)Controller.GetControlOnGroupBox(groupBox, "parallelCheckbox")).Checked;

                _settings.Spammer[index] = slot;
            }

            _settings.LockOverlay = lockOverlayToolStripMenuItem.Checked;
            _settings.Save();
        }

        private void ReloadSettings()
        {
            _settings = Settings.Get();
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

            _events.Dispose();

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

        private void aboutMenuStrip_Click(object sender, EventArgs e)
        {
            if (_aboutForm.IsDisposed)
                _aboutForm = new AboutForm();

            _aboutForm.ShowDialog();
        }

        private void lockOverlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lockOverlayToolStripMenuItem.Checked = !lockOverlayToolStripMenuItem.Checked;
            ReloadSettings();
            _settings.LockOverlay = lockOverlayToolStripMenuItem.Checked;
            _events.SetOverlayLock(_settings.LockOverlay);
            _settings.Save();
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
