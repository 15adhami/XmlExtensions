using System.Xml;

namespace XmlExtensions
{
    internal class PatchOperationAttributeSet : PatchOperationExtendedAttribute
    {
        public string value;

        protected override bool Patch(XmlDocument xml)
        {
            if (value == null)
            {
                Error("<value> is null");
                return false;
            }
            foreach (XmlNode xmlNode in nodes)
            {
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
            }
            return true;
        }
    }
}