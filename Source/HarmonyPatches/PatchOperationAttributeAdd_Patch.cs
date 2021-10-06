using System;
using System.Xml;
using HarmonyLib;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(Verse.PatchOperationAttributeAdd))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationAttributeAdd_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, ref string ___attribute, ref string ___value, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("Verse.PatchOperationAttributeAdd(xpath=" + ___xpath + ", attribute=" + ___attribute + ", value=" + ___value + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref string ___xpath, ref string ___attribute, ref string ___value, XmlDocument xml)
        {
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("Verse.PatchOperationAttributeAdd(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    PatchManager.errors.Add("Verse.PatchOperationAttributeAdd(xpath=" + ___xpath + ", attribute=" + ___attribute + ", value=" + ___value + "): Error");
            }
        }
    }
}
