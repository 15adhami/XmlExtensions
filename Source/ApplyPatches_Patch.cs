using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using HarmonyLib;
using System.Xml;
using System.Linq.Expressions;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(LoadedModManager))]
    [HarmonyPatch("ApplyPatches")]
    static class ApplyPatches_Patch
    {
        static void Prefix(XmlDocument xmlDoc, Dictionary<XmlNode, LoadableXmlAsset> assetlookup)
        {
            PatchManager.context = false;
            PatchManager.xmlDoc = xmlDoc;
            PatchManager.defaultDoc = xmlDoc;
            /*
            NewExpression newExp = Expression.New(typeof(AggregateValues).GetConstructor(Type.EmptyTypes));
            LambdaExpression lambda = Expression.Lambda(newExp);
            Delegate compiled = lambda.Compile();
            XmlMod.createPatch = compiled;
            */
        }

        static void Postfix(XmlDocument xmlDoc, Dictionary<XmlNode, LoadableXmlAsset> assetlookup)
        {
            PatchManager.watch.Reset();
            //Add defNames to the menus
            foreach(XmlNode node in xmlDoc.SelectNodes("/Defs/XmlExtensions.SettingsMenuDef"))
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
