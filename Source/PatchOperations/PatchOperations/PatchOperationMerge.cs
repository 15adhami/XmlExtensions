using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class PatchOperationMerge : PatchOperationSafe
    {
        protected XmlContainer value;

        protected override bool Patch(XmlDocument xml)
        {
            XmlNode node = value.node;
            foreach (XmlNode xmlNode in nodes)
            {
                foreach (XmlNode addNode in node.ChildNodes)
                {
                    TryMergeNode(xmlNode, addNode);
                }
            }
            return true;
        }

        private void TryMergeNode(XmlNode parent, XmlNode child)
        {
            XmlAttribute attributeOperation = child.Attributes["Operation"];
            if (attributeOperation == null || attributeOperation.InnerText == "AddOrReplace")
            {
                XmlNode foundNode = null;
                if (!ContainsNode(parent, child, ref foundNode))
                {
                    parent.AppendChild(parent.OwnerDocument.ImportNode(child, true));
                }
                else
                {
                    parent.InsertAfter(parent.OwnerDocument.ImportNode(child, true), foundNode);
                    parent.RemoveChild(foundNode);
                }
            }
            else if (attributeOperation.InnerText == "Add")
            {
                parent.AppendChild(parent.OwnerDocument.ImportNode(child, true));
            }
            else if (attributeOperation.InnerText == "Replace")
            {
                XmlNode foundNode = null;
                ContainsNode(parent, child, ref foundNode);
                parent.InsertAfter(parent.OwnerDocument.ImportNode(child, true), foundNode);
                parent.RemoveChild(foundNode);
            }
        }

        /// <summary>
        /// Checks if a node contains another node. Checks names and potentially attributes. XmlDocument doesn't matter.
        /// </summary>
        /// <param name="parent">The node whose children you want check</param>
        /// <param name="node">The node that will be checked against</param>
        /// <returns>Whether or not the node contains the given node</returns>
        protected bool ContainsNodeMerge(XmlNode parent, XmlNode node)
        {
            XmlNode temp = null;
            return ContainsNode(parent, node, ref temp);
        }

        protected bool ContainsNodeMerge(XmlNode parent, XmlNode node, ref XmlNode foundNode)
        {
            XmlAttributeCollection attrs = node.Attributes;
            foreach (XmlNode childNode in parent.ChildNodes)
            {
                if ((childNode.Name == node.Name && (compare == Compare.Name || (node.HasChildNodes && node.FirstChild.HasChildNodes))) || (childNode.InnerText == node.InnerText && compare == Compare.InnerText) || (childNode.InnerText == node.InnerText && childNode.Name == node.Name && compare == Compare.Both))
                {
                    if (!checkAttributes)
                    {
                        foundNode = childNode;
                        return true;
                    }
                    XmlAttributeCollection attrsChild = childNode.Attributes;
                    if (attrs == null && attrsChild == null)
                    {
                        foundNode = childNode;
                        return true;
                    }
                    if (attrs != null && attrsChild != null && attrs.Count == attrsChild.Count)
                    {
                        bool b = true;
                        foreach (XmlAttribute attr in attrs)
                        {
                            XmlNode attrChild = attrsChild.GetNamedItem(attr.Name);
                            if (attrChild == null)
                            {
                                b = false;
                                break;
                            }
                            if (attrChild.Value != attr.Value)
                            {
                                b = false;
                                break;
                            }
                        }
                        if (b)
                        {
                            foundNode = childNode;
                            return true;
                        }
                    }
                }
            }
            foundNode = null;
            return false;
        }
    }
}