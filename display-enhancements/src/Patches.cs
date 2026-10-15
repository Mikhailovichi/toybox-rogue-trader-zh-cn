using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace ToyBoxDisplayEnhancements {
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
            const string preview = "ToyBox.Features.BagOfTricks.Preview.DialogPreviewUtilities";
            const string bp = "ToyBox.Infrastructure.Blueprints.BPHelper";
            var action = Target(preview, "FormatActionAsList", "Kingmaker.ElementsSystem.GameAction");
            var array = Target(preview, "FormatConditions", "Kingmaker.ElementsSystem.Condition[]");
            var conditions = Target(preview, "FormatConditionsAsList", "Kingmaker.DialogSystem.Blueprints.BlueprintAnswer");
            var answer = Target(preview, "GetAnswerResultText", "Kingmaker.DialogSystem.Blueprints.BlueprintAnswer");
            var cue = Target(preview, "GetCueResultText", "Kingmaker.DialogSystem.Blueprints.BlueprintCue");
            var shift = Target(preview, "FormatSoulmarkShift", "Kingmaker.UnitLogic.Alignments.SoulMarkShift", "System.String");
            var title = Target(bp, "GetTitle", "Kingmaker.Blueprints.SimpleBlueprint", "System.Func`2");
            var search = Target(bp, "GetSearchKey", "Kingmaker.Blueprints.SimpleBlueprint");
            var sort = Target(bp, "GetSortKey", "Kingmaker.Blueprints.SimpleBlueprint");
            var description = Target(bp, "GetDescription", "Kingmaker.Blueprints.SimpleBlueprint");
            var locale = Target("ToyBox.Infrastructure.Localization.LocalizationManager", "Update");
            var gui = Target("ToyBox.Main", "OnGUI", "UnityModManagerNet.UnityModManager+ModEntry");
            Patch(harmony, action, prefix: nameof(ActionPrefix));
            Patch(harmony, array, prefix: nameof(ArrayPrefix));
            Patch(harmony, conditions, prefix: nameof(AnswerConditionsPrefix));
            Patch(harmony, shift, prefix: nameof(ShiftPrefix));
            Patch(harmony, answer, transpiler: nameof(PreviewTranspiler));
            Patch(harmony, cue, postfix: nameof(CuePreviewPostfix), transpiler: nameof(CueTranspiler));
            Patch(harmony, title, postfix: nameof(TitlePostfix));
            Patch(harmony, search, postfix: nameof(SearchPostfix));
            Patch(harmony, sort, postfix: nameof(SortPostfix));
            Patch(harmony, description, postfix: nameof(DescriptionPostfix));
            Patch(harmony, locale, postfix: nameof(CacheRefresh));
            Patch(harmony, gui, prefix: nameof(CacheRefresh));
            Patch(harmony, Runtime.GameLocaleChanged, postfix: nameof(CacheRefresh));
            return 13;
        }
        private static void CacheRefresh() {
            try { Runtime.RefreshCaches(); }
            catch (Exception ex) { Main.Warn(ex); }
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
                    list.Add(UiText.Get("Availability", Text.Check(Reflect.String(check, "Type"), Reflect.Int(check, "DC"), false, Runtime.Localize)));
                var show = Reflect.Read(__0, "ShowConditions");
                var select = Reflect.Read(__0, "SelectConditions");
                if (Reflect.Items(Reflect.Read(show, "Conditions")).Any()) list.Add(UiText.Get("Availability", Semantic.Checker(show)));
                if (Reflect.Items(Reflect.Read(select, "Conditions")).Any()) list.Add(UiText.Get("Selection", Semantic.Checker(select)));
                __result = list;
                return false;
            } catch (Exception ex) { Main.Warn(ex); return true; }
        }
        private static bool ShiftPrefix(object __0, string __1, ref string __result) {
            var requiredFormat = Runtime.PreviewText("m_SoulMarkRequiredLocalizedText") + "({0})";
            var shiftFormat = Runtime.PreviewText("m_SoulMarkShiftLocalizedText") + "({0})";
            if (!Runtime.Active || (__1 != requiredFormat && __1 != shiftFormat)) return true;
            try {
                if (__0 == null || Reflect.Int(__0, "Value") == 0) { __result = null; return false; }
                var required = __1 == requiredFormat;
                if (required && (!Reflect.Bool(__0, "CheckByValue") || Reflect.String(__0, "Direction") == "None")) return true;
                __result = Text.Shift(Reflect.String(__0, "Direction"), Reflect.Int(__0, "Value"), required,
                    Reflect.Localized(Reflect.Read(__0, "Description")), Runtime.Localize);
                if (Main.Settings.ShowTechnicalDetails) {
                    var description = Reflect.Localized(Reflect.Read(__0, "Description"));
                    var raw = Reflect.String(__0, "Direction") + ", " + Reflect.Int(__0, "Value") + (string.IsNullOrEmpty(description) ? "" : ", " + description);
                    __result = Text.WithDetails(__result, UiText.Get("Original", Text.Safe(string.Format(__1, raw))));
                }
                return false;
            } catch (Exception ex) { Main.Warn(ex); return true; }
        }
        internal static IEnumerable<CodeInstruction> PreviewTranspiler(IEnumerable<CodeInstruction> instructions) =>
            ReplaceFormat(instructions, new[] { typeof(string), typeof(object), typeof(object) }, nameof(FormatTwo));
        internal static IEnumerable<CodeInstruction> CueTranspiler(IEnumerable<CodeInstruction> instructions) =>
            ReplaceFormat(instructions, new[] { typeof(string), typeof(object), typeof(object), typeof(object) }, nameof(FormatThree));
        private static IEnumerable<CodeInstruction> ReplaceFormat(IEnumerable<CodeInstruction> instructions, Type[] parameters, string replacement) {
            var result = instructions.ToList();
            var method = typeof(string).GetMethod("Format", parameters);
            var calls = result.Where(i => i.Calls(method)).ToArray();
            if (calls.Length != 1) throw new NotSupportedException("Expected one pinned preview format call: " + replacement);
            calls[0].operand = Own(replacement);
            return result;
        }
        internal static string FormatTwo(string format, object first, object second) => Runtime.Active && format == "[{0}: {1}]"
            ? "[" + Text.Layer(Convert.ToInt32(first), second as string) + "]" : string.Format(format, first, second);
        internal static string FormatThree(string format, object first, object second, object third) {
            if (Runtime.Active && format == Runtime.PreviewText("m_SoulMarkShift_0_By_1__DescriptioLocalizedText"))
                return Text.WithDetails(Text.Shift(first.ToString(), Convert.ToInt32(second), false, third as string, Runtime.Localize),
                    UiText.Get("Original", Text.Safe(string.Format(format, first, second, third))));
            return string.Format(format, first, second, third);
        }
        private static void CuePreviewPostfix(ref string __result) {
            if (Runtime.Active && !string.IsNullOrEmpty(__result))
                __result = UiText.Get("CueResult", __result);
        }
        private static void TitlePostfix(object __0, Func<string, string> __1, ref string __result) {
            if (!Runtime.Active) return;
            try { __result = Aliases.Title(__0, __1) ?? DialogueText.Title(__0, __1) ?? __result; }
            catch (Exception ex) { Main.Warn(ex); }
        }
        private static void SearchPostfix(object __0, ref string __result) {
            if (!Runtime.Active) return;
            try {
                var alias = Aliases.Find(__0);
                if (alias != null) {
                    __result += " " + string.Join(" ", Aliases.SearchNames(__0)).ToUpperInvariant();
                    if (Runtime.Setting("ToggleSearchDescriptions")) __result += " " + alias.Description.ToUpperInvariant();
                }
                var dialogue = DialogueText.SearchText(__0);
                if (!string.IsNullOrWhiteSpace(dialogue)) __result += " " + dialogue.ToUpperInvariant();
            } catch (Exception ex) { Main.Warn(ex); }
        }
        private static void SortPostfix(object __0, ref string __result) {
            if (!Runtime.Active) return;
            try { __result = Text.Clean(Aliases.Title(__0, null) ?? DialogueText.Name(__0) ?? __result); }
            catch (Exception ex) { Main.Warn(ex); }
        }
        private static void DescriptionPostfix(object __0, ref string __result) {
            if (!Runtime.Active) return;
            try { __result = DialogueText.DescriptionFor(__0, Aliases.DescriptionFor(__0, __result)); }
            catch (Exception ex) { Main.Warn(ex); }
        }
    }
}
