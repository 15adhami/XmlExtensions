using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationAttributeRemove : PatchOperationExtendedPathed
	{
		public string attribute;

		protected override bool Patch(XmlDocument xml)
		{
            try
            {
				if (attribute == null)
				{
					PatchManager.errors.Add("XmlExtensions.PatchOperationAttributeRemove(xpath=" + xpath + "): Attribute is null");
					return false;
				}
				bool result = false;
				XmlNodeList nodeList = xml.SelectNodes(xpath);
				if (nodeList == null || nodeList.Count == 0)
				{
					PatchManager.errors.Add("XmlExtensions.PatchOperationAttributeRemove(xpath=" + xpath + "): Failed to find a node with the given xpath");
					return false;
				}
				foreach (object item in nodeList)
				{
					XmlNode xmlNode = item as XmlNode;
					if (xmlNode.Attributes[attribute] != null)
					{
						xmlNode.Attributes.Remove(xmlNode.Attributes[attribute]);
						result = true;
					}
				}
				return result;
			}
			catch (Exception e)
			{
				PatchManager.errors.Add("XmlExtensions.PatchOperationAttributeRemove(xpath=" + xpath + ", attribute=" + attribute + "): " + e.Message);
				return false;
			}
		}
	}
}
