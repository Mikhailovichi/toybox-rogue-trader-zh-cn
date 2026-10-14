using System.Collections.Generic;

namespace ToyBoxDisplayEnhancements {
    internal static class StoryAliases {
        private static Dictionary<string, Alias> chinese = new Dictionary<string, Alias>(), english = new Dictionary<string, Alias>();
        private static Dictionary<string, Alias> Entries => UiText.Chinese ? chinese : english;
        internal static void Load(string directory) {
            chinese = Aliases.Read(directory, "story-aliases.zh-CN.json");
            english = Aliases.Read(directory, "story-aliases.en.json");
            Aliases.Validate(chinese, english, 1);
        }
        internal static Alias Find(object blueprint) {
            var guid = Reflect.String(blueprint, "AssetGuid").Replace("-", "").ToLowerInvariant();
            return Entries.TryGetValue(guid, out var alias) && alias.InternalName == Reflect.InternalName(blueprint) ? alias : null;
        }
        internal static string Name(object blueprint) => Find(blueprint)?.Name ?? Reflect.InternalName(blueprint);
    }
}
