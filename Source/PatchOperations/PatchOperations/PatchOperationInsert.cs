using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class PatchOperationInsert : PatchOperationExtendedPathed
	{
		private enum Order
		{
			Append,
			Prepend
		}

		private XmlContainer value;

		private Order order = Order.Prepend;

		protected override bool Patch(XmlDocument xml)
		{
            try
            {
				XmlNode node = value.node;
				bool result = false;
				XmlNodeList nodeList = xml.SelectNodes(xpath);
				if (nodeList == null || nodeList.Count == 0)
				{
					PatchManager.errors.Add("XmlExtensions.PatchOperationInsert(xpath=" + xpath + "): Failed to find a node with the given xpath");
					return false;
				}
				foreach (object item in nodeList)
				{
					result = true;
					XmlNode xmlNode = item as XmlNode;
					XmlNode parentNode = xmlNode.ParentNode;
					if (order == Order.Append)
					{
						foreach (XmlNode childNode in node.ChildNodes)
						{
							parentNode.InsertAfter(parentNode.OwnerDocument.ImportNode(childNode, deep: true), xmlNode);
						}
					}
					else if (order == Order.Prepend)
					{
						for (int num = node.ChildNodes.Count - 1; num >= 0; num--)
						{
							parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(node.ChildNodes[num], deep: true), xmlNode);
						}
					}
					if (xmlNode.NodeType == XmlNodeType.Text)
					{
						parentNode.Normalize();
					}
				}
				return result;
			}
			catch (Exception e)
			{
				PatchManager.errors.Add("XmlExtensions.PatchOperationInsert(xpath=" + xpath + "): " + e.Message);
				return false;
			}
		}
	}
}
