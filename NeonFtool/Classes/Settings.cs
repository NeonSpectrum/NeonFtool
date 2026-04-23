using nucs.JsonSettings;
using System.Collections.Generic;

namespace NeonFtool.Classes
{
    internal class Settings : JsonSettings
    {
        public override string FileName { get; set; } = "settings.json";

        public string[] TargetProcess { get; set; } = { "Client.exe", "Neuz.exe" };

        public Dictionary<int, Dictionary<string, object>> Spammer { get; set; } = new();

        public Dictionary<string, object> DumpCleaner { get; set; } = new();

        public Dictionary<string, object> WindowManager { get; set; } = new();

        public static Settings Get() => JsonSettings.Load<Settings>();

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
