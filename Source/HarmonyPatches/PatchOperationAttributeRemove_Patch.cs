using HarmonyLib;
using System;
using System.Xml;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(Verse.PatchOperationAttributeRemove), "ApplyWorker")]
    internal static class PatchOperationAttributeRemove_Patch
    {
        private static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, ref string ___attribute, XmlDocument xml)
        {
            if (__exception != null)
            {
                ErrorManager.AddError("Verse.PatchOperationAttributeRemove(xpath=" + ___xpath + ", attribute=" + ___attribute + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }

        private static void Postfix(ref bool __result, ref string ___xpath, ref string ___attribute, XmlDocument xml)
        {
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    ErrorManager.AddError("Verse.PatchOperationAttributeRemove(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    ErrorManager.AddError("Verse.PatchOperationAttributeRemove(xpath=" + ___xpath + ", attribute=" + ___attribute + "): Error ");
            }
        }
    }
}