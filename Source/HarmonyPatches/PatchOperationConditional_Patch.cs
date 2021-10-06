using System;
using System.Xml;
using HarmonyLib;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(PatchOperationConditional))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationConditional_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("Verse.PatchOperationConditional(xpath=" + ___xpath + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (!__result)
            {
                PatchManager.errors.Add("Verse.PatchOperationConditional(xpath=" + ___xpath + "): Error");
            }
        }
    }
}
