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

        static IEnumerable<MethodBase> TargetMethods()
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

        static Exception Finalizer(PatchOperation __instance, Exception __exception, ref bool __result, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add(__instance.GetType().ToString() + ": " + __exception.Message);
                __result = false;
                return null;
            }
            if (!__result)
            {
                PatchManager.errors.Add(__instance.GetType().ToString() + ": Error");
            }
            return null;

        }
    }
}
