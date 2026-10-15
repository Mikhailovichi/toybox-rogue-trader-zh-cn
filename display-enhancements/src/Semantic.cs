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
            object[] identities = null;
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
                case Actions + "MarkAnswersSelected":
                case Actions + "UnmarkAnswersSelected":
                    identities = Reflect.Items(Reflect.Read(action, "Answers")).Select(Reflect.Resolve).ToArray();
                    if (identities.Length == 0 || identities.Any(a => Text.Missing(DialogueText.Identity(a)))) return null;
                    var answers = string.Join(UiText.Get("StateSeparator"), identities.Select(a => UiText.Get("Quoted", Text.Safe(DialogueText.Identity(a)))));
                    result = UiText.Get(action.GetType().Name == "MarkAnswersSelected" ? "MarkAnswers" : "UnmarkAnswers", answers);
                    break;
                case Actions + "StartDialog":
                    blueprint = Reflect.Read(action, "Dialogue");
                    var dialog = blueprint == null ? Expression(Reflect.Read(action, "DialogEvaluator")) ?? Expression(Reflect.Read(action, "DialogueOwner"))
                        : DialogueText.Identity(blueprint);
                    if (!Text.Missing(dialog)) result = UiText.Get("StartDialog", Text.Safe(dialog));
                    break;
                case Actions + "UnlockFlag":
                    blueprint = Reflect.Read(action, "flag");
                    if (!Text.Missing(Reflect.BlueprintName(blueprint)))
                        result = UiText.Get("UnlockFlag", Text.Safe(Reflect.BlueprintName(blueprint)), Reflect.Int(action, "flagValue"));
                    break;
                case Actions + "LockFlag":
                case Actions + "IncrementFlagValue":
                    blueprint = Reflect.Read(action, "Flag");
                    var flag = Reflect.BlueprintName(blueprint);
                    if (Text.Missing(flag)) return null;
                    if (action.GetType().Name == "LockFlag") result = UiText.Get("LockFlag", Text.Safe(flag));
                    else {
                        var value = Reflect.Read(action, "Value");
                        if (value == null) return null;
                        var constant = value.GetType().FullName == "Kingmaker.Designers.EventConditionActionSystem.Evaluators.IntConstant";
                        var change = constant ? Text.Signed(Reflect.Int(value, "Value")) : Expression(value);
                        if (Text.Missing(change)) return null;
                        result = UiText.Get(constant ? "IncrementFlag" : "IncrementFlagExpression", Text.Safe(flag), Text.Safe(change));
                        result = UiText.Get(Reflect.Bool(action, "UnlockIfNot") ? "UnlockBeforeIncrement" : "OnlyUnlockedIncrement", result);
                    }
                    break;
            }
            return result == null ? null : Text.WithDetails(result, Details(action, identities ?? new[] { blueprint }));
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
            } else if (type == Conditions + "AnswerSelected" || type == Conditions + "AnswerListShown") {
                var selected = type == Conditions + "AnswerSelected";
                blueprint = Reflect.Read(condition, selected ? "Answer" : "AnswersList");
                var identity = DialogueText.Identity(blueprint);
                if (!Text.Missing(identity)) {
                    var current = Reflect.Bool(condition, "CurrentDialog");
                    var key = selected ? "AnswerSelected" : "AnswerListShown";
                    result = UiText.Get(key + (current ? "Current" : "Before"), Text.Safe(identity));
                    negative = UiText.Get(key + (current ? "NotCurrent" : "NotBefore"), Text.Safe(identity));
                }
            } else if (type == Conditions + "CueSeen") {
                blueprint = Reflect.Read(condition, "Cue");
                if (blueprint != null) {
                    var identity = DialogueText.Identity(blueprint);
                    if (!Text.Missing(identity)) {
                        var current = Reflect.Bool(condition, "CurrentDialog");
                        result = UiText.Get(current ? "CueSeenCurrent" : "CueSeenBefore", Text.Safe(identity));
                        negative = UiText.Get(current ? "CueNotSeenCurrent" : "CueNotSeenBefore", Text.Safe(identity));
                    }
                }
            } else if (type == Conditions + "CheckPassed" || type == Conditions + "CheckFailed") {
                blueprint = Reflect.Read(condition, "CheckBlueprint");
                var identity = DialogueText.Identity(blueprint);
                if (!Text.Missing(identity)) {
                    var passed = type == Conditions + "CheckPassed";
                    result = UiText.Get(passed ? "CheckPassedRecord" : "CheckFailedRecord", Text.Safe(identity));
                    negative = UiText.Get(passed ? "NoCheckPassedRecord" : "NoCheckFailedRecord", Text.Safe(identity));
                }
            } else if (type == Conditions + "CompanionInParty") {
                blueprint = Reflect.Read(condition, "companion");
                var name = Reflect.BlueprintName(blueprint);
                if (!Text.Missing(name)) {
                    var excluded = new List<string>();
                    foreach (var state in new[] { "Active", "Remote", "Detached", "Ex" })
                        if (!Reflect.Bool(condition, "MatchWhen" + state)) excluded.Add(UiText.Get("CompanionState" + state));
                    result = UiText.Get("CompanionExists", Text.Safe(name));
                    if (excluded.Count > 0) result = UiText.Get("CompanionExclusions", result, string.Join(UiText.Get("StateSeparator"), excluded));
                    result = UiText.Get("Description", result, UiText.Get(Reflect.Bool(condition, "IncludeDead") ? "IncludingDead" : "ExcludingDead"));
                }
            } else if (type == Conditions + "ItemsEnough") {
                blueprint = Reflect.Read(condition, "ItemToCheck");
                var money = Reflect.Bool(condition, "Money") || Reflect.Items(Reflect.Read(blueprint, "ComponentsArray"))
                    .Any(c => c?.GetType().FullName == "Kingmaker.Blueprints.Items.Components.MoneyReplacement");
                var name = Reflect.BlueprintName(blueprint);
                if (money) { result = UiText.Get("MoneyEnough", Reflect.Int(condition, "Quantity")); negative = UiText.Get("MoneyNotEnough", Reflect.Int(condition, "Quantity")); }
                else if (!Text.Missing(name)) {
                    result = UiText.Get("ItemsEnough", Text.Safe(name), Reflect.Int(condition, "Quantity"));
                    negative = UiText.Get("ItemsNotEnough", Text.Safe(name), Reflect.Int(condition, "Quantity"));
                }
            } else if (type == Conditions + "HasFact" || type == Conditions + "HasBuff") {
                var buff = type == Conditions + "HasBuff";
                blueprint = Reflect.Read(condition, buff ? "Buff" : "Fact");
                var target = TargetName(Reflect.Read(condition, buff ? "Target" : "Unit"));
                var name = Reflect.BlueprintName(blueprint);
                if (!Text.Missing(target) && !Text.Missing(name)) {
                    result = UiText.Get(buff ? "HasBuff" : "HasFact", Text.Safe(target), Text.Safe(name));
                    negative = UiText.Get(buff ? "HasNoBuff" : "HasNoFact", Text.Safe(target), Text.Safe(name));
                }
            } else if (type == Conditions + "FlagUnlocked" || type == Conditions + "FlagInRange") {
                var range = type == Conditions + "FlagInRange";
                blueprint = Reflect.Read(condition, range ? "Flag" : "ConditionFlag");
                var name = Reflect.BlueprintName(blueprint);
                if (!Text.Missing(name)) {
                    result = UiText.Get("FlagUnlocked", Text.Safe(name));
                    if (range) result = UiText.Get("FlagRange", result, Reflect.Int(condition, "MinValue"), Reflect.Int(condition, "MaxValue"));
                    else {
                        var values = Reflect.Items(Reflect.Read(condition, "SpecifiedValues")).Select(v => v.ToString()).ToArray();
                        if (values.Length > 0) result = UiText.Get(Reflect.Bool(condition, "ExceptSpecifiedValues") ? "FlagExceptValues" : "FlagValues",
                            result, string.Join(", ", values));
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
        private static string Expression(object evaluator) => evaluator == null ? null : Reflect.Caption(evaluator);
        private static string TargetName(object evaluator) {
            if (evaluator?.GetType().FullName == "Kingmaker.Designers.EventConditionActionSystem.Evaluators.PlayerCharacter")
                return UiText.Get("PlayerCharacter");
            if (evaluator?.GetType().FullName == "Kingmaker.Designers.EventConditionActionSystem.Evaluators.CompanionInParty") {
                var name = Reflect.BlueprintName(Reflect.Read(evaluator, "Companion"));
                if (!Text.Missing(name)) return UiText.Get("CompanionTarget", name, Expression(evaluator));
            }
            return Expression(evaluator);
        }
        private static string Details(object element, params object[] blueprints) {
            if (!Main.Settings.ShowTechnicalDetails) return null;
            string caption;
            try { caption = Reflect.Caption(element); }
            catch { caption = element.GetType().Name; }
            var detail = UiText.Get("Original", Text.Safe(caption));
            foreach (var blueprint in blueprints.Where(b => b != null).Distinct()) {
                detail = UiText.Get("Identity", detail, Text.Safe(Reflect.InternalName(blueprint)), Reflect.String(blueprint, "AssetGuid"));
                if (StoryAliases.Find(blueprint) != null) detail = UiText.Get("AliasSource", detail);
            }
            return detail;
        }
    }
}
