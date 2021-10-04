using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class PatchOperationAdd : PatchOperationExtendedPathed
    {
        private enum Order
        {
            Append,
            Prepend
        }

        private XmlContainer value;

        private Order order = Order.Append;

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
                result = true;
                XmlNode xmlNode = item as XmlNode;
                if (order == Order.Append)
                {
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        xmlNode.AppendChild(xmlNode.OwnerDocument.ImportNode(childNode, true));
                    }
                }
                else if (order == Order.Prepend)
                {
                    for (int num = node.ChildNodes.Count - 1; num >= 0; num--)
                    {
                        xmlNode.PrependChild(xmlNode.OwnerDocument.ImportNode(node.ChildNodes[num], true));
                    }
                }
            }
            return result;
        }
    }
}
