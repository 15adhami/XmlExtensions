using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using HarmonyLib;

namespace XmlExtensions
{

    [HarmonyPatch(typeof(Verse.PatchOperationSetName))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationSetName_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, ref string ___name, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("Verse.PatchOperationSetName(xpath=" + ___xpath + ", name=" + ___name + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref string ___xpath, ref string ___name, XmlDocument xml)
        {
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("Verse.PatchOperationSetName(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    PatchManager.errors.Add("Verse.PatchOperationSetName(xpath=" + ___xpath + ", name=" + ___name + "): Error");
            }
        }
    }
}
