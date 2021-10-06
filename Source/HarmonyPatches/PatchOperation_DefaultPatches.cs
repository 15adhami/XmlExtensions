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
        static bool flag = false;

        static IEnumerable<MethodBase> TargetMethods()
        {
            if (!flag)
                Verse.Log.Message(typeof(PatchOperation).ToString());
            yield return AccessTools.Method(typeof(PatchOperation), "ApplyWorker");
            foreach (Type T in typeof(PatchOperation).AllSubclassesNonAbstract())
            {
                if (PatchManager.CheckType(T))
                {
                    if (!flag)
                        Verse.Log.Message(T.ToString());
                    yield return AccessTools.Method(T, "ApplyWorker");
                }
            }
            flag = true;
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

    
    
    [HarmonyPatch]
    static class PatchOperationPathed_DefaultPatches
    {
        static bool flag = false;

        static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (Type T in typeof(PatchOperationPathed).AllSubclassesNonAbstract())
            {
                if (PatchManager.CheckTypePathed(T))
                {
                    if (!flag)
                        Verse.Log.Message(T.ToString() + " pathed");
                    yield return AccessTools.Method(T, "ApplyWorker");
                }
            }
            flag = true;
        }

        static Exception Finalizer(PatchOperationPathed __instance, Exception __exception, ref bool __result, string ___xpath, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add(__instance.GetType().ToString() + "(xpath=\"" + ___xpath + "\"): " + __exception.Message);
                __result = false;
                return null;
            }
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                {
                    PatchManager.errors.Add(__instance.GetType().ToString() + "(xpath=\"" + ___xpath + "\"): Failed to find a node with the given xpath");
                }
                else
                {
                    PatchManager.errors.Add(__instance.GetType().ToString() + "(xpath=\"" + ___xpath + "\"): Error");
                }
            }
            return null;
        }
    }
}
