using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HarmonyLib;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(PatchOperation))]
    [HarmonyPatch("Apply")]
    static class PatchOperation_Patch
    {
        static void Prefix(ref XmlDocument xml)
        {
            ErrorManager.depth += 1;
        }

        static void Postfix(PatchOperation __instance, ref bool __result, XmlDocument xml)
        {
            ErrorManager.depth -= 1;
            if (ErrorManager.depth == 0 && ErrorManager.ErrorCount() > 0 && !__result)
            {
                ErrorManager.PrintErrors(__instance.sourceFile, PatchManager.ModPatchDict[__instance]);
            }
            else if (__result)
            {
                ErrorManager.ClearErrors();
            }
        }
    }
}
