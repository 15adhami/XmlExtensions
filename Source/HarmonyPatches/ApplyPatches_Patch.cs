using HarmonyLib;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(LoadedModManager), "ApplyPatches")]
    internal static class ApplyPatches_Patch
    {
        private static void Prefix(XmlDocument xmlDoc, Dictionary<XmlNode, LoadableXmlAsset> assetlookup, List<ModContentPack> ___runningMods)
        {
            PatchManager.context = false;
            PatchManager.xmlDoc = xmlDoc;
            PatchManager.defaultDoc = xmlDoc;
            PatchManager.XmlDocs.Add("Defs", xmlDoc);
            foreach (ModContentPack mod in ___runningMods)
            {
                foreach (PatchOperation patch in mod.Patches)
                {
                    PatchManager.PatchModDict.Add(patch, mod);
                }
            }
            if (XmlMod.allSettings.advancedDebugging)
            {
                Verse.Log.Warning("[XML Extensions]: Advanced XML Debugging is enabled");
            }
            PatchManager.applyingPatches = true;
            PatchManager.watch2.Start();
        }

        private static void Postfix(XmlDocument xmlDoc, Dictionary<XmlNode, LoadableXmlAsset> assetlookup)
        {
            PatchManager.watch2.Stop();
            PatchManager.applyingPatches = false;
            PatchManager.XmlDocs.Clear();
            PatchManager.nodeMap.Clear();
            PatchManager.watch.Reset();
            PatchManager.SetActiveMod(null);

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