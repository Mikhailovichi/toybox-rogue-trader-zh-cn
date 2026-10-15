using System;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

internal static class Verify {
    private static int checks;
    private static void Require(bool value, string message) {
        if (!value) throw new InvalidDataException(message);
        checks++;
    }
    private static MethodDef Method(ModuleDefMD module, string type, string name, params string[] parameters) {
        var matches = module.GetTypes().Where(t => t.FullName == type).SelectMany(t => t.Methods).Where(m =>
            m.Name == name && m.MethodSig.Params.Select(p => p is GenericInstSig g ? g.GenericType.FullName : p.FullName).SequenceEqual(parameters)).ToArray();
        Require(matches.Length == 1, "Missing/ambiguous entry: " + type + "." + name);
        return matches[0];
    }
    private static TypeDef Type(ModuleDefMD[] modules, string name) => modules.SelectMany(m => m.GetTypes()).Single(t => t.FullName == name);
    private static void Enum(ModuleDefMD[] modules, string name, params string[] values) => Require(
        Type(modules, name).Fields.Where(f => f.IsStatic).Select(f => f.Name.String).SequenceEqual(values), "Unexpected enum: " + name);
    private static void Member(ModuleDefMD[] modules, string name, params string[] members) {
        var type = Type(modules, name);
        foreach (var member in members) Require(type.Fields.Any(f => f.Name == member) || type.Properties.Any(p => p.Name == member), "Missing member: " + name + "." + member);
    }
    internal static void Run(string managed, string umm) {
        using var toy = ModuleDefMD.Load(Path.Combine(umm, "0ToyBox0", "ToyBox.dll"));
        using var code = ModuleDefMD.Load(Path.Combine(managed, "Code.dll"));
        using var core = ModuleDefMD.Load(Path.Combine(managed, "RogueTrader.GameCore.dll"));
        using var manager = ModuleDefMD.Load(Path.Combine(umm, "UnityModManager.dll"));
        using var localization = ModuleDefMD.Load(Path.Combine(managed, "LocalizationShared.dll"));
        using var locales = ModuleDefMD.Load(Path.Combine(managed, "Kingmaker.Localization.Enums.dll"));
        var game = new[] { code, core };
        const string gameLocalization = "Kingmaker.Localization.LocalizationManager";
        var localeModule = game.Single(m => m.GetTypes().Any(t => t.FullName == gameLocalization));
        var localeChanged = Method(localeModule, gameLocalization, "OnLocaleProviderValueChanged", "Kingmaker.Localization.Enums.Locale");
        Require(!localeChanged.IsStatic && localeChanged.MethodSig.RetType.FullName == "System.Void", "Game locale update signature changed");
        Member(game, gameLocalization, "Instance", "CurrentPack");
        Member(new[] { localization }, "Kingmaker.Localization.Shared.LocalizationPack", "Locale");
        Require(Type(new[] { locales }, "Kingmaker.Localization.Enums.Locale").Fields.Any(f => f.Name == "enGB") &&
            Type(new[] { locales }, "Kingmaker.Localization.Enums.Locale").Fields.Any(f => f.Name == "zhCN"), "English/Chinese game locales missing");
        Require(localeChanged.Body.Instructions.Any(i => i.Operand is IMethod m && m.Name == "set_CurrentPack"), "Game locale hook must follow pack update");
        Require(toy.Assembly.Version == new Version(2, 0, 16, 0), "ToyBox version must be 2.0.16.0");
        const string util = "ToyBox.Features.BagOfTricks.Preview.DialogPreviewUtilities";
        const string ext = "ToyBox.Infrastructure.Blueprints.BPHelper";
        const string answer = "Kingmaker.DialogSystem.Blueprints.BlueprintAnswer";
        Method(toy, util, "FormatActionAsList", "Kingmaker.ElementsSystem.GameAction");
        Method(toy, util, "FormatConditions", "Kingmaker.ElementsSystem.Condition[]");
        Method(toy, util, "FormatConditionsAsList", answer);
        Method(toy, util, "FormatSoulmarkShift", "Kingmaker.UnitLogic.Alignments.SoulMarkShift", "System.String");
        var answerResult = Method(toy, util, "GetAnswerResultText", answer);
        var cueResult = Method(toy, util, "GetCueResultText", "Kingmaker.DialogSystem.Blueprints.BlueprintCue");
        Require(answerResult.Body.Instructions.Count(i => i.OpCode == OpCodes.Ldstr && (string)i.Operand == "[{0}: {1}]") == 1, "Pinned answer depth format changed");
        Require(answerResult.Body.Instructions.Count(i => i.Operand is IMethod m && m.Name == "CollateAnswerData") == 1, "Answer traversal changed");
        Require(answerResult.Body.Instructions.Count(i => i.Operand is IMethod m && m.FullName == "System.String System.String::Format(System.String,System.Object,System.Object)") == 1, "Answer Format overload changed");
        Require(cueResult.Body.Instructions.Count(i => i.Operand is IMethod m && m.FullName == "System.String System.String::Format(System.String,System.Object,System.Object,System.Object)") == 1, "Cue Format overload changed");
        Require(cueResult.Body.Instructions.Any(i => i.Operand is IMethod m && m.Name == "get_m_SoulMarkShift_0_By_1__DescriptioLocalizedText"), "Cue localized format changed");
        foreach (var name in new[] { "m_SoulMarkRequiredLocalizedText", "m_SoulMarkShiftLocalizedText", "m_SoulMarkShift_0_By_1__DescriptioLocalizedText" })
            Method(toy, util, "get_" + name);
        Method(toy, ext, "GetTitle", "Kingmaker.Blueprints.SimpleBlueprint", "System.Func`2");
        Method(toy, ext, "GetSearchKey", "Kingmaker.Blueprints.SimpleBlueprint");
        Method(toy, ext, "GetSortKey", "Kingmaker.Blueprints.SimpleBlueprint");
        Method(toy, ext, "GetDescription", "Kingmaker.Blueprints.SimpleBlueprint");
        var clear = Method(toy, ext, "ClearNameCaches");
        Require(clear.Body.Instructions.Count(i => i.Operand is IMethod m && m.Name == "Clear") == 4, "Four name caches must be cleared");
        Require(clear.Body.Instructions.Any(i => i.Operand is IMethod m && m.Name == "InvalidateBrowserCaches"), "Browser invalidation missing");
        Method(toy, "ToyBox.Infrastructure.Localization.LocalizationManager", "Update");
        Method(toy, "ToyBox.Main", "OnGUI", "UnityModManagerNet.UnityModManager/ModEntry");
        Member(new[] { toy }, "ToyBox.Main", "ModEntry");
        Member(new[] { toy }, "ToyBox.Infrastructure.GeneralSettings", "Settings", "UILanguage", "ToggleBPsShowDisplayAndInternalName", "ToggleSearchDescriptions");
        Method(code, "Kingmaker.UI.Common.UIUtility", "GetSoulMarkDirectionText", "Kingmaker.UnitLogic.Alignments.SoulMarkDirection");
        Method(code, "Kingmaker.UI.Common.UIUtility", "GetStatText", "Kingmaker.EntitySystem.Stats.Base.StatType");
        Member(new[] { manager }, "UnityModManagerNet.UnityModManager/ModEntry", "Enabled");
        Enum(game, "Kingmaker.Designers.Quests.Common.SummonPoolCountTrigger/ObjectiveStatus", "Complete", "Fail");
        Enum(game, "Kingmaker.QuestObjectiveState", "None", "Started", "Completed", "Postponed", "Failed");
        Enum(game, "Kingmaker.AreaLogic.QuestSystem.QuestState", "None", "Started", "Completed", "Updated", "Postponed", "Failed");
        Enum(game, "Kingmaker.ElementsSystem.Operation", "And", "Or");
        const string actions = "Kingmaker.Designers.EventConditionActionSystem.Actions.";
        const string conditions = "Kingmaker.Designers.EventConditionActionSystem.Conditions.";
        Member(game, actions + "GiveObjective", "Objective");
        Member(game, actions + "SetObjectiveStatus", "Objective", "Status", "StartObjectiveIfNone");
        Member(game, actions + "StartEtude", "Etude", "Evaluate", "StartImmediately");
        Member(game, actions + "CompleteEtude", "Etude", "Evaluate");
        Member(game, conditions + "ObjectiveStatus", "QuestObjective", "State");
        Member(game, conditions + "QuestStatus", "Quest", "State");
        Member(game, conditions + "EtudeStatus", "Etude", "NotStarted", "Started", "Playing", "CompletionInProgress", "Completed");
        Member(game, conditions + "CheckConditionsHolder", "ConditionsHolder");
        Member(game, conditions + "OrAndLogic", "ConditionsChecker");
        Member(game, conditions + "CueSeen", "Cue", "CurrentDialog");
        Member(game, conditions + "AnswerSelected", "Answer", "CurrentDialog");
        Member(game, conditions + "AnswerListShown", "AnswersList", "CurrentDialog");
        var answerReference = Method(code, conditions + "AnswerSelected", "get_Answer");
        Require(answerReference.Body.Instructions.Any(i => i.Operand is IMethod m && m.Name == "Get") &&
            answerReference.MethodSig.RetType.FullName == answer, "AnswerSelected must resolve its answer reference");
        foreach (var entry in new[] { new[] { "AnswerSelected", "LocalSelectedAnswers", "SelectedAnswersContains" },
                                      new[] { "AnswerListShown", "LocalShownAnswerLists", "ShownAnswerListsContains" } }) {
            var check = Method(code, conditions + entry[0], "CheckCondition");
            Require(check.Body.Instructions.Any(i => i.Operand is IField f && f.Name == "CurrentDialog") &&
                check.Body.Instructions.Any(i => i.Operand is IField f && f.Name == entry[1]) &&
                check.Body.Instructions.Any(i => i.Operand is IMethod m && m.Name == entry[2]),
                entry[0] + " current/history scope changed");
        }
        Member(game, answer, "Text", "Description");
        Member(game, "Kingmaker.DialogSystem.Blueprints.BlueprintAnswersList", "Answers");
        var listAnswers = Type(game, "Kingmaker.DialogSystem.Blueprints.BlueprintAnswersList").Fields.Single(f => f.Name == "Answers");
        Require(listAnswers.FieldType.FullName == "System.Collections.Generic.List`1<Kingmaker.Blueprints.BlueprintAnswerBaseReference>",
            "Answer lists must preserve ordered answer/base-list references");
        Require(Type(game, conditions + "HasEnoughMoneyForCustomCompanion").Methods.Any(m => m.Name == "CheckCondition"), "Missing companion affordability condition");
        Require(Type(game, actions + "OpenVendorSelectingWindow").Methods.Any(m => m.Name == "RunAction"), "Missing vendor action type");
        Require(Type(game, actions + "CreateCustomCompanion").Methods.Any(m => m.Name == "RunAction"), "Missing companion creation action type");
        Member(game, "Kingmaker.DialogSystem.Blueprints.BlueprintCue", "Text");
        Member(game, "Kingmaker.ElementsSystem.Condition", "Not");
        Member(game, "Kingmaker.ElementsSystem.ConditionsChecker", "Operation", "Conditions");
        Member(game, "Kingmaker.UnitLogic.Alignments.SoulMarkShift", "Direction", "Value", "CheckByValue", "Description");
        Member(game, "Kingmaker.Blueprints.Quests.BlueprintQuest", "Title");
        Member(game, "Kingmaker.Blueprints.Quests.BlueprintQuestObjective", "Title", "Quest");
        Method(manager, "UnityModManagerNet.UnityModManager", "FindMod", "System.String");
        DialogueDisplay(game, code);
        Console.WriteLine("PASS: " + checks + " actual-assembly compatibility checks; no game code executed.");
    }

    private static void DialogueDisplay(ModuleDefMD[] game, ModuleDefMD code) {
        const string bp = "Kingmaker.DialogSystem.Blueprints.";
        const string conditions = "Kingmaker.Designers.EventConditionActionSystem.Conditions.";
        const string actions = "Kingmaker.Designers.EventConditionActionSystem.Actions.";
        const string evaluators = "Kingmaker.Designers.EventConditionActionSystem.Evaluators.";
        Member(game, bp + "BlueprintCue", "Text", "Description", "Continue", "Answers");
        Member(game, bp + "BlueprintCheck", "Type", "Hidden", "Difficulty", "DC", "DCModifiers");
        Member(game, bp + "DCModifier", "Mod", "Conditions");
        Member(game, bp + "BlueprintDialog", "FirstCue", "Description");
        Member(game, "Kingmaker.DialogSystem.CueSelection", "Cues");
        Member(game, bp + "BlueprintBookPage", "Cues", "Answers");
        Member(game, bp + "BlueprintCueSequence", "Cues", "Exit");
        Member(game, bp + "BlueprintSequenceExit", "Continue", "Answers");
        Enum(game, "Kingmaker.View.MapObjects.SkillCheckDifficulty", "Custom", "Easy", "Normal", "Hard", "Impossible");
        var custom = Type(game, "Kingmaker.View.MapObjects.SkillCheckDifficulty").Fields.Single(f => f.Name == "Custom");
        Require(Convert.ToInt32(custom.Constant.Value) == 0, "Custom difficulty value changed");
        var difficulty = Method(code, bp + "BlueprintCheck", "get_DifficultyIsCustom");
        Require(difficulty.Body.Instructions.Any(i => i.OpCode == OpCodes.Ceq) &&
            difficulty.Body.Instructions.Any(i => i.Operand is IField f && f.Name == "Difficulty"), "Check custom/preset difficulty distinction changed");
        foreach (var entry in new[] {
            new[] { "BlueprintBookPage", "Cues", "BlueprintCueBaseReference" },
            new[] { "BlueprintBookPage", "Answers", "BlueprintAnswerBaseReference" },
            new[] { "BlueprintCueSequence", "Cues", "BlueprintCueBaseReference" },
            new[] { "BlueprintSequenceExit", "Answers", "BlueprintAnswerBaseReference" }
        }) {
            var field = Type(game, bp + entry[0]).Fields.Single(f => f.Name == entry[1]);
            Require(field.FieldType.FullName == "System.Collections.Generic.List`1<Kingmaker.Blueprints." + entry[2] + ">",
                entry[0] + "." + entry[1] + " must remain an ordered reference list");
        }
        foreach (var entry in new[] { new[] { "CheckPassed", "LocalPassedChecks" }, new[] { "CheckFailed", "LocalFailedChecks" } }) {
            Member(game, conditions + entry[0], "CheckBlueprint");
            var method = Method(code, conditions + entry[0], "CheckCondition");
            Require(method.Body.Instructions.Any(i => i.Operand is IField f && f.Name == entry[1]) &&
                method.Body.Instructions.Any(i => i.Operand is IMethod m && m.Name == "Contains"),
                entry[0] + " must test the local dialog record, not infer the opposite record");
        }
        Member(game, conditions + "CompanionInParty", "companion", "MatchWhenActive", "MatchWhenRemote", "MatchWhenDetached", "MatchWhenEx", "IncludeDead");
        Enum(game, "Kingmaker.UnitLogic.Parts.CompanionState", "None", "Remote", "InParty", "ExCompanion", "InPartyDetached");
        var companion = Method(code, conditions + "CompanionInParty", "CheckCondition");
        foreach (var field in new[] { "MatchWhenActive", "MatchWhenRemote", "MatchWhenDetached", "MatchWhenEx", "IncludeDead" })
            Require(companion.Body.Instructions.Any(i => i.Operand is IField f && f.Name == field), "Companion filter no longer used: " + field);
        Require(companion.Body.Instructions.Any(i => i.Operand is IMethod m && m.Name == "get_AllCharacters") &&
            companion.Body.Instructions.Any(i => i.Operand is IMethod m && m.Name == "get_IsFinallyDead"),
            "Companion roster or death filter changed");
        Member(game, "Kingmaker.Blueprints.BlueprintUnit", "CharacterName");
        Member(game, "Kingmaker.Blueprints.BlueprintScriptableObject", "ComponentsArray");
        Member(game, conditions + "ItemsEnough", "Money", "ItemToCheck", "Quantity");
        var items = Method(code, conditions + "ItemsEnough", "CheckCondition");
        Require(items.Body.Instructions.Any(i => i.Operand is IMethod m && m.FullName.Contains("MoneyReplacement")) &&
            items.Body.Instructions.Any(i => i.Operand is IMethod m && m.Name == "get_Money") &&
            items.Body.Instructions.Any(i => i.Operand is IMethod m && m.Name == "Contains"),
            "ItemsEnough money replacement or inventory branch changed");
        Member(game, conditions + "HasFact", "Unit", "Fact");
        Member(game, conditions + "HasBuff", "Target", "Buff");
        Require(Type(game, evaluators + "PlayerCharacter") != null, "Player character evaluator missing");
        Member(game, evaluators + "CompanionInParty", "Companion");
        Member(game, evaluators + "IntConstant", "Value");
        Member(game, conditions + "FlagUnlocked", "ConditionFlag", "SpecifiedValues", "ExceptSpecifiedValues");
        Member(game, conditions + "FlagInRange", "Flag", "MinValue", "MaxValue");
        foreach (var condition in new[] { "FlagUnlocked", "FlagInRange" }) {
            var method = Method(code, conditions + condition, "CheckCondition");
            Require(method.Body.Instructions.Any(i => i.Operand is IMethod m && m.Name == "get_IsUnlocked") &&
                method.Body.Instructions.Any(i => i.Operand is IMethod m && m.Name == "get_Value"),
                condition + " must require unlock before testing configured values");
        }
        foreach (var action in new[] { "MarkAnswersSelected", "UnmarkAnswersSelected" }) {
            Member(game, actions + action, "Answers");
            Require(Type(game, actions + action).Fields.Single(f => f.Name == "m_Answers").FieldType.FullName ==
                "Kingmaker.Blueprints.BlueprintAnswerReference[]", "Mark action answer reference array changed");
        }
        Member(game, actions + "StartDialog", "Dialogue", "DialogEvaluator", "DialogueOwner");
        Member(game, actions + "UnlockFlag", "flag", "flagValue");
        Member(game, actions + "LockFlag", "Flag");
        Member(game, actions + "IncrementFlagValue", "Flag", "Value", "UnlockIfNot");
        var increment = Method(code, actions + "IncrementFlagValue", "RunAction");
        Require(increment.Body.Instructions.Any(i => i.Operand is IField f && f.Name == "UnlockIfNot") &&
            increment.Body.Instructions.Any(i => i.Operand is IMethod m && m.Name == "get_IsUnlocked") &&
            increment.Body.Instructions.Any(i => i.Operand is IMethod m && m.Name == "Unlock") &&
            increment.Body.Instructions.Any(i => i.Operand is IMethod m && m.Name == "GetValue"),
            "IncrementFlagValue unlock/evaluation policy changed");
    }
}
