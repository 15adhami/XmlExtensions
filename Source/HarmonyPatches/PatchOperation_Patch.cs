using HarmonyLib;
using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(PatchOperation), "Apply")]
    internal static class PatchOperation_Patch
    {
        private static void Prefix(PatchOperation __instance, ref XmlDocument xml)
        {
            try
            {
                if (ErrorManager.depth == 0 && PatchManager.PatchModDict.ContainsKey(__instance))
                {
                    ModContentPack pack = PatchManager.PatchModDict[__instance];
                    if (pack != null && PatchManager.ActiveMod != pack)
                    {
                        PatchManager.SetActiveMod(pack);
                    }
                }
            }
            catch(Exception e)
            { //TODO: Catch exceptions for DefDatabaseOperations
            }
            ErrorManager.depth += 1;
            if (PatchManager.applyingPatches)
            {
                PatchManager.PatchCount++;
            }
        }

        private static void Postfix(PatchOperation __instance, ref bool __result, XmlDocument xml)
        {
            ErrorManager.depth -= 1;
            if (ErrorManager.depth == 0 && ErrorManager.ErrorCount() > 0 && !__result)
            {
                PatchManager.FailedPatchCount++;
                if (PatchManager.PatchModDict.ContainsKey(__instance))
                {
                    ErrorManager.PrintErrors(__instance.sourceFile, PatchManager.PatchModDict[__instance]);
                }
                else
                {
                    ErrorManager.PrintErrors();
                }
            }
            else if (__result)
            {
                ErrorManager.ClearErrors();
            }
        }
    }
}