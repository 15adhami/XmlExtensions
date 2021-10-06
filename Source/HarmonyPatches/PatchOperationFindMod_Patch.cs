using System;
using System.Collections.Generic;
using System.Xml;
using HarmonyLib;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(PatchOperationFindMod))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationFindMod_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("Verse.PatchOperationFindMod: " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(List<string> ___mods, ref bool __result, XmlDocument xml)
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
                PatchManager.errors.Add("Verse.PatchOperationFindMod(" + ___mods[c]+"): Error in <" + str + ">");
            }
        }
    }
}
