using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using HarmonyLib;
using System.Xml;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(LoadedModManager))]
    [HarmonyPatch("ApplyPatches")]
    static class ApplyPatches_Patch
    {
        static void Prefix(XmlDocument xmlDoc, Dictionary<XmlNode, LoadableXmlAsset> assetlookup, List<ModContentPack> ___runningMods)
        {
            PatchManager.context = false;
            PatchManager.xmlDoc = xmlDoc;
            PatchManager.defaultDoc = xmlDoc;
            PatchManager.XmlDocs.Add("Defs", xmlDoc);
            foreach(ModContentPack mod in ___runningMods)
            {
                foreach (PatchOperation patch in mod.Patches)
                {
                    PatchManager.ModPatchDict.Add(patch, mod);
                }
            }
        }

        static void Postfix(XmlDocument xmlDoc, Dictionary<XmlNode, LoadableXmlAsset> assetlookup)
        {
            PatchManager.XmlDocs.Clear();
            PatchManager.nodeMap.Clear();
            PatchManager.watch.Reset();

            //Add defNames to the menus
            foreach (XmlNode node in xmlDoc.SelectNodes("/Defs/XmlExtensions.SettingsMenuDef"))
            {
                if (node["defName"] == null)
                {
                    node.AppendChild(xmlDoc.CreateNode("element", "defName", null));
                    node["defName"].InnerText = node["modId"].InnerText.Replace('.', '_');
                }
                else
                {
                    node["defName"].InnerText = node["defName"].InnerText.Replace('.', '_');
                }
            }
        }
    }
}
