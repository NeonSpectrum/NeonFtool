using Ftool.Libraries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.Control;

namespace NeonFtool.Classes
{
    internal class Controller
    {
        private ControlCollection Controls;

        public Controller(ControlCollection controls)
        {
            Controls = controls;
        }
        public static int GetIndex(Control control)
        {
            int index;

            if (int.TryParse(Regex.Match(control.Name, @"\d+").Value, out index))
            {
                return index;
            }

            return -1;
        }

        public IEnumerable<GroupBox> GetAllGroupBox()
        {
            GroupBox[] groupBoxes = Array.Empty<GroupBox>();
            TabPage[] tabPages = Controls.OfType<TabControl>().First().Controls.OfType<TabPage>().ToArray();

            foreach (TabPage tabPage in tabPages)
            {
                groupBoxes = groupBoxes.Concat(tabPage.Controls.OfType<GroupBox>().ToArray()).ToArray();
            }

            return groupBoxes;
        }

        public ComboBox[] GetAllWindowComboBox()
        {
            List<ComboBox> list = new List<ComboBox>();

            GroupBox[] groupBoxes = GetAllGroupBox().ToArray();

            foreach (GroupBox groupBox in groupBoxes)
            {
                IEnumerable<ComboBox> comboBoxes = groupBox.Controls.OfType<ComboBox>().Where(x => x.Name.StartsWith("windowComboBox"));

                foreach (ComboBox comboBox in comboBoxes)
                {
                    list.Add(comboBox);
                }
            }

            return list.ToArray();
        }

        public GroupBox GetGroupBox(int index)
        {
            return Controls.OfType<GroupBox>().Where(x => x.Name == "spammerGroupBox" + index).First();
        }

        public static Control GetControlOnGroupBox(GroupBox groupBox, string component)
        {
            return groupBox.Controls.OfType<Control>().Where(x => x.Name.StartsWith(component)).First();
        }

        public static Process GetProcessByGroupBox(ProcessManager window, GroupBox groupBox)
        {
            ComboBoxItem windowComboBox = (Controller.GetControlOnGroupBox(groupBox, "windowComboBox") as ComboBox).SelectedItem as ComboBoxItem;
            return window.GetProcessByPID((int)windowComboBox.Value);
        }
    }
}
