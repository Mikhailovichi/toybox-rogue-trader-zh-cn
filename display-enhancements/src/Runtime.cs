using System;
using System.Reflection;

namespace ToyBoxChineseDisplayEnhancements {
    internal static class Runtime {
        internal static Assembly ToyBox;
        internal static Type ToyBoxMain;
        internal static Type Mod;
        internal static MethodInfo LocalizeMethod;
        internal static bool Active => Main.Enabled && Reflect.Read(ToyBoxMain, "Enabled") is bool enabled && enabled && Culture == "zh-CN";
        internal static string Culture => Reflect.String(Reflect.Read(Mod, "ModKitSettings"), "uiCultureCode");
        internal static bool Setting(string key) => Reflect.Bool(Reflect.Read(ToyBoxMain, "Settings"), key);
        internal static string Localize(string key) {
            if (string.IsNullOrEmpty(key)) return key;
            return (string)LocalizeMethod.Invoke(null, new object[] { key });
        }
    }
}
