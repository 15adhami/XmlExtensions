using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class PatchOperationRemoveDuplicateNodes : PatchOperationExtendedPathed
    {
        public Operation operation = Operation.Remove;
        public Compare compare = Compare.Name;
        public bool checkAttributes = false;

        public enum Compare
        {
            Name,
            InnerText,
            Both
        }

        public enum Operation
        {
            Remove,
            Merge
        }
        protected override bool Patch(XmlDocument xml)
        {
            foreach (XmlNode xmlNode in nodes)
            {
                if (xmlNode.ParentNode != null)
                {
                    XmlNode parentNode = xmlNode.ParentNode;
                    List<XmlNode> nodesToRemove = new();
                    foreach (XmlNode child in parentNode.ChildNodes)
                    {
                        if (child != xmlNode && IsDuplicate(xmlNode, child))
                        {
                            if (operation == Operation.Remove)
                            {
                                nodesToRemove.Add(child);
                            }
                            else if (operation == Operation.Merge)
                            {
                                if (child.HasChildNodes)
                                {
                                    foreach (XmlNode newChild in child.ChildNodes)
                                    {
                                        xmlNode.AppendChild(xmlNode.OwnerDocument.ImportNode(newChild, true));
                                    }
                                }
                                nodesToRemove.Add(child);
                            }
                        }
                    }
                    foreach (XmlNode node in nodesToRemove)
                    {
                        parentNode.RemoveChild(node);
                    }
                }
            }
            return true;
        }

        private bool IsDuplicate(XmlNode node1, XmlNode node2)
        {
            bool duplicate = false;
            if (compare == Compare.Name && node1.Name == node2.Name)
            {
                duplicate = true;
            }
            else if (compare == Compare.InnerText && node1.InnerText == node2.InnerText)
            {
                duplicate = true;
            }
            else if (compare == Compare.Both && (node1.Name == node2.Name) && (node1.InnerText == node2.InnerText))
            {
                duplicate = true;
            }
            if (!duplicate)
            {
                return false;
            }
            if (checkAttributes)
            {
                XmlAttributeCollection attrs2 = node2.Attributes;
                XmlAttributeCollection attrs1 = node1.Attributes;
                if (attrs2 == null && attrs1 == null)
                {
                    return true;
                }
                if (attrs2 != null && attrs1 != null && attrs2.Count == attrs1.Count)
                {
                    bool b = true;
                    foreach (XmlAttribute attr in attrs1)
                    {
                        XmlNode attrChild = attrs2.GetNamedItem(attr.Name);
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
                    return b;
                }
                else
                {
                    return false;
                }
            }
            return duplicate;
        }
    }
}