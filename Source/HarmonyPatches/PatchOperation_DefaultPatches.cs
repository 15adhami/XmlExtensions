using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Verse;

namespace XmlExtensions.Source.HarmonyPatches
{
    [HarmonyPatch]
    static class PatchOperation_DefaultPatches
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(PatchOperation), "ApplyWorker");
            foreach (Type T in typeof(PatchOperation).AllSubclassesNonAbstract())
            {
                if (PatchManager.CheckType(T))
                {
                    yield return AccessTools.Method(T, "ApplyWorker");
                }
            }
        }

        private static Exception Finalizer(PatchOperation __instance, Exception __exception, ref bool __result, XmlDocument xml)
        {
            if (__exception != null)
            {
                ErrorManager.AddError(__instance.GetType().ToString() + ": " + __exception.Message);
                __result = false;
                return null;
            }
            if (!__result)
            {
                ErrorManager.AddError(__instance.GetType().ToString() + ": Error");
            }
            return null;
        }
    }
}