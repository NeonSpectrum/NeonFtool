using NeonFtool.Libraries;
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
        private readonly ControlCollection _controls;

        public Controller(ControlCollection controls)
        {
            _controls = controls;
        }

        /// <summary>
        /// Extracts the trailing integer from a control's Name (e.g. "spammerGroupBox3" → 3).
        /// Returns -1 if no number is found.
        /// </summary>
        public static int GetIndex(Control control)
        {
            return int.TryParse(Regex.Match(control.Name, @"\d+").Value, out int index)
                ? index
                : -1;
        }

        /// <summary>
        /// Returns all GroupBox controls across every TabPage in the first TabControl.
        /// </summary>
        public IEnumerable<GroupBox> GetAllGroupBox()
        {
            TabPage[] tabPages = _controls
                .OfType<TabControl>()
                .First()
                .Controls
                .OfType<TabPage>()
                .ToArray();

            return tabPages.SelectMany(tab => tab.Controls.OfType<GroupBox>());
        }

        /// <summary>
        /// Returns all TextBoxes named "windowTextBox..." inside every GroupBox.
        /// </summary>
        public TextBox[] GetAllWindowTextBox()
        {
            return GetAllGroupBox()
                .SelectMany(gb => gb.Controls
                    .OfType<TextBox>()
                    .Where(tb => tb.Name.StartsWith("windowTextBox")))
                .ToArray();
        }

        /// <summary>
        /// Finds the first child control of <paramref name="groupBox"/> whose name
        /// starts with <paramref name="component"/>.
        /// </summary>
        public static Control GetControlOnGroupBox(GroupBox groupBox, string component)
        {
            return groupBox.Controls
                .OfType<Control>()
                .First(c => c.Name.StartsWith(component));
        }

        /// <summary>
        /// Returns the process matching the regex in the window TextBox of a GroupBox.
        /// </summary>
        public static Process GetProcessByGroupBox(ProcessManager processManager, GroupBox groupBox)
        {
            TextBox tb = GetControlOnGroupBox(groupBox, "windowTextBox") as TextBox;
            if (string.IsNullOrEmpty(tb?.Text)) return null;

            return processManager.GetProcessByRegex(tb.Text);
        }
    }
}
