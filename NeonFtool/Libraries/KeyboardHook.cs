using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace NeonFtool.Libraries
{
    /// <summary>
    /// Provides system-wide global hotkey registration via WinAPI.
    /// </summary>
    public sealed class KeyboardHook : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Internal native window used to receive WM_HOTKEY messages.
        /// </summary>
        private sealed class HotkeyWindow : NativeWindow, IDisposable
        {
            private const int WM_HOTKEY = 0x0312;

            public event EventHandler<KeyPressedEventArgs> KeyPressed;

            public HotkeyWindow()
            {
                CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == WM_HOTKEY)
                {
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);
                    KeyPressed?.Invoke(this, new KeyPressedEventArgs(modifier, key));
                }
            }

            public void Dispose() => DestroyHandle();
        }

        private readonly HotkeyWindow _window = new HotkeyWindow();
        private int _currentId;

        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        public KeyboardHook()
        {
            _window.KeyPressed += (sender, args) => KeyPressed?.Invoke(this, args);
        }

        /// <summary>
        /// Registers a global hotkey. Returns the registration ID.
        /// </summary>
        public int Register(ModifierKeys modifier, Keys key)
        {
            _currentId++;

            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key))
                throw new InvalidOperationException("Couldn't register the hot key. It may already be in use.");

            return _currentId;
        }

        /// <summary>
        /// Unregisters a previously registered hotkey by ID.
        /// </summary>
        public void Unregister(int id)
        {
            UnregisterHotKey(_window.Handle, id);
        }

        public void Dispose()
        {
            for (int i = _currentId; i > 0; i--)
                UnregisterHotKey(_window.Handle, i);

            _window.Dispose();
        }
    }

    /// <summary>
    /// Event arguments raised when a registered global hotkey is pressed.
    /// </summary>
    public class KeyPressedEventArgs : EventArgs
    {
        public ModifierKeys Modifier { get; }
        public Keys Key { get; }

        internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }
    }
}
