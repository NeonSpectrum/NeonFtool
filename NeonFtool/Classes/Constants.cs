using System.Collections.Generic;
using System.Windows.Forms;

namespace NeonFtool.Classes
{
    internal static class Constants
    {
        public const string MAIN_TITLE         = "NeonFtool 1.0";
        public const string DUMP_CLEANER_TITLE = "Dump Cleaner";
        public const string WINDOW_MANAGER_TITLE = "Window Manager";

        public const string SELECT_WINDOW = "Select Window";
        public const string BROWSE_LABEL  = "Select Insanity Folder...";

        // Win32 message/style constants
        public const int WM_KEYDOWN       = 0x100;
        public const int WM_KEYUP         = 0x101;
        public const int SW_HIDE          = 0;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int GWL_STYLE        = -16;
        public const int WS_VISIBLE       = 0x10000000;

        // F1–F10 virtual key codes
        public static readonly IReadOnlyDictionary<int, string> FKeys =
            new Dictionary<int, string>
            {
                { -1,  "" },
                { 112, "F1"  },
                { 113, "F2"  },
                { 114, "F3"  },
                { 115, "F4"  },
                { 116, "F5"  },
                { 117, "F6"  },
                { 118, "F7"  },
                { 119, "F8"  },
                { 120, "F9"  },
                { 121, "F10" },
            };

        // Skill-bar number keys (0–9)
        public static readonly IReadOnlyDictionary<int, string> SkillKeys =
            new Dictionary<int, string>
            {
                { -1, "" },
                { 48, "0" },
                { 49, "1" },
                { 50, "2" },
                { 51, "3" },
                { 52, "4" },
                { 53, "5" },
                { 54, "6" },
                { 55, "7" },
                { 56, "8" },
                { 57, "9" },
            };

        // Global hotkey combinations — uses a List to avoid duplicate-key exceptions.
        // Each entry is (ModifierLabel, WinForms Keys value, display label).
        public static readonly IReadOnlyList<(string Modifier, Keys Key, string Label)> Hotkeys =
            new List<(string, Keys, string)>
            {
                ("",     Keys.None, ""),
                ("CTRL", Keys.F1,  "CTRL + F1"),
                ("CTRL", Keys.F2,  "CTRL + F2"),
                ("CTRL", Keys.F3,  "CTRL + F3"),
                ("CTRL", Keys.F4,  "CTRL + F4"),
                ("CTRL", Keys.F5,  "CTRL + F5"),
                ("CTRL", Keys.F6,  "CTRL + F6"),
                ("CTRL", Keys.F7,  "CTRL + F7"),
                ("CTRL", Keys.F8,  "CTRL + F8"),
                ("CTRL", Keys.F9,  "CTRL + F9"),
                ("CTRL", Keys.F10, "CTRL + F10"),
                ("ALT",  Keys.F1,  "ALT + F1"),
                ("ALT",  Keys.F2,  "ALT + F2"),
                ("ALT",  Keys.F3,  "ALT + F3"),
                ("ALT",  Keys.F4,  "ALT + F4"),
                ("ALT",  Keys.F5,  "ALT + F5"),
                ("ALT",  Keys.F6,  "ALT + F6"),
                ("ALT",  Keys.F7,  "ALT + F7"),
                ("ALT",  Keys.F8,  "ALT + F8"),
                ("ALT",  Keys.F9,  "ALT + F9"),
                ("ALT",  Keys.F10, "ALT + F10"),
            };
    }
}
