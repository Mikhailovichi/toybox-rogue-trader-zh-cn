using System.Collections.Generic;
using System.Linq;

namespace ToyBoxChineseDisplayEnhancements {
    internal static class Semantic {
        private const string Actions = "Kingmaker.Designers.EventConditionActionSystem.Actions.";
        private const string Conditions = "Kingmaker.Designers.EventConditionActionSystem.Conditions.";

        internal static string Action(object action) {
            if (action == null) return null;
            string result = null;
            object blueprint = null;
            switch (action.GetType().FullName) {
                case Actions + "OpenVendorSelectingWindow": result = "打开商人选择界面"; break;
                case Actions + "CreateCustomCompanion": result = "进入自定义队友创建流程"; break;
                case Actions + "GiveObjective":
                    blueprint = Reflect.Read(action, "Objective");
                    result = Objective(blueprint, "开启目标"); break;
                case Actions + "SetObjectiveStatus":
                    var state = Reflect.String(action, "Status");
                    var verb = state == "Complete" ? "完成目标" : state == "Fail" ? "目标失败" : null;
                    if (verb == null) return null;
                    blueprint = Reflect.Read(action, "Objective");
                    result = Objective(blueprint, verb);
                    if (result != null && Reflect.Bool(action, "StartObjectiveIfNone")) result += "（未开启时先开启目标）";
                    break;
                case Actions + "StartEtude":
                case Actions + "CompleteEtude":
                    if (Reflect.Bool(action, "Evaluate")) return null;
                    blueprint = Reflect.Resolve(Reflect.Read(action, "Etude"));
                    var name = StoryAliases.Name(blueprint);
                    if (Text.Missing(name)) return null;
                    var starts = action.GetType().Name == "StartEtude";
                    result = (starts ? "启动剧情状态：" : "完成剧情状态：") + Text.Safe(name) +
                        (starts && Reflect.Bool(action, "StartImmediately") ? "（立即启动）" : "");
                    break;
            }
            return result == null ? null : Text.WithDetails(result, Details(action, blueprint));
        }
        private static string Objective(object blueprint, string verb) =>
            Text.Objective(verb, Reflect.Title(blueprint), Reflect.Title(Reflect.Read(blueprint, "Quest")));

        internal static string Checker(object checker) => ReadChecker(checker, new HashSet<object>()).Render();
        internal static string Condition(object condition) => ReadCondition(condition, new HashSet<object>()).Render();

        private static ConditionText ReadChecker(object checker, HashSet<object> visited, object owner = null) {
            if (checker == null) return Fallback(owner, "条件引用为空");
            if (!visited.Add(checker)) return Fallback(owner, "循环条件引用，未展开");
            try {
                var operation = Reflect.String(checker, "Operation");
                if (operation != "And" && operation != "Or") return Fallback(owner, "未知条件组合：" + Text.Safe(operation));
                var conditions = Reflect.Items(Reflect.Read(checker, "Conditions")).ToArray();
                // ToyBox checks empty top-level summaries for availability; nested empty groups must remain visible.
                if (conditions.Length == 0 && owner != null) return Fallback(owner, "空条件组，保留原摘要");
                return new ConditionText {
                    Operation = operation,
                    Children = conditions.Select(c => ReadCondition(c, visited)).ToList(),
                    Not = Reflect.Bool(owner, "Not"),
                    Details = owner == null ? null : Details(owner)
                };
            } finally { visited.Remove(checker); }
        }
        private static ConditionText ReadCondition(object condition, HashSet<object> visited) {
            if (condition == null) return Fallback(null, "空条件引用");
            var type = condition.GetType().FullName;
            if (type == Conditions + "OrAndLogic") return ReadChecker(Reflect.Read(condition, "ConditionsChecker"), visited, condition);
            if (type == Conditions + "CheckConditionsHolder") {
                var holder = Reflect.Resolve(Reflect.Read(condition, "ConditionsHolder"));
                return ReadChecker(Reflect.Read(holder, "Conditions"), visited, condition);
            }
            string result = null, negative = null;
            object blueprint = null;
            if (type == Conditions + "ObjectiveStatus" || type == Conditions + "QuestStatus") {
                var quest = type == Conditions + "QuestStatus";
                blueprint = Reflect.Read(condition, quest ? "Quest" : "QuestObjective");
                var state = Text.State(Reflect.String(condition, "State"), quest);
                if (state != null) {
                    var title = Text.Objective(quest ? "任务状态" : "目标状态", Reflect.Title(blueprint), quest ? null : Reflect.Title(Reflect.Read(blueprint, "Quest")));
                    if (title != null) { result = title + "为“" + state + "”"; negative = title + "不是“" + state + "”"; }
                }
            } else if (type == Conditions + "EtudeStatus") {
                blueprint = Reflect.Read(condition, "Etude");
                var name = StoryAliases.Name(blueprint);
                if (!Text.Missing(name)) {
                    var states = new List<string>();
                    if (Reflect.Bool(condition, "NotStarted")) states.Add("未启动");
                    if (Reflect.Bool(condition, "Started")) states.Add("已启动（包括运行中或完成处理中）");
                    else {
                        if (Reflect.Bool(condition, "Playing")) states.Add("运行中");
                        if (Reflect.Bool(condition, "CompletionInProgress")) states.Add("完成处理中");
                    }
                    if (Reflect.Bool(condition, "Completed")) states.Add("已完成");
                    var title = "剧情状态“" + Text.Safe(name) + "”";
                    if (states.Count == 1) {
                        result = title + states[0];
                        negative = title + "不是“" + states[0] + "”状态";
                        if (states[0] == "运行中") { result = title + "正在运行"; negative = title + "未在运行"; }
                    } else if (states.Count > 1) {
                        result = title + "处于以下任一状态：" + string.Join("／", states);
                        negative = title + "不处于以下任何状态：" + string.Join("／", states);
                    }
                }
            } else if (type == Conditions + "HasEnoughMoneyForCustomCompanion") {
                result = "利润因子达到招募自定义队友所需的门槛";
                negative = "利润因子未达到招募自定义队友所需的门槛";
            } else if (type == Conditions + "CueSeen") {
                blueprint = Reflect.Read(condition, "Cue");
                if (blueprint != null) {
                    var excerpt = Text.Excerpt(Reflect.Localized(Reflect.Read(blueprint, "Text")));
                    var identity = Text.Missing(excerpt) ? Reflect.InternalName(blueprint) : excerpt;
                    if (Text.Missing(identity)) identity = Reflect.String(blueprint, "AssetGuid");
                    if (!Text.Missing(identity)) {
                        var scope = Reflect.Bool(condition, "CurrentDialog") ? "本次对话中" : "此前";
                        result = scope + "已显示过对白：“" + Text.Safe(identity) + "”";
                        negative = scope + "未显示过对白：“" + Text.Safe(identity) + "”";
                    }
                }
            }
            if (result == null) return Fallback(condition);
            return new ConditionText { Summary = result, NegativeSummary = negative, Not = Reflect.Bool(condition, "Not"), Details = Details(condition, blueprint) };
        }
        private static ConditionText Fallback(object condition, string reason = null) {
            // Native captions already contain negation. Never negate them a second time.
            var caption = condition == null ? "" : Reflect.Caption(condition);
            return new ConditionText { Summary = caption + (reason == null ? "" : "〔" + reason + "〕") };
        }
        private static string Details(object element, object blueprint = null) {
            if (!Main.Settings.ShowTechnicalDetails) return null;
            string caption;
            try { caption = Reflect.Caption(element); }
            catch { caption = element.GetType().Name; }
            var detail = "原始：" + Text.Safe(caption);
            if (blueprint != null) {
                detail += "；内部名：" + Text.Safe(Reflect.InternalName(blueprint)) + "；GUID：" + Reflect.String(blueprint, "AssetGuid");
                if (StoryAliases.Name(blueprint) != Reflect.InternalName(blueprint)) detail += "；中文名为本包别名";
            }
            return detail;
        }
    }
}
