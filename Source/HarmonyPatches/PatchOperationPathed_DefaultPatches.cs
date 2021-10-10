using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Verse;

namespace XmlExtensions.Source.HarmonyPatches
{
    [HarmonyPatch]
    static class PatchOperationPathed_DefaultPatches
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (Type T in typeof(PatchOperationPathed).AllSubclassesNonAbstract())
            {
                if (PatchManager.CheckTypePathed(T))
                {
                    yield return AccessTools.Method(T, "ApplyWorker");
                }
            }
        }

        private static Exception Finalizer(PatchOperationPathed __instance, Exception __exception, ref bool __result, string ___xpath, XmlDocument xml)
        {
            if (__exception != null)
            {
                ErrorManager.AddError(__instance.GetType().ToString() + "(xpath=\"" + ___xpath + "\"): " + __exception.Message);
                __result = false;
                return null;
            }
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                {
                    ErrorManager.AddError(__instance.GetType().ToString() + "(xpath=\"" + ___xpath + "\"): Failed to find a node with the given xpath");
                }
                else
                {
                    ErrorManager.AddError(__instance.GetType().ToString() + "(xpath=\"" + ___xpath + "\"): Error");
                }
            }
            return null;
        }
    }
}