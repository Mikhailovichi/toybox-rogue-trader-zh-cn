using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ToyBoxDisplayEnhancements {
    internal static class Text {
        internal static string Clean(string value) => Regex.Replace(value ?? "", "<[^>]*>", "").Trim();
        internal static bool Missing(string value) => string.IsNullOrWhiteSpace(value) || value == "<null>" || value.StartsWith("[unknown key: ", StringComparison.Ordinal);
        internal static string Safe(string value) => (value ?? "").Replace("<", "＜").Replace(">", "＞");
        internal static string Signed(int value) => value.ToString("+0;-0;0", CultureInfo.InvariantCulture);
        internal static string WithDetails(string summary, string details) => Main.Settings.ShowTechnicalDetails && !string.IsNullOrEmpty(details)
            ? UiText.Get("Details", summary, details) : summary;
        internal static string Excerpt(string value) {
            if (Missing(value)) return null;
            var plain = Regex.Replace(Clean(value), @"\s+", " ");
            var limit = UiText.Chinese ? 40 : 120;
            if (plain.Length <= limit) return plain;
            var excerpt = plain.Substring(0, limit);
            if (!UiText.Chinese && !char.IsWhiteSpace(plain[limit]) && !char.IsWhiteSpace(excerpt[limit - 1])) {
                var boundary = excerpt.LastIndexOf(' ');
                if (boundary > 0) excerpt = excerpt.Substring(0, boundary);
            }
            return excerpt.TrimEnd() + (UiText.Chinese ? "…" : "...");
        }
        internal static string Objective(string verb, string title, string quest) {
            if (Missing(title)) return null;
            return Missing(quest) ? UiText.Get("Objective", verb, Safe(Clean(title))) : UiText.Get("ObjectiveQuest", verb, Safe(Clean(title)), Safe(Clean(quest)));
        }
        internal static string State(string value, bool quest) {
            switch (value) {
                case "None": return UiText.Get(quest ? "QuestNone" : "ObjectiveNone");
                case "Started": case "Completed": case "Postponed": case "Failed": return UiText.Get(value);
                case "Updated": return quest ? UiText.Get(value) : null;
                default: return null;
            }
        }
        internal static string Layer(int depth, string content) => WithDetails(
            UiText.Get(depth == 0 ? "Result" : "LaterResult", content), UiText.Get("Depth", depth));
        internal static string Conviction(string direction, Func<string, string> localize) {
            var official = localize(direction);
            if (!Missing(official) && official != direction) return official;
            var culture = Runtime.GameCulture;
            var chinese = culture == "zhCN";
            if (!chinese && culture != "enGB") return direction;
            switch (direction) {
                case "Faith": return chinese ? "教条主义" : "Dogmatic";
                case "Hope": return chinese ? "自由主义" : "Iconoclast";
                case "Corruption": return chinese ? "异端主义" : "Heretical";
                case "Reason": return chinese ? "理性" : direction;
                default: return direction;
            }
        }
        internal static string Shift(string direction, int value, bool required, string description, Func<string, string> localize) {
            var name = Safe(Conviction(direction, localize));
            var result = required ? UiText.Get("Required", name, value) : UiText.Get("Shift", name, Signed(value));
            return Missing(description) ? result : UiText.Get("Description", result, Safe(Clean(description)));
        }
        internal static string Check(string type, int dc, bool hidden, Func<string, string> localize) =>
            UiText.Get(hidden ? "HiddenCheck" : "Check", Safe(localize(type)), dc);
    }
}
