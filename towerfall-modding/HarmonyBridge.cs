using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace PuppetMaster
{
    internal class HarmonyBridge
    {
        private object instance;

        private Type HarmonyType { get; set; }

        public static Assembly getHarmony()
        {
            return UtilitiesInternal.GetAssemblyByName("0Harmony");
        }
        public HarmonyBridge(string harmony_id)
        {
            HarmonyType = getHarmony().GetType("HarmonyLib.Harmony");

            /*object[] constructorParameters = new object[1];
            constructorParameters[0] = "com.jakesnake.modloader";
            */
            this.instance = Activator.CreateInstance(HarmonyType, new object[] { harmony_id });

        }

        public void Patch(object method_to_patch, object prefix, object postfix)
        {
            UtilitiesInternal.RunReflectiveCommand(HarmonyType, "Patch", new Type[] { typeof(MethodBase), HarmonyMethod.GetType(), HarmonyMethod.GetType(), HarmonyMethod.GetType(), HarmonyMethod.GetType() }, instance, new object[] { method_to_patch, prefix, postfix, null, null });
        }


    }

    internal class AccessTools
    {
        private static Type getAccessTools()
        {
            return HarmonyBridge.getHarmony().GetType("HarmonyLib.AccessTools");
        }
        public static object Method(Type type, string method_name, Type[] parameter_types, Type[] generic_types)
        {
            return UtilitiesInternal.RunReflectiveCommand(getAccessTools(), "Method", new Type[] { typeof(Type), typeof(string), typeof(Type[]), typeof(Type[]) }, null, new object[] { type, method_name, parameter_types, generic_types });
        }
        public static List<MethodInfo> GetDeclaredMethods(Type type)
        {
            return (List<MethodInfo>) UtilitiesInternal.RunReflectiveCommand(getAccessTools(), "GetDeclaredMethods", new Type[] { typeof(Type) }, null, new object[] { type });

        }
    }

    internal class SymbolExtensions
    {
        private static Type getSymbolExtensions()
        {
            return HarmonyBridge.getHarmony().GetType("HarmonyLib.SymbolExtensions");
        }
        public static object GetMethodInfo(Expression<Action> action)
        {
            return UtilitiesInternal.RunReflectiveCommand(getSymbolExtensions(), "GetMethodInfo",
                new Type[] { typeof(Expression<Action>) }, null, new object[] { action });
        }
    }

    internal class HarmonyMethod
    {
        public static Type GetType()
        {
            return HarmonyBridge.getHarmony().GetType("HarmonyLib.HarmonyMethod");
        }
        public static object New(object MethodInfo)
        {
            return Activator.CreateInstance(GetType(), new object[1] { MethodInfo });
        }
    }
}
