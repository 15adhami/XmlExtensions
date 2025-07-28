// Code by Taranchuk

using HarmonyLib;
using System.Xml;
using Verse;
using static Verse.XmlInheritance;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(XmlInheritance), "CheckForDuplicateNodes")]
    internal static class CheckForDuplicateNodes_Patch
    {
        public static bool Prefix(XmlNode node, XmlNode root)
        {
            CheckForDuplicateNodes(node, root);
            return false;
        }

        private static void CheckForDuplicateNodes(XmlNode node, XmlNode root)
        {
            tempUsedNodeNames.Clear();
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element && !IsListElement(childNode))
                {
                    if (tempUsedNodeNames.Contains(childNode.Name))
                    {
                        var outerNode = GetFirstMatchingNode(node.ChildNodes, childNode);
                        if (!(outerNode is null))
                        {
                            if (outerNode.ChildNodes?[0]?.NodeType == XmlNodeType.Element)
                            {
                                for (var i = 0; i < childNode.ChildNodes.Count; i++)
                                {
                                    outerNode.AppendChild(childNode.ChildNodes[i]);
                                }
                            }
                            node.RemoveChild(childNode);
                        }
                    }
                    else
                    {
                        tempUsedNodeNames.Add(childNode.Name);
                    }
                }
            }
            tempUsedNodeNames.Clear();
            foreach (XmlNode childNode2 in node.ChildNodes)
            {
                if (childNode2.NodeType == XmlNodeType.Element)
                {
                    CheckForDuplicateNodes(childNode2, root);
                }
            }
        }

        public static XmlNode GetFirstMatchingNode(XmlNodeList xmlNodeList, XmlNode childNode)
        {
            for (var i = 0; i < xmlNodeList.Count; i++)
            {
                var node = xmlNodeList[i];
                if (node != childNode && node.Name == childNode.Name)
                {
                    return node;
                }
            }
            return null;
        }
    }
}