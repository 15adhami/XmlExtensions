﻿using System;
using System.Xml;
using HarmonyLib;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(Verse.PatchOperationAttributeRemove))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationAttributeRemove_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, ref string ___attribute, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("Verse.PatchOperationAttributeRemove(xpath=" + ___xpath + ", attribute=" + ___attribute + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref string ___xpath, ref string ___attribute, XmlDocument xml)
        {
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("Verse.PatchOperationAttributeRemove(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    PatchManager.errors.Add("Verse.PatchOperationAttributeRemove(xpath=" + ___xpath + ", attribute=" + ___attribute + "): Error ");
            }
        }
    }
}