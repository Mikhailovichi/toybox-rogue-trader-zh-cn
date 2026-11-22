using System;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace ToyBoxDisplayEnhancements {
    public static class Main {
        internal const string Id = "ToyBoxDisplayEnhancements";
        internal static bool Enabled;
        internal static DisplaySettings Settings = new DisplaySettings();
        private static bool available;
        private static string failure;
        private static int patchCount;
        internal static string StatusText => available ? UiText.Get("Ready", patchCount)
            : failure == null ? UiText.Get("NotLoaded") : UiText.Get("Unavailable", failure);
        private static UnityModManager.ModEntry entry;
        private static Harmony harmony;

        public static bool Load(UnityModManager.ModEntry modEntry) {
            entry = modEntry;
            modEntry.OnToggle = Toggle;
            modEntry.OnGUI = Draw;
            Settings = DisplaySettings.Load(modEntry.Path);
            try {
                var toyBox = UnityModManager.FindMod("0ToyBox0");
                if (toyBox == null || toyBox.Info.Version != "2.0.20" || toyBox.Assembly == null || toyBox.Assembly.GetName().Version != new Version(2, 0, 20, 0))
                    throw new NotSupportedException("Requires ToyBox 2.0.20 with its matching assembly.");
                Runtime.ToyBox = toyBox.Assembly;
                Runtime.Initialize();
                Aliases.Load(modEntry.Path);
                StoryAliases.Load(modEntry.Path);
                harmony = new Harmony(Id);
                patchCount = Patches.Install(harmony);
                available = true;
                Enabled = true;
                Runtime.RefreshCaches();
                failure = null;
                modEntry.Logger.Log(StatusText);
            } catch (Exception ex) {
                Enabled = false;
                available = false;
                harmony?.UnpatchAll(Id);
                failure = ex.GetBaseException().Message;
                modEntry.Logger.Error(StatusText);
            }
            return true;
        }
        private static bool Toggle(UnityModManager.ModEntry modEntry, bool value) {
            if (value && !available) return false;
            Enabled = value;
            Runtime.RefreshCaches();
            return true;
        }
        private static void Draw(UnityModManager.ModEntry modEntry) {
            GUILayout.Label("ToyBox Display Enhancements " + modEntry.Info.Version);
            GUILayout.Label(StatusText);
            var details = GUILayout.Toggle(Settings.ShowTechnicalDetails, UiText.Get("TechnicalSetting"));
            if (details != Settings.ShowTechnicalDetails) {
                Settings.ShowTechnicalDetails = details;
                try { Settings.Save(modEntry.Path); }
                catch (Exception ex) { modEntry.Logger.Error("Could not save display settings: " + ex.Message); }
            }
            GUILayout.Label(UiText.Get("PreviewNote"));
        }
        internal static void Warn(Exception ex) => entry?.Logger.Warning("Display fallback: " + ex.GetBaseException().Message);
    }
}
