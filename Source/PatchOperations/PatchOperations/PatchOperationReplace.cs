using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class PatchOperationReplace : PatchOperationExtendedPathed
	{
		private XmlContainer value;

		protected override bool Patch(XmlDocument xml)
		{
            try
            {
				XmlNode node = value.node;
				bool result = false;
				XmlNodeList nodeList = xml.SelectNodes(xpath);
				if (nodeList == null || nodeList.Count == 0)
				{
					PatchManager.errors.Add("XmlExtensions.PatchOperationReplace(xpath=" + xpath + "): Failed to find a node with the given xpath");
					return false;
				}
				foreach (XmlNode xmlNode in nodeList)
				{
					result = true;
					XmlNode parentNode = xmlNode.ParentNode;
					foreach (XmlNode childNode in node.ChildNodes)
					{
						parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(childNode, deep: true), xmlNode);
					}
					parentNode.RemoveChild(xmlNode);
				}
				return result;
			}
			catch (Exception e)
			{
				PatchManager.errors.Add("XmlExtensions.PatchOperationReplace(xpath=" + xpath + "): " + e.Message);
				return false;
			}
		}
	}
}
