using Ftool.Libraries;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace NeonFtool.Classes
{
    internal class Events
    {
        private ProcessManager processManager;
        private Hotkey hotkey;
        private IDictionary<int, Spam> spamList = new Dictionary<int, Spam>();

        public Events(ProcessManager processManager, Hotkey hotkey)
        {
            this.processManager = processManager;
            this.hotkey = hotkey;
        }

        public void OnStartStopClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            GroupBox groupBox = button.Parent as GroupBox;
            ComboBox windowComboBox = Controller.GetControlOnGroupBox(groupBox, "windowComboBox") as ComboBox;
            NumericUpDown intervalNumeric = Controller.GetControlOnGroupBox(groupBox, "intervalNumeric") as NumericUpDown;
            ComboBox fKeyComboBox = Controller.GetControlOnGroupBox(groupBox, "fKeyComboBox") as ComboBox;
            ComboBox skillComboBox = Controller.GetControlOnGroupBox(groupBox, "skillComboBox") as ComboBox;

            int index = Controller.GetIndex(button);
            int interval = 0;
            int fKey = -1;
            int skillKey = -1;

            if (index == -1)
            {
                return;
            }

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

                Process process = processManager.GetProcessByWindowTitle(windowComboBox.Text);

                if (intervalNumeric.Text != "")
                {
                    int.TryParse(intervalNumeric.Text, out interval);
                }

                if (fKeyComboBox.Text != "")
                {
                    fKey = (int)(fKeyComboBox.SelectedItem as ComboBoxItem).Value;
                }

                if (skillComboBox.Text != "")
                {
                    skillKey = (int)(skillComboBox.SelectedItem as ComboBoxItem).Value;
                }

                Spam spam = new Spam(process, interval, fKey, skillKey);
                spam.Start(button);

                spamList.Add(index, spam);

                windowComboBox.Enabled = false;
                intervalNumeric.Enabled = false;
                fKeyComboBox.Enabled = false;
                skillComboBox.Enabled = false;
                button.Text = "Stop";
            }
            else
            {
                spamList[index].Stop();
                spamList.Remove(index);

                windowComboBox.Enabled = true;
                intervalNumeric.Enabled = true;
                fKeyComboBox.Enabled = true;
                skillComboBox.Enabled = true;
                button.Text = "Start";
            }
        }

        public void OnRenameClick(object sender, EventArgs e)
        {
            LinkLabel label = sender as LinkLabel;
            GroupBox groupBox = label.Parent as GroupBox;
            int index = Controller.GetIndex(label);

            groupBox.Text = Interaction.InputBox("Rename Spammer " + index, Constants.MAIN_TITLE, groupBox.Text);
        }

        public void OnHotkeyChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            GroupBox groupBox = comboBox.Parent as GroupBox;
            ComboBoxItem comboBoxItem = comboBox.SelectedItem as ComboBoxItem;

            if (comboBoxItem == null)
            {
                return;
            }

            int index = Controller.GetIndex(comboBox);

            try
            {
                hotkey.Set(index, (Keys)comboBoxItem.Value, Controller.GetControlOnGroupBox(groupBox, "startButton") as Button);
            }
            catch
            {
                MessageBox.Show("Hotkey already registered.", Constants.MAIN_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox.SelectedIndex = 0;
            }
        }
    }
}
