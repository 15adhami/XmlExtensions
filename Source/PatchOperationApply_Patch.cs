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
    static class PatchOperationApply_Patch
    {
        static void Prefix(XmlDocument xml)
        {
            PatchManager.depth += 1;
        }

        static void Postfix(PatchOperation __instance, ref bool __result, XmlDocument xml)
        {
            PatchManager.depth -= 1;
            if (PatchManager.depth == 0 && PatchManager.errors.Count > 0 && !__result)
            {
                PatchManager.printError(__instance.sourceFile);
            }
            else if (__result)
            {
                PatchManager.errors.Clear();
            }
        }
    }
}
