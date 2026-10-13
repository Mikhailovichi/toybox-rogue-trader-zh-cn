using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ToyBoxChineseDisplayEnhancements {
    internal static class Reflect {
        internal const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        internal static object Read(object instance, string name) {
            if (instance == null) return null;
            var type = instance as Type ?? instance.GetType();
            var target = instance is Type ? null : instance;
            for (; type != null; type = type.BaseType) {
                var field = type.GetField(name, Flags | BindingFlags.DeclaredOnly);
                if (field != null) return field.GetValue(target);
                var property = type.GetProperty(name, Flags | BindingFlags.DeclaredOnly);
                if (property != null && property.GetIndexParameters().Length == 0) return property.GetValue(target, null);
            }
            return null;
        }
        internal static string String(object instance, string name) => Read(instance, name)?.ToString() ?? "";
        internal static bool Bool(object instance, string name) => Read(instance, name) is bool b && b;
        internal static int Int(object instance, string name) => Convert.ToInt32(Read(instance, name));
        internal static object Resolve(object reference) {
            if (reference == null) return null;
            var get = reference.GetType().GetMethod("Get", Flags, null, Type.EmptyTypes, null);
            return get != null ? get.Invoke(reference, null) : reference;
        }
        internal static IEnumerable<object> Items(object value) => value is IEnumerable list ? list.Cast<object>() : Enumerable.Empty<object>();
        internal static string Caption(object value) {
            if (value == null) return "空条件";
            var method = value.GetType().GetMethod("GetCaption", Flags, null, Type.EmptyTypes, null);
            return (method?.Invoke(value, null) as string) ?? value.GetType().Name;
        }
        internal static string Localized(object value) => value is string s ? s : String(value, "Text");
        internal static string Title(object blueprint) {
            var title = Localized(Read(blueprint, "Title"));
            return Text.Missing(title) ? null : title;
        }
        internal static string InternalName(object blueprint) => String(blueprint, "name");
    }
}
