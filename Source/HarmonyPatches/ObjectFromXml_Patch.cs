using HarmonyLib;
using System;
using System.Reflection;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch]
    internal static class ObjectFromXml_Patch
    {
        static void Postfix()
        {
        }

        static MethodBase TargetMethod()
        {
            return typeof(DirectXmlToObject).GetMethod("ObjectFromXml").MakeGenericMethod(typeof(PatchOperation));
        }
    }
}