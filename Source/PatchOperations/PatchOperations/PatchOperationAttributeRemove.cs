using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationAttributeRemove : PatchOperationExtendedAttribute
    {
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
                if (xmlNode.Attributes[attribute] != null)
                {
                    xmlNode.Attributes.Remove(xmlNode.Attributes[attribute]);
                    result = true;
                }
            }
            return result;
        }
    }
}
