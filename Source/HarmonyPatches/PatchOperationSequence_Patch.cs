using System;
using System.Collections.Generic;
using System.Xml;
using HarmonyLib;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(PatchOperationSequence))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationSequence_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, XmlDocument xml)
        {
            if (__exception != null)
            {
                ErrorManager.Add("Verse.PatchOperationSequence: " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref PatchOperation ___lastFailedOperation, ref List<PatchOperation> ___operations, XmlDocument xml)
        {
            if (!__result)
            {
                int num = 0;
                int c = 0;
                foreach(PatchOperation operation in ___operations)
                {
                    c++;
                    if (operation == ___lastFailedOperation && ___lastFailedOperation.GetType() != typeof(PatchOperationTest))
                    {
                        num = c;
                    }                    
                }
                if (___operations != null && ___operations.Count > 0 && num != 0)
                    ErrorManager.Add("Verse.PatchOperationSequence: Error in the operation at position=" + num.ToString());
            }
        }
    }
}
