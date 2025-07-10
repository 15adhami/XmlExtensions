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
            PatchManager.XmlDocs.MainDocument = xmlDoc;
            PatchManager.XmlDocs.Add("Defs", xmlDoc);
            foreach (ModContentPack mod in ___runningMods)
            {
                foreach (PatchOperation patch in mod.Patches)
                {
                    PatchManager.Coordinator.PatchModDict.Add(patch, mod);
                }
            }
            PatchManager.Coordinator.IsApplyingPatches = true;
            PatchManager.Profiler.globalWatch.Start();
        }

        private static void Postfix(XmlDocument xmlDoc, Dictionary<XmlNode, LoadableXmlAsset> assetlookup)
        {
            PatchManager.Profiler.globalWatch.Stop();
            PatchManager.Coordinator.IsApplyingPatches = false;
            PatchManager.XmlDocs.Clear();
            PatchManager.XmlDocs.ClearNodeMaps();
            PatchManager.Profiler.ResetWatch();
            PatchManager.SetActivePatchingMod(null);

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