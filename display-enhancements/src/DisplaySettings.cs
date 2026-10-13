using System;
using System.IO;
using Newtonsoft.Json;

namespace ToyBoxChineseDisplayEnhancements {
    internal sealed class DisplaySettings {
        public bool ShowTechnicalDetails { get; set; }
        internal static DisplaySettings Load(string directory) {
            var path = Path.Combine(directory, "display-settings.json");
            try {
                return File.Exists(path) ? JsonConvert.DeserializeObject<DisplaySettings>(File.ReadAllText(path)) ?? new DisplaySettings() : new DisplaySettings();
            } catch (Exception ex) { Main.Warn(ex); return new DisplaySettings(); }
        }
        internal void Save(string directory) => File.WriteAllText(Path.Combine(directory, "display-settings.json"), JsonConvert.SerializeObject(this, Formatting.Indented));
    }
}
