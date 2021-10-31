using HarmonyLib;
using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(PatchOperationConditional), "ApplyWorker")]
    internal static class PatchOperationConditional_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (__exception != null)
            {
                ErrorManager.AddError("Verse.PatchOperationConditional(xpath=" + ___xpath + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (!__result)
            {
                ErrorManager.AddError("Verse.PatchOperationConditional(xpath=" + ___xpath + "): Error");
            }
        }
    }
}