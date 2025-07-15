using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Verse;

namespace XmlExtensions.Source.HarmonyPatches
{
    [HarmonyPatch]
    internal static class PatchOperationPathed_DefaultPatches
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            Type[] patchTypes =
            {
                typeof(Verse.PatchOperationAdd),
                typeof(Verse.PatchOperationAddModExtension),
                typeof(Verse.PatchOperationInsert),
                typeof(Verse.PatchOperationRemove),
                typeof(Verse.PatchOperationReplace),
            };

            foreach (Type T in patchTypes)
            {
                if (T != null)
                {
                    MethodInfo method = AccessTools.Method(T, "ApplyWorker");
                    if (method != null)
                        yield return method;
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