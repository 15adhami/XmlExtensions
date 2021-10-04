using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class PatchOperationAddModExtension : PatchOperationExtendedPathed
    {
        private XmlContainer value;

        protected override bool Patch(XmlDocument xml)
        {
            XmlNode node = value.node;
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
                XmlNode xmlNode2 = xmlNode["modExtensions"];
                if (xmlNode2 == null)
                {
                    xmlNode2 = xmlNode.OwnerDocument.CreateElement("modExtensions");
                    xmlNode.AppendChild(xmlNode2);
                }
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    xmlNode2.AppendChild(xmlNode.OwnerDocument.ImportNode(childNode, true));
                }
                result = true;
            }
            return result;
        }
    }
}
