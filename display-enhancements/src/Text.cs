using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ToyBoxChineseDisplayEnhancements {
    internal static class Text {
        internal static string Clean(string value) => Regex.Replace(value ?? "", "<[^>]*>", "").Trim();
        internal static bool Missing(string value) => string.IsNullOrWhiteSpace(value) || value == "<null>" || value.StartsWith("[unknown key: ", StringComparison.Ordinal);
        internal static string Safe(string value) => (value ?? "").Replace("<", "＜").Replace(">", "＞");
        internal static string Signed(int value) => value.ToString("+0;-0;0", CultureInfo.InvariantCulture);
        internal static string WithDetails(string summary, string details) => Main.Settings.ShowTechnicalDetails && !string.IsNullOrEmpty(details)
            ? summary + "\n〔" + details + "〕" : summary;
        internal static string Excerpt(string value) {
            if (Missing(value)) return null;
            var plain = Regex.Replace(Clean(value), @"\s+", " ");
            return plain.Length > 40 ? plain.Substring(0, 40) + "…" : plain;
        }
        internal static string Objective(string verb, string title, string quest) {
            if (Missing(title)) return null;
            return verb + "：" + Safe(Clean(title)) + (Missing(quest) ? "" : "〔" + Safe(Clean(quest)) + "〕");
        }
        internal static string State(string value, bool quest) {
            if (quest) {
                switch (value) {
                    case "None": return "未开始";
                    case "Started": return "进行中";
                    case "Completed": return "已完成";
                    case "Updated": return "已更新";
                    case "Postponed": return "已推迟";
                    case "Failed": return "已失败";
                    default: return null;
                }
            }
            switch (value) {
                case "None": return "未开启";
                case "Started": return "进行中";
                case "Completed": return "已完成";
                case "Postponed": return "已推迟";
                case "Failed": return "已失败";
                default: return null;
            }
        }
        internal static string Layer(int depth, string content) => WithDetails(
            (depth == 0 ? "结果预览" : "后续可能涉及") + "：" + content,
            "来源深度：" + depth);
        internal static string Conviction(string direction, Func<string, string> localize) {
            switch (direction) {
                case "Faith": return "教条主义";
                case "Hope": return "自由主义";
                case "Corruption": return "异端主义";
                case "Reason": return "理性";
                default: return localize(direction);
            }
        }
        internal static string Shift(string direction, int value, bool required, string description, Func<string, string> localize) {
            var name = Safe(Conviction(direction, localize));
            var result = required ? "信念要求：" + name + "点数 ≥ " + value : "信念变化：" + name + " " + Signed(value);
            return result + (Missing(description) ? "" : "（" + Safe(Clean(description)) + "）");
        }
        internal static string Check(string type, int dc, bool hidden, Func<string, string> localize) =>
            "检定：" + Safe(localize(type)) + "，难度 " + dc + (hidden ? "（隐藏检定）" : "");
    }
}
