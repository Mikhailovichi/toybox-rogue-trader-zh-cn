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
        var game = new[] { code, core };
        Require(toy.Assembly.Version == new Version(1, 7, 34, 0), "ToyBox version must be 1.7.34.0");
        const string util = "ToyBox.PreviewUtilities";
        const string preview = "ToyBox.PreviewManager";
        const string ext = "ToyBox.BlueprintExtensions";
        const string answer = "Kingmaker.DialogSystem.Blueprints.BlueprintAnswer";
        Method(toy, util, "FormatActionAsList", "Kingmaker.ElementsSystem.GameAction");
        Method(toy, util, "FormatConditions", "Kingmaker.ElementsSystem.ConditionsChecker");
        Method(toy, util, "FormatConditions", "Kingmaker.ElementsSystem.Condition[]");
        Method(toy, util, "FormatConditionsAsList", answer);
        Method(toy, preview, "FormatShift", "Kingmaker.UnitLogic.Alignments.SoulMarkShift", "System.String");
        var answerResult = Method(toy, preview, "ResultsText", answer);
        var cueResult = Method(toy, preview, "ResultsText", "Kingmaker.DialogSystem.Blueprints.BlueprintCue");
        foreach (var text in new[] { "{0}: {1}", "Check({0}, DC {1}, hidden {2})" })
            Require(answerResult.Body.Instructions.Any(i => i.OpCode == OpCodes.Ldstr && (string)i.Operand == text), "Missing answer format: " + text);
        Require(cueResult.Body.Instructions.Any(i => i.OpCode == OpCodes.Ldstr && (string)i.Operand == " \n<size=75%>[SoulMarkShift {0} by {1} - {2}]"), "Missing cue shift format");
        Method(toy, ext, "GetTitle", "Kingmaker.Blueprints.SimpleBlueprint", "System.Func`2");
        Method(toy, ext, "GetSearchKey", "Kingmaker.Blueprints.SimpleBlueprint", "System.Boolean");
        Method(toy, ext, "GetSortKey", "Kingmaker.Blueprints.SimpleBlueprint");
        var row = toy.GetTypes().Single(t => t.FullName == "ToyBox.FactsEditor").Methods.Single(m => m.Name == "BlueprintRowGUI");
        Require(row.GenericParameters.Count == 2 && row.MethodSig.Params.Count == 5, "Unexpected generic row signature");
        var getters = row.Body.Instructions.Where(i => i.Operand is IMethod m && m.Name == "get_Description").ToArray();
        Require(getters.Length == 1 && ((IMethod)getters[0].Operand).DeclaringType.FullName == "Kingmaker.UI.Models.Tooltip.Base.IUIDataProvider", "Row description call changed");
        Require(!row.Body.Instructions.Any(i => i.OpCode == OpCodes.Constrained), "Unsupported constrained row call");
        Method(toy, "ModKit.LocalizationManager", "localize", "System.String");
        Member(new[] { toy }, "ToyBox.Main", "Enabled", "Settings");
        Member(new[] { toy }, "ModKit.Mod", "ModKitSettings");
        Member(new[] { toy }, "ToyBox.Settings", "showDisplayAndInternalNames", "searchDescriptions");
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
