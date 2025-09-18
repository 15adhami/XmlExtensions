using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(PatchOperationSequence), "ApplyWorker")]
    internal static class PatchOperationSequence_Patch
    {
        private static Exception Finalizer(Exception __exception, PatchOperation __instance, ref bool __result, ref PatchOperation ___lastFailedOperation, ref List<PatchOperation> ___operations, XmlDocument xml)
        {
            if (__exception != null)
            {
                ErrorManager.AddPatchOperationError(__instance, ": " + __exception.Message);
                __result = false;
            }
            else if (!__result)
            {
                int num = 0;
                int c = 0;
                if (___operations != null)
                {
                    foreach (PatchOperation operation in ___operations)
                    {
                        c++;
                        if (operation != null && operation == ___lastFailedOperation && ___lastFailedOperation.GetType() != typeof(PatchOperationTest))
                        {
                            num = c;
                        }
                    }
                    if (___operations.Count > 0 && num != 0)
                        ErrorManager.AddPatchOperationError(__instance, ": Error in the operation at position=" + num.ToString());
                    else
                        ErrorManager.AddPatchOperationError(__instance, ": Operation failed");
                }
                else
                {
                    ErrorManager.AddPatchOperationError(__instance, ": <operations> is null");
                }
            }
            return null;
        }
    }
}