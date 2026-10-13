using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ToyBoxChineseDisplayEnhancements {
    internal sealed class Alias {
        public string InternalName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    internal static class Aliases {
        internal static Dictionary<string, Alias> Entries = new Dictionary<string, Alias>();
        internal static void Load(string directory) {
            Entries = JsonConvert.DeserializeObject<Dictionary<string, Alias>>(File.ReadAllText(Path.Combine(directory, "feature-aliases.zh-CN.json")));
            if (Entries == null || Entries.Count != 8) throw new InvalidDataException("Expected eight reviewed aliases.");
        }
        internal static Alias Find(object blueprint) {
            var id = Reflect.String(blueprint, "AssetGuid").Replace("-", "").ToLowerInvariant();
            if (!Entries.TryGetValue(id, out var alias)) return null;
            // A renamed blueprint is not silently assigned an old description.
            return Reflect.InternalName(blueprint) == alias.InternalName ? alias : null;
        }
        internal static string OfficialName(object blueprint) {
            var value = Reflect.String(blueprint, "Name");
            return Text.Missing(value) || value == Reflect.InternalName(blueprint) ? null : value;
        }
        internal static string Title(object blueprint, Func<string, string> formatter) {
            var alias = Find(blueprint);
            if (alias == null || OfficialName(blueprint) != null) return null;
            var title = Text.Safe(alias.Name);
            if (formatter != null) title = formatter(title);
            if (Runtime.Setting("showDisplayAndInternalNames") && alias.Name != alias.InternalName)
                title += " : <color=#808080>" + Text.Safe(alias.InternalName) + "</color>";
            return title;
        }
        public static string DescriptionFor(object blueprint) {
            var original = Reflect.Read(blueprint, "Description") as string;
            if (!Runtime.Active || !Text.Missing(Text.Clean(original))) return original;
            var alias = Find(blueprint);
            return alias == null ? original : "【汉化说明】" + alias.Description;
        }
    }
}
