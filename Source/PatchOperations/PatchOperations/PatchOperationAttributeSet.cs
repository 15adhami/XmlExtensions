using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationAttributeSet : PatchOperationExtendedPathed
	{
		public string value;
		public string attribute;

		protected override bool Patch(XmlDocument xml)
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
					XPathError(xpath, "xpath");
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
				ExceptionError(e, new string[] { attribute, xpath }, new string[] { "attribute", "xpath" });
				return false;
			}
		}
	}
}
