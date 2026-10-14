using System;
using System.IO;
using System.Linq;
using dnlib.DotNet;

internal static class Program {
    private static void Main(string[] args) {
        if (args.Length == 3 && args[0] == "--verify") { Verify.Run(args[1], args[2]); return; }
        if (args.Length == 4 && args[0] == "--method") {
            using var module = ModuleDefMD.Load(args[1]);
            foreach (var method in module.GetTypes().Where(t => t.FullName == args[2]).SelectMany(t => t.Methods).Where(m => m.Name == args[3])) {
                Console.WriteLine(method.FullName);
                if (method.HasBody) foreach (var instruction in method.Body.Instructions) Console.WriteLine(instruction);
            }
            return;
        }
        foreach (var path in args) {
            using var module = ModuleDefMD.Load(path);
            Console.WriteLine("ASSEMBLY " + Path.GetFileName(path));
            foreach (var type in module.GetTypes()) {
                if (new[] { "Feature", "Buff", "MechanicEntityFact", "BlueprintBuff", "BlueprintMechanicEntityFact", "IUIDataProvider", "QuestObjectiveState", "SoulMarkDirection", "CombatStateTrigger" }.Contains(type.Name.String)) {
                    Console.WriteLine("TYPE " + type.FullName + " FIELDS " + string.Join(",", type.Fields.Select(f => f.Name.String)));
                }
                if (!type.Name.String.Contains("CombatStateTrigger") && !new[] { "GiveObjective", "SetObjectiveStatus", "ObjectiveStatus", "QuestStatus", "EtudeStatus", "BlueprintAnswer", "SoulMarkExtensions",
                    "ConditionsChecker", "SoulMarkShift", "SoulMarkShiftExtension", "FactsEditor", "PreviewManager", "BlueprintFeature",
                    "CombatStateTrigger", "ContextActionCastSpell", "UnityModManager", "ModEntry" }.Contains(type.Name.String) && !type.Methods.Any(m => m.Name == "IsSoulMarkRequirementSatisfied")) continue;
                foreach (var method in type.Methods.Where(m => type.Name.String.Contains("CombatStateTrigger") || new[] { "GetCaption", "GetCaptionInternal", "CheckCondition",
                    "CheckConditionInternal", "BlueprintRowGUI", "ResultsText", "FormatShift", "CollateAnswerData",
                    "get_Description", "Check", "CheckShiftAtLeast", "IsSoulMarkRequirementSatisfied" }.Contains(m.Name.String))) {
                    Console.WriteLine(method.FullName);
                    if (method.HasBody) foreach (var instruction in method.Body.Instructions)
                        Console.WriteLine("  " + instruction);
                }
            }
        }
    }
}
