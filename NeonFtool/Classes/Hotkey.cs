using NeonFtool.Libraries;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;

namespace NeonFtool.Classes
{
    internal class Hotkey
    {
        private readonly KeyboardHook _hook;

        // Maps spammer index → registration info
        private readonly Dictionary<int, HotkeyRegistration> _registrations = new();

        public Hotkey()
        {
            _hook = new KeyboardHook();
            _hook.KeyPressed += OnHotkeyPressed;
        }

        /// <summary>
        /// Registers (or replaces) the hotkey for the given spammer index.
        /// Pass <see cref="Keys.None"/> to clear the hotkey.
        /// </summary>
        public void Set(int index, ModifierKeys modifier, Keys key, Button button)
        {
            // Always remove any existing registration for this slot first.
            Remove(index);

            if (key == Keys.None)
                return;

            int registrationId = _hook.Register(modifier, key);
            _registrations[index] = new HotkeyRegistration(registrationId, modifier, key, button);
        }

        /// <summary>
        /// Unregisters the hotkey for the given spammer index if one exists.
        /// </summary>
        public void Remove(int index)
        {
            if (!_registrations.TryGetValue(index, out HotkeyRegistration reg))
                return;

            _hook.Unregister(reg.Id);
            _registrations.Remove(index);
        }

        public void Dispose()
        {
            _hook.Dispose();
        }

        private void OnHotkeyPressed(object sender, KeyPressedEventArgs e)
        {
            foreach (HotkeyRegistration reg in _registrations.Values)
            {
                if (e.Key == reg.Key && e.Modifier == reg.Modifier)
                {
                    reg.Button.Invoke((MethodInvoker)reg.Button.PerformClick);
                    break;
                }
            }
        }

        /// <summary>Immutable record holding a single hotkey registration.</summary>
        private sealed class HotkeyRegistration
        {
            public int Id { get; }
            public ModifierKeys Modifier { get; }
            public Keys Key { get; }
            public Button Button { get; }

            public HotkeyRegistration(int id, ModifierKeys modifier, Keys key, Button button)
            {
                Id = id;
                Modifier = modifier;
                Key = key;
                Button = button;
            }
        }
    }
}
