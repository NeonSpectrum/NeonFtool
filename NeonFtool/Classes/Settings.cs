using nucs.JsonSettings;
using System.Collections.Generic;

namespace NeonFtool.Classes
{
    internal class Settings : JsonSettings
    {
        public override string FileName { get; set; } = "settings.json";

        public string[] TargetProcess { get; set; } = new string[] { "Client.exe", "Neuz.exe" };

        public IDictionary<int, IDictionary<string, object>> Spammer = new Dictionary<int, IDictionary<string, object>>();

        public IDictionary<string, object> DumpCleaner = new Dictionary<string, object>();

        public IDictionary<string, object> WindowManager = new Dictionary<string, object>();

        public static Settings Get()
        {
            return JsonSettings.Load<Settings>();
        }

        public static object GetOrDefault<T, U>(IDictionary<T, U> dictionary, object key, object defaultValue = null)
        {
            if (dictionary == null) return defaultValue;

            return dictionary.ContainsKey((T)key) ? dictionary[(T)key] : defaultValue;
        }
    }
}
