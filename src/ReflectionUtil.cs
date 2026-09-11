using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SoulDLCRebalance
{
    internal static class R
    {
        internal static readonly BindingFlags Inst = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        internal static readonly BindingFlags Stat = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        internal static Assembly GameAssembly;

        internal static bool BindGameAssembly()
        {
            if (GameAssembly != null) return true;
            GameAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => string.Equals(a.GetName().Name, "Assembly-CSharp", StringComparison.Ordinal));
            return GameAssembly != null;
        }

        internal static Type GameType(string name)
        {
            if (GameAssembly == null) return null;
            Type t = GameAssembly.GetType(name, false);
            if (t != null) return t;
            try { return GameAssembly.GetTypes().FirstOrDefault(x => x != null && x.Name == name); }
            catch (ReflectionTypeLoadException ex) { return ex.Types.FirstOrDefault(x => x != null && x.Name == name); }
        }

        internal static Type AnyType(string name)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    Type t = a.GetType(name, false) ?? a.GetTypes().FirstOrDefault(x => x != null && x.Name == name);
                    if (t != null) return t;
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Type t = ex.Types.FirstOrDefault(x => x != null && x.Name == name);
                    if (t != null) return t;
                }
                catch { }
            }
            return null;
        }

        internal static MethodInfo Method(Type t, string name, bool isStatic, int count)
        {
            if (t == null) return null;
            return t.GetMethods(isStatic ? Stat : Inst).FirstOrDefault(m => m.Name == name && m.GetParameters().Length == count);
        }

        internal static MethodInfo Method(Type t, string name, bool isStatic, Type[] signature)
        {
            return t == null ? null : t.GetMethod(name, isStatic ? Stat : Inst, null, signature, null);
        }

        internal static object Get(object obj, string name)
        {
            if (obj == null) return null;
            for (Type t = obj.GetType(); t != null; t = t.BaseType)
            {
                FieldInfo f = t.GetField(name, Inst); if (f != null) return f.GetValue(obj);
                PropertyInfo p = t.GetProperty(name, Inst); if (p != null && p.CanRead) return p.GetValue(obj, null);
            }
            return null;
        }

        internal static object GetStatic(Type t, string name)
        {
            for (Type c = t; c != null; c = c.BaseType)
            {
                FieldInfo f = c.GetField(name, Stat); if (f != null) return f.GetValue(null);
                PropertyInfo p = c.GetProperty(name, Stat); if (p != null && p.CanRead) return p.GetValue(null, null);
            }
            return null;
        }

        internal static void Set(object obj, string name, object value, bool required)
        {
            if (obj == null) { if (required) throw new ArgumentNullException("obj"); return; }
            for (Type t = obj.GetType(); t != null; t = t.BaseType)
            {
                FieldInfo f = t.GetField(name, Inst);
                if (f != null) { f.SetValue(obj, ConvertTo(value, f.FieldType)); return; }
                PropertyInfo p = t.GetProperty(name, Inst);
                if (p != null && p.CanWrite) { p.SetValue(obj, ConvertTo(value, p.PropertyType), null); return; }
            }
            if (required) throw new MissingMemberException(obj.GetType().FullName, name);
        }

        private static object ConvertTo(object value, Type target)
        {
            if (value == null) return null;
            Type n = Nullable.GetUnderlyingType(target); if (n != null) target = n;
            if (target.IsInstanceOfType(value)) return value;
            if (target.IsEnum) return Enum.Parse(target, value.ToString());
            return Convert.ChangeType(value, target, CultureInfo.InvariantCulture);
        }

        internal static string Id(object obj) { return Get(obj, "id") as string; }
        internal static bool Bool(object obj, string name)
        {
            object v = Get(obj, name);
            try { return v != null && Convert.ToBoolean(v, CultureInfo.InvariantCulture); } catch { return false; }
        }
        internal static bool Eq(float a, float b) { return Math.Abs(a - b) <= 0.001f; }

        internal static float GameResGet(object gr, string key)
        {
            MethodInfo m = Method(gr.GetType(), "Get", false, new[] { typeof(string), typeof(float) });
            if (m == null) throw new MissingMethodException("GameRes.Get(string,float)");
            return Convert.ToSingle(m.Invoke(gr, new object[] { key, 0f }), CultureInfo.InvariantCulture);
        }

        internal static void GameResSet(object gr, string key, float value)
        {
            MethodInfo m = Method(gr.GetType(), "Set", false, new[] { typeof(string), typeof(float) });
            if (m == null) throw new MissingMethodException("GameRes.Set(string,float)");
            m.Invoke(gr, new object[] { key, value });
        }

        internal static object CloneGameRes(object original)
        {
            Type t = original.GetType();
            ConstructorInfo c = t.GetConstructor(Inst, null, new[] { t }, null);
            if (c == null) throw new MissingMethodException("GameRes copy constructor");
            return c.Invoke(new[] { original });
        }

        internal static object Item(string id, int value)
        {
            Type t = GameType("Item");
            ConstructorInfo c = t == null ? null : t.GetConstructor(Inst, null, new[] { typeof(string), typeof(int) }, null);
            if (c == null) throw new MissingMethodException("Item(string,int)");
            return c.Invoke(new object[] { id, value });
        }

        internal static IList NewList(Type listType, object[] values)
        {
            IList list = Activator.CreateInstance(listType) as IList;
            if (list == null) throw new InvalidOperationException("Cannot create list " + listType.FullName);
            for (int i = 0; i < values.Length; i++) list.Add(values[i]);
            return list;
        }

        internal static float SmartFloat(object expr)
        {
            object v = Get(expr, "_simpified_float");
            if (v != null && Bool(expr, "_simplified")) return Convert.ToSingle(v, CultureInfo.InvariantCulture);
            MethodInfo m = Method(expr.GetType(), "EvaluateFloat", false, 2);
            if (m == null) throw new MissingMethodException("SmartExpression.EvaluateFloat");
            return Convert.ToSingle(m.Invoke(expr, new object[] { null, null }), CultureInfo.InvariantCulture);
        }

        internal static object CloneSmartConstant(object original, float value)
        {
            MethodInfo c = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
            object copy = c == null ? null : c.Invoke(original, null);
            if (copy == null) throw new InvalidOperationException("SmartExpression clone failed");
            Set(copy, "_expression", value.ToString("0.###", CultureInfo.InvariantCulture), true);
            Set(copy, "_simpified_float", value, true);
            Set(copy, "_simplified", true, true);
            Set(copy, "_exp", null, false); Set(copy, "_character", null, false); Set(copy, "_wgo", null, false);
            return copy;
        }

        internal static object Player()
        {
            Type t = GameType("MainGame");
            return Get(GetStatic(t, "me"), "player");
        }

        internal static float Gratitude()
        {
            object v = Get(Player(), "gratitude_points");
            if (v == null) throw new MissingMemberException("player.gratitude_points");
            return Convert.ToSingle(v, CultureInfo.InvariantCulture);
        }

        internal static void SetGratitude(float value) { Set(Player(), "gratitude_points", value, true); }

        internal static bool GlobalCraftActive()
        {
            Type t = GameType("GlobalCraftControlGUI");
            object v = t == null ? null : GetStatic(t, "is_global_control_active");
            return v != null && Convert.ToBoolean(v, CultureInfo.InvariantCulture);
        }

        internal static bool SoulsDlcAvailable()
        {
            Type t = GameType("DLCEngine");
            MethodInfo m = Method(t, "IsDLCSoulsAvailable", true, 0);
            if (m == null) return true;
            try { return Convert.ToBoolean(m.Invoke(null, null), CultureInfo.InvariantCulture); } catch { return false; }
        }

        internal static void Patch(string harmonyId, Type owner, MethodInfo target, string prefixName, string postfixName, string finalizerName)
        {
            if (target == null) throw new MissingMethodException("Patch target " + harmonyId);
            Type h = AnyType("HarmonyLib.Harmony"), hm = AnyType("HarmonyLib.HarmonyMethod");
            if (h == null || hm == null) throw new InvalidOperationException("Harmony unavailable");
            object harmony = Activator.CreateInstance(h, new object[] { harmonyId });
            object prefix = HM(hm, owner, prefixName), postfix = HM(hm, owner, postfixName), finalizer = HM(hm, owner, finalizerName);
            MethodInfo patch = h.GetMethods(Inst).FirstOrDefault(m => m.Name == "Patch" && m.GetParameters().Length >= 5 && typeof(MethodBase).IsAssignableFrom(m.GetParameters()[0].ParameterType));
            if (patch == null) throw new MissingMethodException("Harmony.Patch");
            object[] args = new object[patch.GetParameters().Length];
            args[0] = target; args[1] = prefix; args[2] = postfix; args[3] = null; args[4] = finalizer;
            patch.Invoke(harmony, args);
        }

        private static object HM(Type hm, Type owner, string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            MethodInfo m = owner.GetMethod(name, Stat);
            if (m == null) throw new MissingMethodException(owner.FullName, name);
            return Activator.CreateInstance(hm, new object[] { m });
        }
    }
}
