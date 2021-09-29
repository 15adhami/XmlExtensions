using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationAttributeAdd : PatchOperationExtendedPathed
	{
		protected string value;
		public string attribute;

		protected override bool applyWorker(XmlDocument xml)
		{
            try
            {
				bool result = false;
				XmlNodeList nodeList = xml.SelectNodes(xpath);
				if (attribute == null)
                {
					PatchManager.errors.Add("XmlExtensions.PatchOperationAttributeAdd(xpath=" + xpath + "): Attribute is null");
					return false;
				}
				if (nodeList == null || nodeList.Count == 0)
				{
					PatchManager.errors.Add("XmlExtensions.PatchOperationAttributeAdd(xpath=" + xpath + "): Failed to find a node with the given xpath");
					return false;
				}
				foreach (object item in nodeList)
				{
					XmlNode xmlNode = item as XmlNode;
					if (xmlNode.Attributes[attribute] == null)
					{
						XmlAttribute xmlAttribute = xmlNode.OwnerDocument.CreateAttribute(attribute);
						xmlAttribute.Value = value;
						xmlNode.Attributes.Append(xmlAttribute);
						result = true;
					}
				}
				return result;
			}
			catch (Exception e)
			{
				PatchManager.errors.Add("XmlExtensions.PatchOperationAttributeAdd(xpath=" + xpath + ", attribute=" + attribute + "): " + e.Message);
				return false;
			}
		}
	}
}
