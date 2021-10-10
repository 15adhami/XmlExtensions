using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class PatchOperationInsert : PatchOperationExtendedPathed
    {
        private enum Order
        {
            Append,
            Prepend
        }

        private XmlContainer value;

        private Order order = Order.Prepend;

        protected override bool Patch(XmlDocument xml)
        {
            XmlNode node = value.node;
            foreach (XmlNode xmlNode in nodes)
            {
                XmlNode parentNode = xmlNode.ParentNode;
                if (order == Order.Append)
                {
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        parentNode.InsertAfter(parentNode.OwnerDocument.ImportNode(childNode, deep: true), xmlNode);
                    }
                }
                else if (order == Order.Prepend)
                {
                    for (int num = node.ChildNodes.Count - 1; num >= 0; num--)
                    {
                        parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(node.ChildNodes[num], deep: true), xmlNode);
                    }
                }
                if (xmlNode.NodeType == XmlNodeType.Text)
                {
                    parentNode.Normalize();
                }
            }
            return true;
        }
    }
}