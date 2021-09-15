using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using HarmonyLib;
using System.Xml;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(LoadedModManager))]
    [HarmonyPatch("ApplyPatches")]
    static class ApplyPatches_Patch
    {
        static void Postfix(XmlDocument xmlDoc, Dictionary<XmlNode, LoadableXmlAsset> assetlookup)
        {
            //Add defNames to the menus
            foreach(XmlNode node in xmlDoc.SelectNodes("/Defs/XmlExtensions.SettingsMenuDef"))
            {
                if (node["defName"] == null)
                {
                    node.AppendChild(xmlDoc.CreateNode("element", "defName", null));
                }
                node["defName"].InnerText = node["modId"].InnerText.Replace('.', '_');
            }            
        }
    }
}
