using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ToyBoxChineseDisplayEnhancements {
    internal static class StoryAliases {
        private static Dictionary<string, Alias> entries = new Dictionary<string, Alias>();
        internal static void Load(string directory) {
            entries = JsonConvert.DeserializeObject<Dictionary<string, Alias>>(File.ReadAllText(Path.Combine(directory, "story-aliases.zh-CN.json")))
                ?? throw new InvalidDataException("Story aliases are missing.");
        }
        internal static string Name(object blueprint) {
            var guid = Reflect.String(blueprint, "AssetGuid").Replace("-", "").ToLowerInvariant();
            if (entries.TryGetValue(guid, out var alias) && alias.InternalName == Reflect.InternalName(blueprint)) return alias.Name;
            return Reflect.InternalName(blueprint);
        }
    }
}
