using NeonFtool.Libraries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Input;

namespace NeonFtool.Classes
{
    internal class Events
    {
        private readonly ProcessManager _processManager;
        private readonly Hotkey _hotkey;
        private readonly Dictionary<int, Spam> _spamList = new();

        public Events(ProcessManager processManager, Hotkey hotkey)
        {
            _processManager = processManager;
            _hotkey = hotkey;
        }

        public void OnStartStopClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            GroupBox groupBox = (GroupBox)button.Parent;
            ComboBox windowComboBox   = (ComboBox)Controller.GetControlOnGroupBox(groupBox, "windowComboBox");
            ComboBox fKeyComboBox     = (ComboBox)Controller.GetControlOnGroupBox(groupBox, "fKeyComboBox");
            ComboBox skillComboBox    = (ComboBox)Controller.GetControlOnGroupBox(groupBox, "skillComboBox");
            NumericUpDown intervalNum = (NumericUpDown)Controller.GetControlOnGroupBox(groupBox, "intervalNumeric");

            int index = Controller.GetIndex(button);
            if (index == -1) return;

            if (button.Text == "Start")
            {
                if (windowComboBox.Text == Constants.SELECT_WINDOW)
                {
                    MessageBox.Show("Please select a window.", Constants.MAIN_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (fKeyComboBox.Text == "" && skillComboBox.Text == "")
                {
                    MessageBox.Show("Select at least one F-Key or Skill Bar.", Constants.MAIN_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Process process = _processManager.GetProcessByWindowTitle(windowComboBox.Text);
                int interval    = (int)intervalNum.Value;
                int fKey        = fKeyComboBox.Text  != "" ? (int)(fKeyComboBox.SelectedItem  as ComboBoxItem).Value : -1;
                int skillKey    = skillComboBox.Text != "" ? (int)(skillComboBox.SelectedItem as ComboBoxItem).Value : -1;

                Spam spam = new Spam(process, interval, fKey, skillKey);
                spam.Start(button);
                _spamList[index] = spam;

                windowComboBox.Enabled = false;
                intervalNum.Enabled    = false;
                fKeyComboBox.Enabled   = false;
                skillComboBox.Enabled  = false;
                button.Text = "Stop";
            }
            else
            {
                _spamList[index].Stop();
                _spamList.Remove(index);

                windowComboBox.Enabled = true;
                intervalNum.Enabled    = true;
                fKeyComboBox.Enabled   = true;
                skillComboBox.Enabled  = true;
                button.Text = "Start";
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
            ModifierKeys modifier = selected.Text.Contains("CTRL")
                ? ModifierKeys.Control
                : ModifierKeys.Alt;

            try
            {
                _hotkey.Set(index, modifier, (Keys)selected.Value, (Button)Controller.GetControlOnGroupBox(groupBox, "startButton"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Constants.MAIN_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox.SelectedIndex = 0;
            }
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
