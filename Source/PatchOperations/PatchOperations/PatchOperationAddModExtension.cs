using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class PatchOperationAddModExtension : PatchOperationExtendedPathed
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
					PatchManager.errors.Add("XmlExtensions.PatchOperationAddModExtension(xpath=" + xpath + "): Failed to find a node with the given xpath");
					return false;
				}
				foreach (object item in nodeList)
				{
					XmlNode xmlNode = item as XmlNode;
					XmlNode xmlNode2 = xmlNode["modExtensions"];
					if (xmlNode2 == null)
					{
						xmlNode2 = xmlNode.OwnerDocument.CreateElement("modExtensions");
						xmlNode.AppendChild(xmlNode2);
					}
					foreach (XmlNode childNode in node.ChildNodes)
					{
						xmlNode2.AppendChild(xmlNode.OwnerDocument.ImportNode(childNode, true));
					}
					result = true;
				}
				return result;
			}
			catch (Exception e)
			{
				PatchManager.errors.Add("XmlExtensions.PatchOperationAddModExtension(xpath=" + xpath + "): " + e.Message);
				return false;
			}
		}
	}
}
