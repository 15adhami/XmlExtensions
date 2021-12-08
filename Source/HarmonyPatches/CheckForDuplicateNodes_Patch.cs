using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(XmlInheritance), "CheckForDuplicateNodes")]
    internal static class CheckForDuplicateNodes_Patch
    {
        private static bool Prefix(XmlNode node, XmlNode root)
        {
			HashSet<string> tempHashSet = ((HashSet<string>)AccessTools.Field(typeof(XmlInheritance), "tempUsedNodeNames").GetValue(null));
			tempHashSet.Clear();
			foreach (XmlNode childNode in node.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Element && !((bool)AccessTools.Method(typeof(XmlInheritance), "IsListElement").Invoke(null, new object[]{ childNode})))
				{
					if (tempHashSet.Contains(childNode.Name))
					{
						if (ErrorManager.bootedWithAdvancedDebugging)
						{
							Verse.Log.Error("XML error: The node <" + childNode.Name + "> appeared twice within <" + node.Name + ">. Def: " + root.Name + " " + Helpers.GetNameFromName(Helpers.GetDefNameFromNode(root)) + ((node != root) ? ("\nWhole XML: " + root.OuterXml) : ""));
							ErrorManager.PrintSusMods(root);
						}
						else
						{
							Verse.Log.Error("XML error: Duplicate XML node name " + childNode.Name + " in this XML block: " + node.OuterXml + ((node != root) ? ("\n\nRoot node: " + root.OuterXml) : ""));
						}
					}
					else
					{
						tempHashSet.Add(childNode.Name);
					}
				}
			}
			tempHashSet.Clear();
			foreach (XmlNode childNode2 in node.ChildNodes)
			{
				if (childNode2.NodeType == XmlNodeType.Element)
				{
					AccessTools.Method(typeof(XmlInheritance), "CheckForDuplicateNodes").Invoke(null, new object[] { childNode2, root });
				}
			}
			return false;
		}
    }
}