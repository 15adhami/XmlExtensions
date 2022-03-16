using System.Xml;

namespace XmlExtensions
{
    internal abstract class PatchOperationSafe : PatchOperationExtendedPathed
    {
        public int safetyDepth = -1;
        public bool checkAttributes = false;
        public Mode mode = Mode.Name;

        public enum Mode
        {
            Name,
            InnerText
        }

        /// <summary>
        /// Checks if a node contains another node. Checks names and potentially attributes. XmlDocument doesn't matter.
        /// </summary>
        /// <param name="parent">The node whose children you want check</param>
        /// <param name="node">The node that will be checked against</param>
        /// <returns>Whether or not the node contains the given node</returns>
        protected bool ContainsNode(XmlNode parent, XmlNode node)
        {
            XmlAttributeCollection attrs = node.Attributes;
            foreach (XmlNode childNode in parent.ChildNodes)
            {
                if ((childNode.Name == node.Name && mode == Mode.Name) || (childNode.InnerText == node.InnerText && mode == Mode.InnerText))
                {
                    if (!checkAttributes)
                    {
                        return true;
                    }
                    XmlAttributeCollection attrsChild = childNode.Attributes;
                    if (attrs == null && attrsChild == null)
                    {
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
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}