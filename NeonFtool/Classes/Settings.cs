using nucs.JsonSettings;
using System.Collections.Generic;

namespace NeonFtool.Classes
{
    internal class Settings : JsonSettings
    {
        public override string FileName { get; set; } = "settings.json";

        public string[] TargetProcess { get; set; } = { "Client.exe", "Neuz.exe" };

        public Dictionary<int, Dictionary<string, object>> Spammer { get; set; } = new();

        public Dictionary<string, object> WindowManager { get; set; } = new();
        public bool LockOverlay { get; set; } = true;
        public bool StopOnKeyPress { get; set; } = false;
        public int OverlayOffsetX { get; set; } = -1; // -1 means use default
        public int OverlayOffsetY { get; set; } = -1;

        public static Settings Get()
        {
            var appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            var appDir  = System.IO.Path.Combine(appData, "NeonFtool");
            
            // Ensure the directory exists
            if (!System.IO.Directory.Exists(appDir))
                System.IO.Directory.CreateDirectory(appDir);

            var path = System.IO.Path.Combine(appDir, "settings.json");
            return JsonSettings.Load<Settings>(path);
        }

        /// <summary>
        /// Returns <paramref name="dictionary"/>[<paramref name="key"/>] if it exists,
        /// otherwise <paramref name="defaultValue"/>.
        /// </summary>
        public static object GetOrDefault<TKey, TValue>(
            IDictionary<TKey, TValue> dictionary,
            TKey key,
            object defaultValue = null)
        {
            if (dictionary == null) return defaultValue;
            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
        }
    }
}
