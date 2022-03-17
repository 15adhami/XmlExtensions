using System.Xml;

namespace XmlExtensions
{
    internal abstract class PatchOperationSafe : PatchOperationExtendedPathed
    {
        public int safetyDepth = -1;
        public bool checkAttributes = false;
        public Compare compare = Compare.Name;

        public enum Compare
        {
            Name,
            InnerText,
            Both
        }

        protected override bool PreCheck(XmlDocument xml)
        {
            if (!base.PreCheck(xml))
            {
                return false;
            }
            if (safetyDepth < 0)
            {
                safetyDepth = 99999;
            }
            return true;
        }

        /// <summary>
        /// Checks if a node contains another node. Checks names and potentially attributes. XmlDocument doesn't matter.
        /// </summary>
        /// <param name="parent">The node whose children you want check</param>
        /// <param name="node">The node that will be checked against</param>
        /// <returns>Whether or not the node contains the given node</returns>
        protected bool ContainsNode(XmlNode parent, XmlNode node)
        {
            XmlNode temp = null;
            return ContainsNode(parent, node, ref temp);
        }

        protected bool ContainsNode(XmlNode parent, XmlNode node, ref XmlNode foundNode)
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