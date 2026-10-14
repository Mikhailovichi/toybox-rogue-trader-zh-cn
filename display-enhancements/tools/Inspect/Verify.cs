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
        Console.WriteLine("PASS: " + checks + " actual-assembly compatibility checks; no game code executed.");
    }
}
