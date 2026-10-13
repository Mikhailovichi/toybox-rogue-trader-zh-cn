using System;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace ToyBoxChineseDisplayEnhancements {
    public static class Main {
        internal const string Id = "ToyBoxChineseDisplayEnhancements";
        internal static bool Enabled;
        internal static DisplaySettings Settings = new DisplaySettings();
        private static bool available;
        private static string status = "尚未加载";
        private static UnityModManager.ModEntry entry;
        private static Harmony harmony;

        public static bool Load(UnityModManager.ModEntry modEntry) {
            entry = modEntry;
            modEntry.OnToggle = Toggle;
            modEntry.OnGUI = Draw;
            Settings = DisplaySettings.Load(modEntry.Path);
            try {
                var toyBox = UnityModManager.FindMod("0ToyBox0");
                if (toyBox == null || toyBox.Info.Version != "1.7.34" || toyBox.Assembly == null || toyBox.Assembly.GetName().Version != new Version(1, 7, 34, 0))
                    throw new NotSupportedException("Requires loaded ToyBox 1.7.34 (assembly 1.7.34.0).");
                Runtime.ToyBox = toyBox.Assembly;
                Runtime.ToyBoxMain = Runtime.ToyBox.GetType("ToyBox.Main", true);
                Runtime.Mod = Runtime.ToyBox.GetType("ModKit.Mod", true);
                Runtime.LocalizeMethod = Runtime.ToyBox.GetType("ModKit.LocalizationManager", true).GetMethod("localize");
                if (Runtime.LocalizeMethod == null) throw new MissingMethodException("ModKit.LocalizationManager.localize");
                Aliases.Load(modEntry.Path);
                StoryAliases.Load(modEntry.Path);
                harmony = new Harmony(Id);
                var count = Patches.Install(harmony);
                available = true;
                Enabled = true;
                status = "已安装 " + count + " 个显示补丁；仅在 ToyBox 简体中文界面启用。";
                modEntry.Logger.Log(status);
            } catch (Exception ex) {
                Enabled = false;
                available = false;
                harmony?.UnpatchAll(Id);
                status = "未启用增强，保留原显示：" + ex.GetBaseException().Message;
                modEntry.Logger.Error(status);
            }
            return true;
        }
        private static bool Toggle(UnityModManager.ModEntry modEntry, bool value) {
            if (value && !available) return false;
            Enabled = value;
            return true;
        }
        private static void Draw(UnityModManager.ModEntry modEntry) {
            GUILayout.Label("ToyBox 中文显示增强 0.2 / Chinese Display Enhancements");
            GUILayout.Label(status);
            var details = GUILayout.Toggle(Settings.ShowTechnicalDetails, "显示技术详情 / Show technical details");
            if (details != Settings.ShowTechnicalDetails) {
                Settings.ShowTechnicalDetails = details;
                try { Settings.Save(modEntry.Path); }
                catch (Exception ex) { modEntry.Logger.Error("Could not save display settings: " + ex.Message); }
            }
            GUILayout.Label("预览沿用 ToyBox 的分支收集规则，后续结果可能取决于分支与检定，不保证全部发生。技术详情显示原始摘要、内部标识和来源深度。\nPreviews follow ToyBox's branch traversal; later results depend on branches and checks and are not guaranteed. Technical details show original captions, identifiers and depth.");
        }
        internal static void Warn(Exception ex) => entry?.Logger.Warning("Display fallback: " + ex.GetBaseException().Message);
    }
}
