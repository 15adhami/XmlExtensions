using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationSafeReplace : PatchOperationExtendedPathed
	{
		private XmlContainer value;

		protected override bool applyWorker(XmlDocument xml)
		{
            try
            {
				XmlNode node = value.node;
				XmlNodeList nodeList = xml.SelectNodes(xpath);
				foreach (XmlNode xmlNode in nodeList)
				{
					XmlNode parentNode = xmlNode.ParentNode;
					foreach (XmlNode childNode in node.ChildNodes)
					{
						parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(childNode, deep: true), xmlNode);
					}
					parentNode.RemoveChild(xmlNode);
				}
				return true;
			}
			catch (Exception e)
			{
				PatchManager.errors.Add("XmlExtensions.PatchOperationSafeReplace(xpath=" + xpath + "): " + e.Message);
				return false;
			}
		}
	}
}
