using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(PatchOperationFindMod), "ApplyWorker")]
    internal static class PatchOperationFindMod_Patch
    {
        private static Exception Finalizer(Exception __exception, ref bool __result, XmlDocument xml)
        {
            if (__exception != null)
            {
                ErrorManager.AddError("Verse.PatchOperationFindMod: " + __exception.Message);
                __result = false;
            }
            return null;
        }

        private static void Postfix(List<string> ___mods, ref bool __result, XmlDocument xml)
        {
            int c = 0;
            if (!__result)
            {
                string str = "nomatch";
                for (int i = 0; i < ___mods.Count; i++)
                {
                    if (ModLister.HasActiveModWithName(___mods[i]))
                    {
                        c = i;
                        str = "match";
                        break;
                    }
                }
                ErrorManager.AddError("Verse.PatchOperationFindMod(" + ___mods[c] + "): Error in <" + str + ">");
            }
        }
    }
}