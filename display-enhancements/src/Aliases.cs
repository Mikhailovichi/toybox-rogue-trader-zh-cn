using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ToyBoxDisplayEnhancements {
    internal sealed class Alias {
        public string InternalName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    internal static class Aliases {
        private static Dictionary<string, Alias> chinese = new Dictionary<string, Alias>(), english = new Dictionary<string, Alias>();
        internal static Dictionary<string, Alias> Entries => UiText.Chinese ? chinese : english;
        internal static void Load(string directory) {
            chinese = Read(directory, "feature-aliases.zh-CN.json");
            english = Read(directory, "feature-aliases.en.json");
            Validate(chinese, english, 8);
        }
        internal static Dictionary<string, Alias> Read(string directory, string file) =>
            JsonConvert.DeserializeObject<Dictionary<string, Alias>>(File.ReadAllText(Path.Combine(directory, file)))
                ?? throw new InvalidDataException("Missing alias data: " + file);
        internal static void Validate(Dictionary<string, Alias> zh, Dictionary<string, Alias> en, int count) {
            if (zh.Count != count || en.Count != count || !zh.Keys.OrderBy(k => k).SequenceEqual(en.Keys.OrderBy(k => k)) ||
                zh.Any(pair => pair.Value == null || en[pair.Key] == null || Text.Missing(pair.Value.InternalName) ||
                    pair.Value.InternalName != en[pair.Key].InternalName || Text.Missing(pair.Value.Name) || Text.Missing(en[pair.Key].Name)))
                throw new InvalidDataException("Expected matching reviewed Chinese and English aliases.");
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
        internal static IEnumerable<string> SearchNames(object blueprint) {
            if (Find(blueprint) == null) return Enumerable.Empty<string>();
            var id = Reflect.String(blueprint, "AssetGuid").Replace("-", "").ToLowerInvariant();
            return new[] { chinese[id].Name, english[id].Name, chinese[id].InternalName };
        }
        internal static string Title(object blueprint, Func<string, string> formatter) {
            var alias = Find(blueprint);
            if (alias == null || OfficialName(blueprint) != null) return null;
            var title = Text.Safe(alias.Name);
            if (formatter != null) title = formatter(title);
            if (Runtime.Setting("ToggleBPsShowDisplayAndInternalName") && alias.Name != alias.InternalName)
                title += " : <color=#808080>" + Text.Safe(alias.InternalName) + "</color>";
            return title;
        }
        internal static string DescriptionFor(object blueprint, string original) {
            if (!Runtime.Active || !Text.Missing(Text.Clean(original))) return original;
            var alias = Find(blueprint);
            return alias == null ? original : UiText.Get("AddonNote", alias.Description);
        }
    }
}
