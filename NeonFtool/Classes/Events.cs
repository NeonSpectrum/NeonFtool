using NeonFtool.Libraries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using NeonFtool.Forms;

namespace NeonFtool.Classes
{
    internal class Events
    {
        private readonly ProcessManager _processManager;
        private readonly Hotkey _hotkey;
        private readonly Controller _controller;
        private readonly Dictionary<int, Spam> _spamList = new();
        private readonly Dictionary<IntPtr, OverlayForm> _overlays = new();

        public Events(ProcessManager processManager, Hotkey hotkey, Controller controller)
        {
            _processManager = processManager;
            _hotkey = hotkey;
            _controller = controller;
        }

        public void OnStartStopClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            GroupBox groupBox = (GroupBox)button.Parent;
            TextBox windowTextBox     = (TextBox)Controller.GetControlOnGroupBox(groupBox, "windowTextBox");
            ComboBox fKeyComboBox     = (ComboBox)Controller.GetControlOnGroupBox(groupBox, "fKeyComboBox");
            ComboBox skillComboBox    = (ComboBox)Controller.GetControlOnGroupBox(groupBox, "skillComboBox");
            NumericUpDown intervalNum = (NumericUpDown)Controller.GetControlOnGroupBox(groupBox, "intervalNumeric");

            int index = Controller.GetIndex(button);
            if (index == -1) return;

            if (button.Text == "Start")
            {
                // Stop other spammers that don't allow parallel execution
                foreach (GroupBox otherGb in _controller.GetAllGroupBox())
                {
                    int otherIndex = Controller.GetIndex(otherGb);
                    if (otherIndex == index) continue; // Skip ourselves

                    if (_spamList.TryGetValue(otherIndex, out Spam otherSpam))
                    {
                        CheckBox parallelCb = (CheckBox)Controller.GetControlOnGroupBox(otherGb, "parallelCheckbox");
                        if (!parallelCb.Checked)
                        {
                            // Trigger stop for the other spammer by clicking its button
                            Button otherStartButton = (Button)Controller.GetControlOnGroupBox(otherGb, "startButton");
                            OnStartStopClick(otherStartButton, EventArgs.Empty);
                        }
                    }
                }

                if (string.IsNullOrEmpty(windowTextBox.Text))
                {
                    MessageBox.Show("Please input a window title or regex.", Constants.MAIN_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (fKeyComboBox.Text == "" && skillComboBox.Text == "")
                {
                    MessageBox.Show("Select at least one F-Key or Skill Bar.", Constants.MAIN_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Process process = _processManager.GetProcessByRegex(windowTextBox.Text);
                if (process == null)
                {
                    MessageBox.Show("No window found matching: " + windowTextBox.Text, Constants.MAIN_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int interval    = (int)intervalNum.Value;
                int fKey        = fKeyComboBox.Text  != "" ? (int)(fKeyComboBox.SelectedItem  as ComboBoxItem).Value : -1;
                int skillKey    = skillComboBox.Text != "" ? (int)(skillComboBox.SelectedItem as ComboBoxItem).Value : -1;

                Spam spam = new Spam(process, interval, fKey, skillKey);
                spam.Start(button);
                _spamList[index] = spam;

                windowTextBox.Enabled  = false;
                intervalNum.Enabled    = false;
                fKeyComboBox.Enabled   = false;
                skillComboBox.Enabled  = false;
                button.Text = "Stop";

                UpdateOverlay(process.MainWindowHandle);
            }
            else
            {
                if (_spamList.TryGetValue(index, out Spam spamToStop))
                {
                    spamToStop.Stop();
                    _spamList.Remove(index);
                }

                windowTextBox.Enabled  = true;
                intervalNum.Enabled    = true;
                fKeyComboBox.Enabled   = true;
                skillComboBox.Enabled  = true;
                button.Text = "Start";

                Process process = _processManager.GetProcessByRegex(windowTextBox.Text);
                if (process != null) UpdateOverlay(process.MainWindowHandle);
            }
        }

        public void StopAllSpammers()
        {
            foreach (var index in _spamList.Keys.ToList())
            {
                GroupBox groupBox = _controller.GetAllGroupBox().FirstOrDefault(g => Controller.GetIndex(g) == index);
                if (groupBox != null)
                {
                    Button startButton = (Button)Controller.GetControlOnGroupBox(groupBox, "startButton");
                    if (startButton != null && startButton.Text == "Stop")
                    {
                        if (startButton.InvokeRequired)
                            startButton.Invoke((MethodInvoker)(() => OnStartStopClick(startButton, EventArgs.Empty)));
                        else
                            OnStartStopClick(startButton, EventArgs.Empty);
                    }
                }
            }
        }

        private void UpdateOverlay(IntPtr handle)
        {
            // Find all active spammers for this window handle
            var activeIndices = _spamList.Keys.Where(idx => {
                GroupBox gb = _controller.GetAllGroupBox().FirstOrDefault(g => Controller.GetIndex(g) == idx);
                if (gb == null) return false;
                TextBox winTb = (TextBox)Controller.GetControlOnGroupBox(gb, "windowTextBox");
                Process p = _processManager.GetProcessByRegex(winTb.Text);
                return p != null && p.MainWindowHandle == handle;
            }).ToList();

            if (activeIndices.Any())
            {
                string combinedNames = string.Join(", ", activeIndices.Select(idx => {
                    GroupBox gb = _controller.GetAllGroupBox().FirstOrDefault(g => Controller.GetIndex(g) == idx);
                    return gb?.Text ?? $"Spammer {idx}";
                }));

                if (!_overlays.ContainsKey(handle))
                {
                    OverlayForm overlay = new OverlayForm(handle, combinedNames);
                    overlay.Show();
                    _overlays[handle] = overlay;
                }
                else
                {
                    _overlays[handle].UpdateName(combinedNames);
                }
            }
            else
            {
                if (_overlays.TryGetValue(handle, out OverlayForm overlay))
                {
                    overlay.Close();
                    overlay.Dispose();
                    _overlays.Remove(handle);
                }
            }
        }

        public void SetOverlayLock(bool locked)
        {
            foreach (var overlay in _overlays.Values)
            {
                overlay.SetLock(locked);
            }
        }

        public void OnRenameClick(object sender, EventArgs e)
        {
            LinkLabel label  = (LinkLabel)sender;
            GroupBox groupBox = (GroupBox)label.Parent;
            int index = Controller.GetIndex(label);

            string current = groupBox.Text;
            string result  = ShowInputDialog($"Rename Spammer {index}", Constants.MAIN_TITLE, current);

            if (result != null)
                groupBox.Text = result;
        }

        public void OnHotkeyChanged(object sender, EventArgs e)
        {
            ComboBox comboBox       = (ComboBox)sender;
            GroupBox groupBox       = (GroupBox)comboBox.Parent;
            ComboBoxItem selected   = comboBox.SelectedItem as ComboBoxItem;

            if (selected == null) return;

            int index = Controller.GetIndex(comboBox);

            // Determine modifier from the display label stored in ComboBoxItem.Text
            ModifierKeys modifier = ModifierKeys.None;
            if (selected.Text.Contains("CTRL"))
                modifier = ModifierKeys.Control;
            else if (selected.Text.Contains("ALT"))
                modifier = ModifierKeys.Alt;

            try
            {
                _hotkey.Set(index, modifier, (Keys)selected.Value, (Button)Controller.GetControlOnGroupBox(groupBox, "startButton"), groupBox);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Constants.MAIN_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox.SelectedIndex = 0;
            }
        }

        public void Dispose()
        {
            foreach (var overlay in _overlays.Values)
            {
                if (!overlay.IsDisposed)
                {
                    overlay.Close();
                    overlay.Dispose();
                }
            }
            _overlays.Clear();
        }

        /// <summary>
        /// Shows a simple WinForms input dialog. Returns the entered string, or null if cancelled.
        /// Replaces the old Microsoft.VisualBasic.Interaction.InputBox dependency.
        /// </summary>
        private static string ShowInputDialog(string prompt, string title, string defaultValue = "")
        {
            Form form   = new Form { Text = title, Width = 340, Height = 140, FormBorderStyle = FormBorderStyle.FixedDialog,
                                     StartPosition = FormStartPosition.CenterParent, MinimizeBox = false, MaximizeBox = false };
            Label label = new Label  { Left = 10, Top = 12,  Width = 300, Text = prompt };
            TextBox tb  = new TextBox { Left = 10, Top = 35,  Width = 300, Text = defaultValue };
            Button ok   = new Button  { Left = 145, Top = 65, Width = 75, Text = "OK",     DialogResult = DialogResult.OK };
            Button cancel = new Button { Left = 225, Top = 65, Width = 75, Text = "Cancel", DialogResult = DialogResult.Cancel };

            form.Controls.AddRange(new Control[] { label, tb, ok, cancel });
            form.AcceptButton = ok;
            form.CancelButton = cancel;

            return form.ShowDialog() == DialogResult.OK ? tb.Text : null;
        }
    }
}
