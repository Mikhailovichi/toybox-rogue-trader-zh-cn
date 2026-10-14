using System.Collections.Generic;
using System.Linq;

namespace ToyBoxDisplayEnhancements {
    internal static class Semantic {
        private const string Actions = "Kingmaker.Designers.EventConditionActionSystem.Actions.";
        private const string Conditions = "Kingmaker.Designers.EventConditionActionSystem.Conditions.";

        internal static string Action(object action) {
            if (action == null) return null;
            string result = null;
            object blueprint = null;
            switch (action.GetType().FullName) {
                case Actions + "OpenVendorSelectingWindow": result = UiText.Get("Vendor"); break;
                case Actions + "CreateCustomCompanion": result = UiText.Get("Companion"); break;
                case Actions + "GiveObjective":
                    blueprint = Reflect.Read(action, "Objective");
                    result = Objective(blueprint, UiText.Get("GiveObjective")); break;
                case Actions + "SetObjectiveStatus":
                    var state = Reflect.String(action, "Status");
                    var verb = state == "Complete" ? UiText.Get("CompleteObjective") : state == "Fail" ? UiText.Get("FailObjective") : null;
                    if (verb == null) return null;
                    blueprint = Reflect.Read(action, "Objective");
                    result = Objective(blueprint, verb);
                    if (result != null && Reflect.Bool(action, "StartObjectiveIfNone")) result = UiText.Get("StartIfNone", result);
                    break;
                case Actions + "StartEtude":
                case Actions + "CompleteEtude":
                    if (Reflect.Bool(action, "Evaluate")) return null;
                    blueprint = Reflect.Resolve(Reflect.Read(action, "Etude"));
                    var name = StoryAliases.Name(blueprint);
                    if (Text.Missing(name)) return null;
                    var starts = action.GetType().Name == "StartEtude";
                    result = UiText.Get(starts ? "StartEtude" : "CompleteEtude", Text.Safe(name));
                    if (starts && Reflect.Bool(action, "StartImmediately")) result = UiText.Get("Immediately", result);
                    break;
            }
            return result == null ? null : Text.WithDetails(result, Details(action, blueprint));
        }
        private static string Objective(object blueprint, string verb) =>
            Text.Objective(verb, Reflect.Title(blueprint), Reflect.Title(Reflect.Read(blueprint, "Quest")));

        internal static string Checker(object checker) => ReadChecker(checker, new HashSet<object>()).Render();
        internal static string Condition(object condition) => ReadCondition(condition, new HashSet<object>()).Render();

        private static ConditionText ReadChecker(object checker, HashSet<object> visited, object owner = null) {
            if (checker == null) return Fallback(owner, UiText.Get("NullChecker"));
            if (!visited.Add(checker)) return Fallback(owner, UiText.Get("Cycle"));
            try {
                var operation = Reflect.String(checker, "Operation");
                if (operation != "And" && operation != "Or") return Fallback(owner, UiText.Get("UnknownOperation", Text.Safe(operation)));
                var conditions = Reflect.Items(Reflect.Read(checker, "Conditions")).ToArray();
                // ToyBox checks empty top-level summaries for availability; nested empty groups must remain visible.
                if (conditions.Length == 0 && owner != null) return Fallback(owner, UiText.Get("EmptyGroup"));
                return new ConditionText {
                    Operation = operation,
                    Children = conditions.Select(c => ReadCondition(c, visited)).ToList(),
                    Not = Reflect.Bool(owner, "Not"),
                    Details = owner == null ? null : Details(owner)
                };
            } finally { visited.Remove(checker); }
        }
        private static ConditionText ReadCondition(object condition, HashSet<object> visited) {
            if (condition == null) return Fallback(null, UiText.Get("NullCondition"));
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
                    var title = Text.Objective(UiText.Get(quest ? "QuestStatus" : "ObjectiveStatus"), Reflect.Title(blueprint), quest ? null : Reflect.Title(Reflect.Read(blueprint, "Quest")));
                    if (title != null) { result = UiText.Get("StateIs", title, state); negative = UiText.Get("StateIsNot", title, state); }
                }
            } else if (type == Conditions + "EtudeStatus") {
                blueprint = Reflect.Read(condition, "Etude");
                var name = StoryAliases.Name(blueprint);
                if (!Text.Missing(name)) {
                    var states = new List<string>();
                    if (Reflect.Bool(condition, "NotStarted")) states.Add(UiText.Get("EtudeNotStarted"));
                    if (Reflect.Bool(condition, "Started")) states.Add(UiText.Get("EtudeStarted"));
                    else {
                        if (Reflect.Bool(condition, "Playing")) states.Add(UiText.Get("EtudePlaying"));
                        if (Reflect.Bool(condition, "CompletionInProgress")) states.Add(UiText.Get("EtudeCompleting"));
                    }
                    if (Reflect.Bool(condition, "Completed")) states.Add(UiText.Get("Completed"));
                    var title = UiText.Get("EtudeTitle", Text.Safe(name));
                    if (states.Count == 1) {
                        result = UiText.Get("EtudeState", title, states[0]);
                        negative = UiText.Get("EtudeNotState", title, states[0]);
                        if (states[0] == UiText.Get("EtudePlaying")) { result = UiText.Get("EtudeRunning", title); negative = UiText.Get("EtudeNotRunning", title); }
                    } else if (states.Count > 1) {
                        result = UiText.Get("EtudeAnyState", title, string.Join(UiText.Get("StateSeparator"), states));
                        negative = UiText.Get("EtudeNoState", title, string.Join(UiText.Get("StateSeparator"), states));
                    }
                }
            } else if (type == Conditions + "HasEnoughMoneyForCustomCompanion") {
                result = UiText.Get("EnoughProfit");
                negative = UiText.Get("NotEnoughProfit");
            } else if (type == Conditions + "CueSeen") {
                blueprint = Reflect.Read(condition, "Cue");
                if (blueprint != null) {
                    var excerpt = Text.Excerpt(Reflect.Localized(Reflect.Read(blueprint, "Text")));
                    var identity = Text.Missing(excerpt) ? Reflect.InternalName(blueprint) : excerpt;
                    if (Text.Missing(identity)) identity = Reflect.String(blueprint, "AssetGuid");
                    if (!Text.Missing(identity)) {
                        var current = Reflect.Bool(condition, "CurrentDialog");
                        result = UiText.Get(current ? "CueSeenCurrent" : "CueSeenBefore", Text.Safe(identity));
                        negative = UiText.Get(current ? "CueNotSeenCurrent" : "CueNotSeenBefore", Text.Safe(identity));
                    }
                }
            }
            if (result == null) return Fallback(condition);
            return new ConditionText { Summary = result, NegativeSummary = negative, Not = Reflect.Bool(condition, "Not"), Details = Details(condition, blueprint) };
        }
        private static ConditionText Fallback(object condition, string reason = null) {
            // Native captions already contain negation. Never negate them a second time.
            var caption = condition == null ? "" : Reflect.Caption(condition);
            return new ConditionText { Summary = reason == null ? caption : UiText.Get("FallbackReason", caption, reason) };
        }
        private static string Details(object element, object blueprint = null) {
            if (!Main.Settings.ShowTechnicalDetails) return null;
            string caption;
            try { caption = Reflect.Caption(element); }
            catch { caption = element.GetType().Name; }
            var detail = UiText.Get("Original", Text.Safe(caption));
            if (blueprint != null) {
                detail = UiText.Get("Identity", detail, Text.Safe(Reflect.InternalName(blueprint)), Reflect.String(blueprint, "AssetGuid"));
                if (StoryAliases.Find(blueprint) != null) detail = UiText.Get("AliasSource", detail);
            }
            return detail;
        }
    }
}
