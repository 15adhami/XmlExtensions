using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationAttributeSet : PatchOperationExtendedPathed
	{
		public string value;
		public string attribute;

		protected override bool applyWorker(XmlDocument xml)
		{
            try
            {
				if (attribute == null)
				{
					PatchManager.errors.Add("XmlExtensions.PatchOperationAttributeSet(xpath=" + xpath + "): Attribute is null");
					return false;
				}
				bool result = false;
				XmlNodeList nodeList = xml.SelectNodes(xpath);
				if (nodeList == null || nodeList.Count == 0)
				{
					PatchManager.errors.Add("XmlExtensions.PatchOperationAttributeSet(xpath=" + xpath + "): Failed to find a node with the given xpath");
					return false;
				}
				foreach (object item in nodeList)
				{
					XmlNode xmlNode = item as XmlNode;
					if (xmlNode.Attributes[attribute] != null)
					{
						xmlNode.Attributes[attribute].Value = value;
					}
					else
					{
						XmlAttribute xmlAttribute = xmlNode.OwnerDocument.CreateAttribute(attribute);
						xmlAttribute.Value = value;
						xmlNode.Attributes.Append(xmlAttribute);
					}
					result = true;
				}
				return result;
			}
			catch (Exception e)
			{
				PatchManager.errors.Add("XmlExtensions.PatchOperationAttributeSet(xpath=" + xpath + ", attribute=" + attribute + "): " + e.Message);
				return false;
			}
		}
	}
}
