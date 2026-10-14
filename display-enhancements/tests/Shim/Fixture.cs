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
    public class BlueprintQuest : Kingmaker.Blueprints.SimpleBlueprint {
        private string title;
        public string TitleEn, TitleZh;
        public string Title { get => FixtureText.Resolve(title, TitleZh, TitleEn); set => title = value; }
    }
    public class BlueprintQuestObjective : Kingmaker.Blueprints.SimpleBlueprint {
        private string title;
        public string TitleEn, TitleZh;
        public string Title { get => FixtureText.Resolve(title, TitleZh, TitleEn); set => title = value; }
        public BlueprintQuest Quest;
    }
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
namespace Kingmaker.UnitLogic.Alignments { public enum SoulMarkDirection { None, Faith, Hope, Corruption, Reason, Unknown } public class SoulMarkShift { public SoulMarkDirection Direction; public int Value; public string Description; public bool CheckByValue = true; } }
namespace Kingmaker.UI.Models.Tooltip.Base { public interface IUIDataProvider { string Name { get; } string Description { get; } } }
namespace Kingmaker.UnitLogic.Progression.Features {
    public class BlueprintFeature : Kingmaker.Blueprints.BlueprintScriptableObject, Kingmaker.UI.Models.Tooltip.Base.IUIDataProvider {
        public string OfficialName, OfficialDescription, NameEn, NameZh, DescriptionEn, DescriptionZh;
        public string Name => FixtureText.Resolve(OfficialName, NameZh, NameEn);
        public string Description => FixtureText.Resolve(OfficialDescription, DescriptionZh, DescriptionEn);
    }
}
namespace Kingmaker.UnitLogic { public class Feature { } }
namespace Kingmaker.DialogSystem.Blueprints {
    public class ShowCheck { public Kingmaker.EntitySystem.Stats.Base.StatType Type = Kingmaker.EntitySystem.Stats.Base.StatType.SkillPersuasion; public int DC = 10; }
    public class BlueprintAnswer {
        public bool HasShowCheck;
        public ShowCheck ShowCheck = new ShowCheck();
        public Kingmaker.ElementsSystem.ConditionsChecker ShowConditions = new Kingmaker.ElementsSystem.ConditionsChecker();
        public Kingmaker.ElementsSystem.ConditionsChecker SelectConditions = new Kingmaker.ElementsSystem.ConditionsChecker();
        public Kingmaker.ElementsSystem.GameAction[] Actions = Array.Empty<Kingmaker.ElementsSystem.GameAction>();
        public Kingmaker.UnitLogic.Alignments.SoulMarkShift Shift;
    }
    public class BlueprintCue : Kingmaker.Blueprints.SimpleBlueprint {
        private string text;
        public string TextEn, TextZh;
        public string Text { get => FixtureText.Resolve(text, TextZh, TextEn); set => text = value; }
        public Kingmaker.ElementsSystem.ActionList OnShow = new Kingmaker.ElementsSystem.ActionList(), OnStop = new Kingmaker.ElementsSystem.ActionList();
        public Kingmaker.UnitLogic.Alignments.SoulMarkShift SoulMarkShift;
    }
}
namespace Kingmaker.EntitySystem.Stats.Base { public enum StatType { SkillPersuasion, SkillAwareness, Unknown } }
internal static class FixtureText {
    internal static bool Chinese => Kingmaker.Localization.LocalizationManager.Instance.CurrentPack.Locale == Kingmaker.Localization.Enums.Locale.zhCN;
    internal static string Resolve(string original, string zh, string en) => original ?? (Chinese ? zh : en);
    internal static string Select(string zh, string en) => Chinese ? zh : en;
}
namespace Kingmaker.Localization.Enums { public enum Locale { enGB, zhCN, frFR } }
namespace Kingmaker.Localization.Shared { public class LocalizationPack { public Kingmaker.Localization.Enums.Locale Locale { get; set; } } }
namespace Kingmaker.Localization {
    public class LocalizationManager {
        public static LocalizationManager Instance = new LocalizationManager();
        public Shared.LocalizationPack CurrentPack { get; private set; } = new Shared.LocalizationPack { Locale = Enums.Locale.zhCN };
        public int Changes;
        [MethodImpl(MethodImplOptions.NoInlining)] public void OnLocaleProviderValueChanged(Enums.Locale locale) {
            CurrentPack = new Shared.LocalizationPack { Locale = locale };
            Changes++;
        }
    }
}
namespace Kingmaker.UI.Common {
    public static class UIUtility {
        public static bool MissingText;
        public static string GetSoulMarkDirectionText(Kingmaker.UnitLogic.Alignments.SoulMarkDirection direction) {
            if (MissingText) return "<null>";
            if (Kingmaker.Localization.LocalizationManager.Instance.CurrentPack.Locale == Kingmaker.Localization.Enums.Locale.frFR &&
                direction == Kingmaker.UnitLogic.Alignments.SoulMarkDirection.Faith) return "Doctrine";
            switch (direction) { case Kingmaker.UnitLogic.Alignments.SoulMarkDirection.Faith: return FixtureText.Select("教条主义", "Dogmatic");
                case Kingmaker.UnitLogic.Alignments.SoulMarkDirection.Hope: return FixtureText.Select("自由主义", "Iconoclast");
                case Kingmaker.UnitLogic.Alignments.SoulMarkDirection.Corruption: return FixtureText.Select("异端主义", "Heretical"); default: return direction.ToString(); }
        }
        public static string GetStatText(Kingmaker.EntitySystem.Stats.Base.StatType type) => MissingText ? "<null>" :
            Kingmaker.Localization.LocalizationManager.Instance.CurrentPack.Locale == Kingmaker.Localization.Enums.Locale.frFR &&
            type == Kingmaker.EntitySystem.Stats.Base.StatType.SkillPersuasion ? "Persuasion (fr)" :
            type == Kingmaker.EntitySystem.Stats.Base.StatType.SkillPersuasion ? FixtureText.Select("劝说", "Persuasion") :
            type == Kingmaker.EntitySystem.Stats.Base.StatType.SkillAwareness ? FixtureText.Select("警觉", "Awareness") : type.ToString();
    }
}
namespace ToyBox.Infrastructure {
    public class GeneralSettings {
        public static GeneralSettings Settings = new GeneralSettings();
        public string UILanguage = "zh-CN";
        public bool ToggleBPsShowDisplayAndInternalName, ToggleSearchDescriptions;
    }
}
namespace ToyBox.Infrastructure.Localization {
    public static class LocalizationManager { [MethodImpl(MethodImplOptions.NoInlining)] public static void Update() { ToyBox.Main.OnLocaleChanged?.Invoke(); } }
}
namespace ToyBox {
    public static class Main {
        public static UnityModManagerNet.UnityModManager.ModEntry ModEntry = CreateEntry();
        private static UnityModManagerNet.UnityModManager.ModEntry CreateEntry() {
            var entry = (UnityModManagerNet.UnityModManager.ModEntry)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(UnityModManagerNet.UnityModManager.ModEntry));
            entry.Enabled = true;
            return entry;
        }
        public static Action OnLocaleChanged;
        public static int BrowserInvalidations;
        public static void InvalidateBrowserCaches() { BrowserInvalidations++; }
        [MethodImpl(MethodImplOptions.NoInlining)] public static void OnGUI(UnityModManagerNet.UnityModManager.ModEntry entry) { }
    }
}
namespace ToyBox.Features.BagOfTricks.Preview {
    public static class DialogPreviewUtilities {
        public static int Traversals;
        public static string m_SoulMarkRequiredLocalizedText => ToyBox.Infrastructure.GeneralSettings.Settings.UILanguage == "zh-CN" ? "信念要求" : "SoulMarkRequired";
        public static string m_SoulMarkShiftLocalizedText => ToyBox.Infrastructure.GeneralSettings.Settings.UILanguage == "zh-CN" ? "信念变化" : "SoulMarkShift";
        public static string m_SoulMarkShift_0_By_1__DescriptioLocalizedText => ToyBox.Infrastructure.GeneralSettings.Settings.UILanguage == "zh-CN" ? "信念变化：{0}，数值 {1}；说明：{2}" : "SoulMarkShift {0} by {1} - Description: {2}";
        [MethodImpl(MethodImplOptions.NoInlining)] public static List<string> FormatActionAsList(Kingmaker.ElementsSystem.GameAction action) {
            if (action is Kingmaker.Designers.EventConditionActionSystem.Actions.Conditional conditional) {
                var chosen = conditional.ConditionsChecker.Check() ? conditional.IfTrue : conditional.IfFalse;
                return chosen.Actions.SelectMany(FormatActionAsList).ToList();
            }
            return new List<string> { action?.GetCaption() ?? "" };
        }
        [MethodImpl(MethodImplOptions.NoInlining)] public static string FormatConditions(Kingmaker.ElementsSystem.Condition[] conditions) => string.Join(", ", (conditions ?? Array.Empty<Kingmaker.ElementsSystem.Condition>()).Select(c => c.GetCaption()));
        [MethodImpl(MethodImplOptions.NoInlining)] public static List<string> FormatConditionsAsList(Kingmaker.DialogSystem.Blueprints.BlueprintAnswer answer) => new List<string> { "Native conditions" };
        [MethodImpl(MethodImplOptions.NoInlining)] public static string FormatSoulmarkShift(Kingmaker.UnitLogic.Alignments.SoulMarkShift shift, string format) => shift == null || shift.Value == 0 ? null : string.Format(format, Kingmaker.UI.Common.UIUtility.GetSoulMarkDirectionText(shift.Direction) + ", " + shift.Value);
        [MethodImpl(MethodImplOptions.NoInlining)] public static string GetAnswerResultText(Kingmaker.DialogSystem.Blueprints.BlueprintAnswer answer) {
            Traversals++;
            var line = answer.Actions.SelectMany(FormatActionAsList).ToList();
            var shift = FormatSoulmarkShift(answer.Shift, m_SoulMarkShiftLocalizedText + "({0})");
            if (shift != null) line.Add(shift);
            return "<size=65%>\n        " + string.Format("[{0}: {1}]", 4, string.Join(", ", line)) + "</size>";
        }
        [MethodImpl(MethodImplOptions.NoInlining)] public static string GetCueResultText(Kingmaker.DialogSystem.Blueprints.BlueprintCue cue) {
            var text = "";
            var actions = cue.OnShow.Actions.Concat(cue.OnStop.Actions).ToArray();
            if (actions.Length > 0) text += " \n<size=65%>[" + string.Join(", ", actions.SelectMany(FormatActionAsList)) + "]</size>";
            var shift = cue.SoulMarkShift;
            if (shift != null && shift.Value > 0)
                text += " \n<size=65%>[" + string.Format(m_SoulMarkShift_0_By_1__DescriptioLocalizedText, Kingmaker.UI.Common.UIUtility.GetSoulMarkDirectionText(shift.Direction), shift.Value, shift.Description) + "]</size>";
            return text;
        }
    }
}
namespace ToyBox.Infrastructure.Blueprints {
    public static class BPHelper {
        private static readonly Dictionary<Kingmaker.Blueprints.SimpleBlueprint, string> titles = new Dictionary<Kingmaker.Blueprints.SimpleBlueprint, string>(), searches = new Dictionary<Kingmaker.Blueprints.SimpleBlueprint, string>(), sorts = new Dictionary<Kingmaker.Blueprints.SimpleBlueprint, string>(), descriptions = new Dictionary<Kingmaker.Blueprints.SimpleBlueprint, string>();
        public static int Clears;
        public static void ClearNameCaches() { titles.Clear(); searches.Clear(); sorts.Clear(); descriptions.Clear(); Clears++; ToyBox.Main.InvalidateBrowserCaches(); }
        private static string Official(Kingmaker.Blueprints.SimpleBlueprint bp) => bp is Kingmaker.UI.Models.Tooltip.Base.IUIDataProvider provider && !string.IsNullOrEmpty(provider.Name) ? provider.Name : bp.name;
        [MethodImpl(MethodImplOptions.NoInlining)] public static string GetTitle(Kingmaker.Blueprints.SimpleBlueprint bp, Func<string, string> formatter = null) {
            if (!titles.TryGetValue(bp, out var title)) titles[bp] = title = Official(bp);
            return formatter == null ? title : formatter(title);
        }
        [MethodImpl(MethodImplOptions.NoInlining)] public static string GetSearchKey(Kingmaker.Blueprints.SimpleBlueprint bp) {
            if (!searches.TryGetValue(bp, out var search)) searches[bp] = search = (Official(bp) + " " + bp.name + " " + bp.AssetGuid + (GeneralSettings.Settings.ToggleSearchDescriptions ? " " + GetDescription(bp) : "")).ToUpperInvariant();
            return search;
        }
        [MethodImpl(MethodImplOptions.NoInlining)] public static string GetSortKey(Kingmaker.Blueprints.SimpleBlueprint bp) {
            if (!sorts.TryGetValue(bp, out var sort)) sorts[bp] = sort = Official(bp) + (GeneralSettings.Settings.ToggleSearchDescriptions ? " " + GetDescription(bp) : "");
            return sort;
        }
        [MethodImpl(MethodImplOptions.NoInlining)] public static string GetDescription(Kingmaker.Blueprints.SimpleBlueprint bp) {
            if (!descriptions.TryGetValue(bp, out var description)) descriptions[bp] = description = (bp as Kingmaker.UI.Models.Tooltip.Base.IUIDataProvider)?.Description;
            return description;
        }
    }
}
