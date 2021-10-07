using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationAttributeAdd : PatchOperationExtendedAttribute
    {
        public string value;

        protected override bool Patch(XmlDocument xml)
        {
            if (value == null)
            {
                Error("<value> is null");
                return false;
            }
            foreach (object item in nodes)
            {
                XmlNode xmlNode = item as XmlNode;
                if (xmlNode.Attributes[attribute] == null)
                {
                    XmlAttribute xmlAttribute = xmlNode.OwnerDocument.CreateAttribute(attribute);
                    xmlAttribute.Value = value;
                    xmlNode.Attributes.Append(xmlAttribute);
                }
            }
            return true;
        }
    }
}
