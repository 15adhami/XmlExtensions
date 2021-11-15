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
            return null;
        }

        private static void Postfix(ref bool __result, ref string ___xpath, ref string ___name, XmlDocument xml)
        {
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    ErrorManager.AddError("Verse.PatchOperationSetName(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    ErrorManager.AddError("Verse.PatchOperationSetName(xpath=" + ___xpath + ", name=" + ___name + "): Error");
            }
            else if (XmlMod.allSettings.advancedDebugging)
            {
                foreach (string name in Helpers.GetDefsFromPath(___xpath, xml))
                {
                    if (!PatchManager.DefModDict.ContainsKey(name))
                    {
                        PatchManager.DefModDict.Add(name, new HashSet<ModContentPack>());
                    }
                    if (!PatchManager.DefModDict[name].Contains(PatchManager.ActiveMod))
                    {
                        PatchManager.DefModDict[name].Add(PatchManager.ActiveMod);
                    }
                }
            }
        }
    }
}