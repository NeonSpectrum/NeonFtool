using NeonFtool.Libraries;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;

namespace NeonFtool.Classes
{
    internal class Hotkey
    {
        private KeyboardHook hook;
        private IDictionary<int, IDictionary<string, object>> hotkeyList = new Dictionary<int, IDictionary<string, object>>();

        public Hotkey()
        {
            hook = new KeyboardHook();
            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(HotkeyPressed);
        }

        public void Set(int index, Keys key, Button button)
        {
            if (key == Keys.None)
            {
                Remove(index);
                return;
            }

            hotkeyList[index] = new Dictionary<string, object>();
            hotkeyList[index]["index"] = hook.Register(ModifierKeys.Control, key);
            hotkeyList[index]["key"] = key;
            hotkeyList[index]["button"] = button;
        }

        public void Remove(int index)
        {
            if (!hotkeyList.ContainsKey(index)) return;

            try
            {
                this.hook.Unregister((int)hotkeyList[index]["index"]);
                hotkeyList.Remove(index);
            }
            catch (KeyNotFoundException)
            { }
        }

        public void Dispose()
        {
            this.hook.Dispose();
        }

        private void HotkeyPressed(object sender, KeyPressedEventArgs e)
        {
            foreach (KeyValuePair<int, IDictionary<string, object>> entry in hotkeyList)
            {
                Keys key = (Keys)entry.Value["key"];
                Button button = entry.Value["button"] as Button;

                if (e.Key == key)
                {
                    button.Invoke((MethodInvoker)delegate ()
                    {
                        button.PerformClick();
                    });

                    break;
                }
            }
        }
    }
}
