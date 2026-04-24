using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace NeonFtool.Libraries
{
    /// <summary>
    /// Provides system-wide keyboard interception via Low-Level Keyboard Hook (WH_KEYBOARD_LL).
    /// This allows keys to be intercepted and optionally blocked from reaching other applications.
    /// </summary>
    public sealed class KeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private readonly LowLevelKeyboardProc _proc;
        private IntPtr _hookId = IntPtr.Zero;
        private int _currentId = 0;

        // Internal list of "registered" keys to monitor
        private readonly Dictionary<int, (ModifierKeys Modifier, Keys Key)> _monitoredKeys = new();

        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        public KeyboardHook()
        {
            _proc = HookCallback;
            _hookId = SetHook(_proc);
        }

        public int Register(ModifierKeys modifier, Keys key)
        {
            _currentId++;
            _monitoredKeys[_currentId] = (modifier, key);
            return _currentId;
        }

        public void Unregister(int id)
        {
            _monitoredKeys.Remove(id);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;
                ModifierKeys modifier = GetCurrentModifierKeys();

                foreach (var kvp in _monitoredKeys)
                {
                    if (kvp.Value.Key == key && kvp.Value.Modifier == modifier)
                    {
                        var args = new KeyPressedEventArgs(modifier, key);
                        KeyPressed?.Invoke(this, args);

                        // If the event handler marked it as handled, block the key from the system
                        if (args.Handled)
                        {
                            return (IntPtr)1;
                        }
                    }
                }
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private ModifierKeys GetCurrentModifierKeys()
        {
            ModifierKeys modifier = ModifierKeys.None;
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control) modifier |= ModifierKeys.Control;
            if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt) modifier |= ModifierKeys.Alt;
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) modifier |= ModifierKeys.Shift;
            return modifier;
        }

        public void Dispose()
        {
            if (_hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
        }
    }

    public class KeyPressedEventArgs : EventArgs
    {
        public ModifierKeys Modifier { get; }
        public Keys Key { get; }
        
        /// <summary>
        /// Set to true to prevent the key from being processed by the system/active window.
        /// </summary>
        public bool Handled { get; set; }

        internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }
    }
}
