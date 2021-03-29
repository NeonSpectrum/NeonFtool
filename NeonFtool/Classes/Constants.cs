using System.Collections.Generic;
using System.Windows.Forms;

namespace NeonFtool.Classes
{
    internal class Constants
    {
        public const string MAIN_TITLE = "NeonFtool 0.1";
        public const string DUMP_CLEANER_TITLE = "Dump Cleaner";
        public const string WINDOW_MANAGER_TITLE = "Window Manager";

        public const string SELECT_WINDOW = "Select Window";
        public const string BROWSE_LABEL = "Select Insanity Folder...";

        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int SW_HIDE = 0;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int GWL_STYLE = -16;
        public const int WS_VISIBLE = 0x10000000;

        public readonly IDictionary<int, string> fKeys = new Dictionary<int, string>()
        {
            {-1, ""},
            {112, "F1"},
            {113, "F2"},
            {114, "F3"},
            {115, "F4"},
            {116, "F5"},
            {117, "F6"},
            {118, "F7"},
            {119, "F8"},
            {120, "F9"},
            {121, "F10"},
        };

        public readonly IDictionary<int, string> skillKeys = new Dictionary<int, string>()
        {
            {-1, ""},
            {48, "0"},
            {49, "1"},
            {50, "2"},
            {51, "3"},
            {52, "4"},
            {53, "5"},
            {54, "6"},
            {55, "7"},
            {56, "8"},
            {57, "9"},
        };

        public readonly IDictionary<Keys, string> hotkeys = new Dictionary<Keys, string>()
        {
            {Keys.None, ""},
            {Keys.F1, "CTRL + F1"},
            {Keys.F2, "CTRL + F2"},
            {Keys.F3, "CTRL + F3"},
            {Keys.F4, "CTRL + F4"},
            {Keys.F5, "CTRL + F5"},
            {Keys.F6, "CTRL + F6"},
            {Keys.F7, "CTRL + F7"},
            {Keys.F8, "CTRL + F8"},
            {Keys.F9, "CTRL + F9"},
            {Keys.F10, "CTRL + F10"},
        };

        public static Constants Get()
        {
            return new Constants();
        }
    }
}
