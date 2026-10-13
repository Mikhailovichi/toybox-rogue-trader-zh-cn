using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

// A managed fixture for patch installation, not a replacement game or distributable mod.
namespace Kingmaker.Blueprints {
    public class SimpleBlueprint { public string name; public string AssetGuid; }
    public class BlueprintScriptableObject : SimpleBlueprint { }
}
namespace Kingmaker.ElementsSystem {
    public class GameAction { public int Executions; public virtual string GetCaption() => "Unknown action"; public void RunAction() { Executions++; } }
    public class Condition { public bool Not; public int Checks; public virtual string GetCaption() => Not ? "Not (Unknown condition)" : "Unknown condition"; public virtual bool Check() { Checks++; return true; } }
    public class ConditionsChecker {
        public string Operation = "And";
        public Condition[] Conditions = Array.Empty<Condition>();
        public int Checks;
        public bool TestResult = true;
        public bool Check() { Checks++; return TestResult; }
    }
    public class ActionList { public GameAction[] Actions = Array.Empty<GameAction>(); }
    public class Ref { public object Value; public object Get() => Value; }
}
namespace Kingmaker.Blueprints.Quests {
    public class BlueprintQuest : Kingmaker.Blueprints.SimpleBlueprint { public string Title; }
    public class BlueprintQuestObjective : Kingmaker.Blueprints.SimpleBlueprint { public string Title; public BlueprintQuest Quest; }
}
namespace Kingmaker.Designers.EventConditionActionSystem.Actions {
    public class GiveObjective : Kingmaker.ElementsSystem.GameAction { public Kingmaker.Blueprints.Quests.BlueprintQuestObjective Objective; }
    public class SetObjectiveStatus : GiveObjective { public string Status; public bool StartObjectiveIfNone; }
    public class StartEtude : Kingmaker.ElementsSystem.GameAction { public Kingmaker.ElementsSystem.Ref Etude; public bool Evaluate; public bool StartImmediately; }
    public class CompleteEtude : StartEtude { }
    public class OpenVendorSelectingWindow : Kingmaker.ElementsSystem.GameAction { public override string GetCaption() => "Open vendor selecting window"; }
    public class CreateCustomCompanion : Kingmaker.ElementsSystem.GameAction { public override string GetCaption() => "Create custom companion"; }
    public class Conditional : Kingmaker.ElementsSystem.GameAction {
        public Kingmaker.ElementsSystem.ConditionsChecker ConditionsChecker = new Kingmaker.ElementsSystem.ConditionsChecker();
        public Kingmaker.ElementsSystem.ActionList IfTrue = new Kingmaker.ElementsSystem.ActionList(), IfFalse = new Kingmaker.ElementsSystem.ActionList();
    }
}
namespace Kingmaker.Designers.EventConditionActionSystem.Conditions {
    public class QuestStatus : Kingmaker.ElementsSystem.Condition { public Kingmaker.Blueprints.Quests.BlueprintQuest Quest; public string State; }
    public class ObjectiveStatus : Kingmaker.ElementsSystem.Condition { public Kingmaker.Blueprints.Quests.BlueprintQuestObjective QuestObjective; public string State; }
    public class EtudeStatus : Kingmaker.ElementsSystem.Condition { public Kingmaker.Blueprints.SimpleBlueprint Etude; public bool NotStarted, Started, Playing, CompletionInProgress, Completed; }
    public class CheckConditionsHolder : Kingmaker.ElementsSystem.Condition { public Kingmaker.ElementsSystem.Ref ConditionsHolder; }
    public class Holder { public Kingmaker.ElementsSystem.ConditionsChecker Conditions; }
    public class OrAndLogic : Kingmaker.ElementsSystem.Condition {
        public Kingmaker.ElementsSystem.ConditionsChecker ConditionsChecker = new Kingmaker.ElementsSystem.ConditionsChecker();
        public override string GetCaption() => (Not ? "Not " : "") + ConditionsChecker.Operation + "[" + ConditionsChecker.Conditions.Length + "]";
    }
    public class HasEnoughMoneyForCustomCompanion : Kingmaker.ElementsSystem.Condition { public override string GetCaption() => (Not ? "Not " : "") + "Has enough money for custom companion"; }
    public class CueSeen : Kingmaker.ElementsSystem.Condition {
        public Kingmaker.DialogSystem.Blueprints.BlueprintCue Cue;
        public bool CurrentDialog;
        public override string GetCaption() => (Not ? "Not " : "") + "Cue Seen (" + Cue?.name + ")";
    }
}
namespace Kingmaker.UnitLogic.Alignments { public class SoulMarkShift { public string Direction; public int Value; public string Description; public bool CheckByValue = true; } }
namespace Kingmaker.UI.Models.Tooltip.Base { public interface IUIDataProvider { string Name { get; } string Description { get; } } }
namespace Kingmaker.UnitLogic.Progression.Features {
    public class BlueprintFeature : Kingmaker.Blueprints.BlueprintScriptableObject, Kingmaker.UI.Models.Tooltip.Base.IUIDataProvider {
        public string OfficialName, OfficialDescription;
        public string Name => OfficialName;
        public string Description => OfficialDescription;
    }
}
namespace Kingmaker.UnitLogic { public class Feature { } }
namespace Kingmaker.DialogSystem.Blueprints {
    public class ShowCheck { public string Type = "SkillPersuasion"; public int DC = 10; }
    public class BlueprintAnswer {
        public bool HasShowCheck;
        public ShowCheck ShowCheck = new ShowCheck();
        public Kingmaker.ElementsSystem.ConditionsChecker ShowConditions = new Kingmaker.ElementsSystem.ConditionsChecker();
        public Kingmaker.ElementsSystem.ConditionsChecker SelectConditions = new Kingmaker.ElementsSystem.ConditionsChecker();
        public Kingmaker.ElementsSystem.GameAction[] Actions = Array.Empty<Kingmaker.ElementsSystem.GameAction>();
        public Kingmaker.UnitLogic.Alignments.SoulMarkShift Shift;
    }
    public class BlueprintCue : Kingmaker.Blueprints.SimpleBlueprint { public string Text; }
}
namespace ModKit {
    public class KitSettings { public string uiCultureCode = "zh-CN"; }
    public static class Mod { public static KitSettings ModKitSettings = new KitSettings(); }
    public static class LocalizationManager {
        public static string localize(string key) {
            if (key == "Faith") return "教条主义";
            if (key == "SkillPersuasion") return "劝说";
            return key;
        }
    }
}
namespace ToyBox {
    public class Settings { public bool showDisplayAndInternalNames, searchDescriptions; }
    public static class Main { public static bool Enabled = true; public static Settings Settings = new Settings(); }
    public static class PreviewUtilities {
        [MethodImpl(MethodImplOptions.NoInlining)] public static List<string> FormatActionAsList(Kingmaker.ElementsSystem.GameAction action) {
            if (action is Kingmaker.Designers.EventConditionActionSystem.Actions.Conditional conditional) {
                var chosen = conditional.ConditionsChecker.Check() ? conditional.IfTrue : conditional.IfFalse;
                return chosen.Actions.SelectMany(FormatActionAsList).ToList();
            }
            return new List<string> { action?.GetCaption() ?? "" };
        }
        [MethodImpl(MethodImplOptions.NoInlining)] public static string FormatConditions(Kingmaker.ElementsSystem.Condition[] conditions) => string.Join(", ", conditions.Select(c => c.GetCaption()));
        [MethodImpl(MethodImplOptions.NoInlining)] public static string FormatConditions(Kingmaker.ElementsSystem.ConditionsChecker checker) => FormatConditions(checker.Conditions);
        [MethodImpl(MethodImplOptions.NoInlining)] public static List<string> FormatConditionsAsList(Kingmaker.DialogSystem.Blueprints.BlueprintAnswer answer) => new List<string> { "Native conditions" };
    }
    public static class PreviewManager {
        public static int Traversals;
        [MethodImpl(MethodImplOptions.NoInlining)] public static string FormatShift(Kingmaker.UnitLogic.Alignments.SoulMarkShift shift, string format) => shift == null || shift.Value == 0 ? null : string.Format(format, shift.Direction + ", " + shift.Value);
        [MethodImpl(MethodImplOptions.NoInlining)] public static string ResultsText(Kingmaker.DialogSystem.Blueprints.BlueprintAnswer answer) {
            Traversals++;
            var line = answer.Actions.SelectMany(PreviewUtilities.FormatActionAsList).ToList();
            var shift = FormatShift(answer.Shift, "SoulMarkShift({0})");
            if (shift != null) line.Add(shift);
            line.Add(string.Format("Check({0}, DC {1}, hidden {2})", "SkillPersuasion", 10, true));
            return "<size=75%>[" + string.Format("{0}: {1}", 4, string.Join(", ", line)) + "]</size>";
        }
        [MethodImpl(MethodImplOptions.NoInlining)] public static string ResultsText(Kingmaker.DialogSystem.Blueprints.BlueprintCue cue) => string.Format(" \n<size=75%>[SoulMarkShift {0} by {1} - {2}]", "Faith", 3, "说明");
    }
    public static class BlueprintExtensions {
        [MethodImpl(MethodImplOptions.NoInlining)] public static string GetTitle(Kingmaker.Blueprints.SimpleBlueprint blueprint, Func<string, string> formatter = null) => blueprint.name;
        [MethodImpl(MethodImplOptions.NoInlining)] public static string GetSearchKey(Kingmaker.Blueprints.SimpleBlueprint blueprint, bool forceDisplayInternalName = false) => blueprint.name + " " + blueprint.AssetGuid;
        [MethodImpl(MethodImplOptions.NoInlining)] public static string GetSortKey(Kingmaker.Blueprints.SimpleBlueprint blueprint) => blueprint.name;
    }
    public static class FactsEditor {
        public static string LastDescription;
        [MethodImpl(MethodImplOptions.NoInlining)] public static void BlueprintRowGUI<Item, Definition>(object browser, Item feature, Definition blueprint, object character, List<Action> todo)
            where Definition : Kingmaker.Blueprints.BlueprintScriptableObject, Kingmaker.UI.Models.Tooltip.Base.IUIDataProvider {
            LastDescription = blueprint.Description;
        }
    }
}
