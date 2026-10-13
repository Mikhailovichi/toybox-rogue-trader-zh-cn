using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace ToyBoxChineseDisplayEnhancements {
    internal static class Patches {
        private static MethodInfo Own(string name) => typeof(Patches).GetMethod(name, Reflect.Flags);
        private static MethodInfo Target(string type, string name, params string[] parameters) {
            var target = Runtime.ToyBox.GetType(type, true).GetMethods(Reflect.Flags).SingleOrDefault(m =>
                m.Name == name && m.GetParameters().Select(p => p.ParameterType.IsGenericType ? p.ParameterType.GetGenericTypeDefinition().FullName : p.ParameterType.FullName).SequenceEqual(parameters));
            return target ?? throw new MissingMethodException(type + "." + name);
        }
        private static void Patch(Harmony harmony, MethodInfo method, string prefix = null, string postfix = null, string transpiler = null) {
            harmony.Patch(method, prefix == null ? null : new HarmonyMethod(Own(prefix)),
                postfix == null ? null : new HarmonyMethod(Own(postfix)), transpiler == null ? null : new HarmonyMethod(Own(transpiler)));
        }
        internal static int Install(Harmony harmony) {
            // Resolve every required entry point before installing any patch.
            var action = Target("ToyBox.PreviewUtilities", "FormatActionAsList", "Kingmaker.ElementsSystem.GameAction");
            var checker = Target("ToyBox.PreviewUtilities", "FormatConditions", "Kingmaker.ElementsSystem.ConditionsChecker");
            var array = Target("ToyBox.PreviewUtilities", "FormatConditions", "Kingmaker.ElementsSystem.Condition[]");
            var answerConditions = Target("ToyBox.PreviewUtilities", "FormatConditionsAsList", "Kingmaker.DialogSystem.Blueprints.BlueprintAnswer");
            var answer = Target("ToyBox.PreviewManager", "ResultsText", "Kingmaker.DialogSystem.Blueprints.BlueprintAnswer");
            var cue = Target("ToyBox.PreviewManager", "ResultsText", "Kingmaker.DialogSystem.Blueprints.BlueprintCue");
            var shift = Target("ToyBox.PreviewManager", "FormatShift", "Kingmaker.UnitLogic.Alignments.SoulMarkShift", "System.String");
            var title = Target("ToyBox.BlueprintExtensions", "GetTitle", "Kingmaker.Blueprints.SimpleBlueprint", "System.Func`2");
            var search = Target("ToyBox.BlueprintExtensions", "GetSearchKey", "Kingmaker.Blueprints.SimpleBlueprint", "System.Boolean");
            var sort = Target("ToyBox.BlueprintExtensions", "GetSortKey", "Kingmaker.Blueprints.SimpleBlueprint");
            var row = Runtime.ToyBox.GetType("ToyBox.FactsEditor", true).GetMethod("BlueprintRowGUI", Reflect.Flags);
            if (row == null || !row.IsGenericMethodDefinition || row.GetGenericArguments().Length != 2)
                throw new MissingMethodException("FactsEditor.BlueprintRowGUI<T,U>");
            // All current instantiations use reference types and share this generic body on Mono.
            var featureType = action.GetParameters()[0].ParameterType.Assembly.GetType("Kingmaker.UnitLogic.Feature");
            var featureBlueprint = action.GetParameters()[0].ParameterType.Assembly.GetType("Kingmaker.UnitLogic.Progression.Features.BlueprintFeature");
            // GameAction lives in GameCore; feature classes live in Code.
            featureType = featureType ?? AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("Kingmaker.UnitLogic.Feature")).FirstOrDefault(t => t != null);
            featureBlueprint = featureBlueprint ?? AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("Kingmaker.UnitLogic.Progression.Features.BlueprintFeature")).FirstOrDefault(t => t != null);
            if (featureType == null || featureBlueprint == null) throw new TypeLoadException("Feature row types are missing.");
            var closedRow = row.MakeGenericMethod(featureType, featureBlueprint);

            Patch(harmony, action, prefix: nameof(ActionPrefix));
            Patch(harmony, checker, prefix: nameof(CheckerPrefix));
            Patch(harmony, array, prefix: nameof(ArrayPrefix));
            Patch(harmony, answerConditions, prefix: nameof(AnswerConditionsPrefix));
            Patch(harmony, shift, prefix: nameof(ShiftPrefix));
            Patch(harmony, answer, transpiler: nameof(PreviewTranspiler));
            Patch(harmony, cue, postfix: nameof(CuePreviewPostfix), transpiler: nameof(PreviewTranspiler));
            Patch(harmony, title, postfix: nameof(TitlePostfix));
            Patch(harmony, search, postfix: nameof(SearchPostfix));
            Patch(harmony, sort, postfix: nameof(SortPostfix));
            Patch(harmony, closedRow, transpiler: nameof(RowTranspiler));
            return 11;
        }
        private static bool ActionPrefix(object __0, ref List<string> __result) {
            if (!Runtime.Active) return true;
            try {
                var text = Semantic.Action(__0);
                if (text == null) return true;
                __result = new List<string> { text };
                return false;
            } catch (Exception ex) { Main.Warn(ex); return true; }
        }
        private static bool CheckerPrefix(object __0, ref string __result) {
            if (!Runtime.Active) return true;
            try { __result = Semantic.Checker(__0); return false; }
            catch (Exception ex) { Main.Warn(ex); return true; }
        }
        private static bool ArrayPrefix(object __0, ref string __result) {
            if (!Runtime.Active) return true;
            try { __result = string.Join("\n", Reflect.Items(__0).Select(c => Semantic.Condition(c))); return false; }
            catch (Exception ex) { Main.Warn(ex); return true; }
        }
        private static bool AnswerConditionsPrefix(object __0, ref List<string> __result) {
            if (!Runtime.Active) return true;
            try {
                var list = new List<string>();
                var check = Reflect.Read(__0, "ShowCheck");
                if (Reflect.Bool(__0, "HasShowCheck"))
                    list.Add("出现条件：" + Text.Check(Reflect.String(check, "Type"), Reflect.Int(check, "DC"), false, Runtime.Localize));
                var show = Reflect.Read(__0, "ShowConditions");
                var select = Reflect.Read(__0, "SelectConditions");
                if (Reflect.Items(Reflect.Read(show, "Conditions")).Any()) list.Add("出现条件：" + Semantic.Checker(show));
                if (Reflect.Items(Reflect.Read(select, "Conditions")).Any()) list.Add("选择要求：" + Semantic.Checker(select));
                __result = list;
                return false;
            } catch (Exception ex) { Main.Warn(ex); return true; }
        }
        private static bool ShiftPrefix(object __0, string __1, ref string __result) {
            if (!Runtime.Active || (__1 != "SoulMarkRequired({0})" && __1 != "SoulMarkShift({0})")) return true;
            try {
                if (__0 == null || Reflect.Int(__0, "Value") == 0) { __result = null; return false; }
                var required = __1.StartsWith("SoulMarkRequired", StringComparison.Ordinal);
                if (required && (!Reflect.Bool(__0, "CheckByValue") || Reflect.String(__0, "Direction") == "None")) return true;
                __result = Text.Shift(Reflect.String(__0, "Direction"), Reflect.Int(__0, "Value"), required,
                    Reflect.Localized(Reflect.Read(__0, "Description")), Runtime.Localize);
                if (Main.Settings.ShowTechnicalDetails) {
                    var description = Reflect.Localized(Reflect.Read(__0, "Description"));
                    var raw = Reflect.String(__0, "Direction") + ", " + Reflect.Int(__0, "Value") + (string.IsNullOrEmpty(description) ? "" : ", " + description);
                    __result = Text.WithDetails(__result, "原始：" + Text.Safe(string.Format(__1, raw)));
                }
                return false;
            } catch (Exception ex) { Main.Warn(ex); return true; }
        }
        internal static IEnumerable<CodeInstruction> PreviewTranspiler(IEnumerable<CodeInstruction> instructions) {
            var two = typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object), typeof(object) });
            var three = typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object), typeof(object), typeof(object) });
            foreach (var instruction in instructions) {
                if (instruction.Calls(two)) instruction.operand = Own(nameof(FormatTwo));
                if (instruction.Calls(three)) instruction.operand = Own(nameof(FormatThree));
                yield return instruction;
            }
        }
        internal static string FormatTwo(string format, object first, object second) => Runtime.Active && format == "{0}: {1}"
            ? "\n" + Text.Layer(Convert.ToInt32(first), second as string) : string.Format(format, first, second);
        internal static string FormatThree(string format, object first, object second, object third) {
            if (Runtime.Active) {
                if (format == "Check({0}, DC {1}, hidden {2})") return Text.WithDetails(Text.Check(first.ToString(), Convert.ToInt32(second), Convert.ToBoolean(third), Runtime.Localize), "原始：" + string.Format(format, first, second, third));
                if (format == " \n<size=75%>[SoulMarkShift {0} by {1} - {2}]")
                    return " \n<size=75%>[" + Text.WithDetails(Text.Shift(first.ToString(), Convert.ToInt32(second), false, third as string, Runtime.Localize), "原始：" + Text.Safe(string.Format("SoulMarkShift {0} by {1} - {2}", first, second, third))) + "]</size>";
            }
            return string.Format(format, first, second, third);
        }
        private static void CuePreviewPostfix(ref string __result) {
            if (Runtime.Active && !string.IsNullOrEmpty(__result))
                __result = "\n<size=75%>结果预览：</size>" + __result;
        }
        private static void TitlePostfix(object __0, Func<string, string> __1, ref string __result) {
            if (!Runtime.Active) return;
            try { __result = Aliases.Title(__0, __1) ?? __result; }
            catch (Exception ex) { Main.Warn(ex); }
        }
        private static void SearchPostfix(object __0, ref string __result) {
            if (!Runtime.Active) return;
            try {
                var alias = Aliases.Find(__0);
                if (alias == null) return;
                __result += " " + alias.Name + " " + alias.InternalName;
                if (Runtime.Setting("searchDescriptions")) __result += " " + alias.Description;
            } catch (Exception ex) { Main.Warn(ex); }
        }
        private static void SortPostfix(object __0, ref string __result) {
            if (!Runtime.Active) return;
            try { __result = Text.Clean(Aliases.Title(__0, null) ?? __result); }
            catch (Exception ex) { Main.Warn(ex); }
        }
        internal static IEnumerable<CodeInstruction> RowTranspiler(IEnumerable<CodeInstruction> instructions) {
            var result = instructions.ToList();
            var calls = result.Where(i => i.operand is MethodInfo method && method.Name == "get_Description" && method.ReturnType == typeof(string)).ToList();
            if (calls.Count != 1) throw new NotSupportedException("Expected exactly one description getter in the ToyBox fact row.");
            // Only this GUI call is replaced; the game getter and blueprint remain untouched.
            calls[0].opcode = OpCodes.Call;
            calls[0].operand = typeof(Aliases).GetMethod(nameof(Aliases.DescriptionFor));
            return result;
        }
    }
}
