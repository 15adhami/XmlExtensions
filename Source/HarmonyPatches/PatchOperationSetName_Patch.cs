using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(Verse.PatchOperationSetName), "ApplyWorker")]
    internal static class PatchOperationSetName_Patch
    {
        private static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, ref string ___name, XmlDocument xml)
        {
            if (__exception != null)
            {
                ErrorManager.AddError("Verse.PatchOperationSetName(xpath=" + ___xpath + ", name=" + ___name + "): " + __exception.Message);
                __result = false;
            }
            else if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    ErrorManager.AddError("Verse.PatchOperationSetName(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    ErrorManager.AddError("Verse.PatchOperationSetName(xpath=" + ___xpath + ", name=" + ___name + "): Error");
            }
            return null;
        }
    }
}