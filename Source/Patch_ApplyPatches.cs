using System.Collections.Generic;
using System.Xml;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;

namespace XmlExtensions
{

    [HarmonyPatch(typeof(LoadedModManager))]
    [HarmonyPatch("ApplyPatches")]
    class Patch_ApplyPatches
    {
        static void Postfix(XmlDocument xmlDoc, Dictionary<XmlNode, LoadableXmlAsset> assetlookup)
        {
            PatchManager.applyPatches(xmlDoc);
        }
    }

    /*
    [HarmonyPatch(typeof(Dialog_ModSettings))]
    [HarmonyPatch("DoWindowContents")]
    class Patch_DoWindowContents
    {
        static void Prefix(Rect inRect)
        {
            
        }
    }
    */
}
