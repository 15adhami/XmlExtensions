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
                if (ErrorManager.depth == 0 && PatchManager.Coordinator.PatchModDict.ContainsKey(__instance))
                {
                    ModContentPack pack = PatchManager.Coordinator.PatchModDict[__instance];
                    if (pack != null && PatchManager.Coordinator.ActiveMod != pack)
                    {
                        PatchManager.SetActivePatchingMod(pack);
                    }
                }
            }
            catch(Exception e)
            { //TODO: Catch exceptions for DefDatabaseOperations
            }
            ErrorManager.depth += 1;
            if (PatchManager.Coordinator.IsApplyingPatches)
            {
                PatchManager.Profiler.TotalPatches++;
            }
        }

        private static Exception Finalizer(Exception __exception, PatchOperation __instance, ref bool __result, XmlDocument xml)
        {
            ErrorManager.depth -= 1;
            if (__exception != null)
            {
                ErrorManager.AddPatchOperationError(__instance, ": " + __exception.Message);
                __result = false;
            }
            if (ErrorManager.depth == 0 && ErrorManager.ErrorCount() > 0 && !__result)
            {
                PatchManager.Profiler.FailedPatches++;
                if (PatchManager.Coordinator.PatchModDict.ContainsKey(__instance))
                {
                    ErrorManager.PrintErrors(__instance.sourceFile, PatchManager.Coordinator.PatchModDict[__instance]);
                }
                else
                {
                    ErrorManager.PrintErrors();
                }
            }
            if (__result)
            {
                ErrorManager.ClearErrors();
            }
            return null;
        }
    }
}