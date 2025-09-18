using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(Verse.PatchOperationAttributeSet), "ApplyWorker")]
    internal static class PatchOperationAttributeSet_Patch
    {
        private static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, ref string ___value, ref string ___attribute, XmlDocument xml)
        {
            if (__exception != null)
            {
                ErrorManager.AddError("Verse.PatchOperationAttributeSet(xpath=" + ___xpath + ", attribute=" + ___attribute + ", value=" + ___value + "): " + __exception.Message);
                __result = false;
            }
            else if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    ErrorManager.AddError("Verse.PatchOperationAttributeSet(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    ErrorManager.AddError("Verse.PatchOperationAttributeSet(xpath=" + ___xpath + ", attribute=" + ___attribute + ", value=" + ___value + "): Error");
            }
            return null;
        }
    }
}