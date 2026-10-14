using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ToyBoxDisplayEnhancements {
    internal static class Runtime {
        internal static Assembly ToyBox;
        internal static Type ToyBoxMain;
        internal static Type GeneralSettings;
        internal static Type PreviewType;
        internal static Type GameLocalization;
        internal static MethodInfo GameLocaleChanged;
        internal static MethodInfo ClearNameCaches;
        internal static MethodInfo ConvictionText;
        internal static MethodInfo StatText;
        private static bool? lastActive;
        private static string lastCulture;
        private static string lastGameCulture;
        private static bool lastDescriptions, lastInternalNames;
        internal static bool Active => Main.Enabled && Reflect.Bool(Reflect.Read(ToyBoxMain, "ModEntry"), "Enabled") && SupportedCulture;
        internal static string Culture => Reflect.String(Reflect.Read(GeneralSettings, "Settings"), "UILanguage");
        internal static string GameCulture => Reflect.String(Reflect.Read(Reflect.Read(GameLocalization, "Instance"), "CurrentPack"), "Locale");
        internal static bool SupportedCulture {
            get {
                if (UiText.Chinese) return true;
                if (string.IsNullOrEmpty(Culture)) return false;
                try { return CultureInfo.GetCultureInfo(Culture).TwoLetterISOLanguageName == "en"; }
                catch (CultureNotFoundException) { return false; }
            }
        }
        internal static bool Setting(string key) => Reflect.Bool(Reflect.Read(GeneralSettings, "Settings"), key);
        internal static string PreviewText(string key) => Reflect.String(PreviewType, key);
        internal static void Initialize() {
            ToyBoxMain = ToyBox.GetType("ToyBox.Main", true);
            GeneralSettings = ToyBox.GetType("ToyBox.Infrastructure.GeneralSettings", true);
            PreviewType = ToyBox.GetType("ToyBox.Features.BagOfTricks.Preview.DialogPreviewUtilities", true);
            GameLocalization = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("Kingmaker.Localization.LocalizationManager")).FirstOrDefault(t => t != null);
            if (GameLocalization == null) throw new TypeLoadException("Kingmaker.Localization.LocalizationManager");
            GameLocaleChanged = GameLocalization.GetMethods(Reflect.Flags).Single(m => m.Name == "OnLocaleProviderValueChanged" && m.GetParameters().Length == 1);
            ClearNameCaches = ToyBox.GetType("ToyBox.Infrastructure.Blueprints.BPHelper", true).GetMethod("ClearNameCaches", Reflect.Flags);
            var ui = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("Kingmaker.UI.Common.UIUtility")).FirstOrDefault(t => t != null);
            if (ui == null) throw new TypeLoadException("Kingmaker.UI.Common.UIUtility");
            ConvictionText = ui.GetMethods(Reflect.Flags).Single(m => m.Name == "GetSoulMarkDirectionText" && m.GetParameters().Length == 1);
            StatText = ui.GetMethods(Reflect.Flags).Single(m => m.Name == "GetStatText" && m.GetParameters().Length == 1);
            if (ClearNameCaches == null) throw new MissingMethodException("BPHelper.ClearNameCaches");
            lastActive = null;
        }
        internal static void RefreshCaches() {
            var active = Active;
            var culture = Culture;
            var gameCulture = GameCulture;
            var descriptions = Setting("ToggleSearchDescriptions");
            var internalNames = Setting("ToggleBPsShowDisplayAndInternalName");
            if (lastActive == active && lastCulture == culture && lastGameCulture == gameCulture && lastDescriptions == descriptions && lastInternalNames == internalNames) return;
            lastActive = active; lastCulture = culture;
            lastGameCulture = gameCulture;
            lastDescriptions = descriptions; lastInternalNames = internalNames;
            // Upstream also invalidates every registered browser, including cached search/sort results.
            ClearNameCaches?.Invoke(null, null);
        }
        internal static string Localize(string key) {
            if (string.IsNullOrEmpty(key)) return key;
            foreach (var method in new[] { ConvictionText, StatText }) {
                if (method == null) continue;
                var type = method.GetParameters()[0].ParameterType;
                if (!Enum.IsDefined(type, key)) continue;
                var result = Reflect.Localized(method.Invoke(null, new[] { Enum.Parse(type, key) }));
                if (!Text.Missing(result) && result != key) return result;
            }
            if (GameCulture == "zhCN" || GameCulture == "enGB") {
                var chinese = GameCulture == "zhCN";
                if (key == "SkillPersuasion") return chinese ? "劝说" : "Persuasion";
                if (key == "SkillAwareness") return chinese ? "警觉" : "Awareness";
            }
            return key;
        }
    }
}
