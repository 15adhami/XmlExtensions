using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationAttributeAdd : PatchOperationExtendedAttribute
    {
        protected string value;

        protected override bool Patch(XmlDocument xml)
        {
            base.Patch(xml);
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
    }
}
