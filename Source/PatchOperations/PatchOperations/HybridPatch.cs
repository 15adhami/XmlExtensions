using System.Xml;
using Verse;
using static RimWorld.BaseGen.SymbolResolver_BasePart_Outdoors_Division_Grid;

namespace XmlExtensions
{
    internal class HybridPatch : PatchOperationSafe
    {
        protected XmlContainer value;
        public string xpathLocal;

        protected override bool Patch(XmlDocument xml)
        {
            XmlNode node = value.node;
            foreach (XmlNode xmlNode in nodes)
            {
                foreach (XmlNode addNode in node.ChildNodes)
                {
                    if (!DoHybridPatch(xmlNode, addNode)) { return false; }
                }
            }
            return true;
        }

        private bool DoHybridPatch(XmlNode parent, XmlNode child)
        {
            XmlAttribute attributeOperation = child.Attributes["Operation"];
            string operation = "Add";
            if (attributeOperation != null)
            {
                operation = attributeOperation.InnerText;
            }
            XmlAttribute attributeCompare = child.Attributes["Compare"];
            compare = Compare.Name;
            if (attributeCompare != null) 
            { 
                compare = (Compare)Compare.Parse(typeof(Compare), attributeCompare.InnerText);
            }
            XmlAttribute attributeCheckAttributes = child.Attributes["CheckAttributes"];
            checkAttributes = false;
            if (attributeCheckAttributes != null)
            {
                checkAttributes = bool.Parse(attributeCheckAttributes.InnerText);
            }
            XmlAttribute attributeXPathLocal = child.Attributes["XPathLocal"];
            xpathLocal = null;
            if (attributeXPathLocal != null)
            {
                xpathLocal = attributeXPathLocal.InnerText;
            }
            if (attributeOperation == null)
            {
                AddNode(parent, child);
            }
            else if (operation != "Safe")
            {
                if (operation == "Add")
                {
                    AddNode(parent, child);
                }
                else if (operation == "Replace")
                {
                    XmlNode foundNode = null;
                    if (!ContainsNode(parent, child, ref foundNode))
                    {
                        Error("No matching node found for <" + child.Name + "> with Operation=Replace");
                        return false;
                    }
                    ReplaceNode(parent, child, foundNode);
                }
                else if (operation == "Remove")
                {
                    XmlNode foundNode = null;
                    if (!ContainsNode(parent, child, ref foundNode))
                    {
                        Error("No matching node found for <" + child.Name + "> with Operation=Remove");
                        return false;
                    }
                    parent.RemoveChild(foundNode);
                }
                else if (operation == "AddOrReplace")
                {
                    XmlNode foundNode = null;
                    if (!ContainsNode(parent, child, ref foundNode))
                    {
                        AddNode(parent, child);
                    }
                    else
                    {
                        ReplaceNode(parent, child, foundNode);
                    }
                }
                else if (operation == "SafeAdd")
                {
                    XmlNode foundNode = null;
                    if (!ContainsNode(parent, child, ref foundNode))
                    {
                        AddNode(parent, child);
                    }
                }
                else if (operation == "SafeReplace")
                {
                    XmlNode foundNode = null;
                    if (ContainsNode(parent, child, ref foundNode))
                    {
                        ReplaceNode(parent, child, foundNode);
                    }
                }
                else if (operation == "SafeRemove")
                {
                    XmlNode foundNode = null;
                    if (ContainsNode(parent, child, ref foundNode))
                    {
                        parent.RemoveChild(foundNode);
                    }
                }
                else
                {
                    Error("<" + child.Name + "> using invalid Operation: " + operation);
                    return false;
                }
            }
            else
            {
                XmlNode foundNode = null;
                if (!ContainsNode(parent, child, ref foundNode))
                {
                    foundNode = AddNode(parent, child, false);
                }

                if (!SafeRecurse(foundNode, child))
                {
                    Error("Error while recursing node <" + child.Name + ">");
                    return false;
                }
            }
            return true;
        }

        private void ReplaceNode(XmlNode parent, XmlNode child, XmlNode foundNode)
        {
            XmlNode node = parent.OwnerDocument.ImportNode(child, true);
            RemoveOperationAttributes(node);
            parent.InsertAfter(node, foundNode);
            parent.RemoveChild(foundNode);
        }

        private XmlNode AddNode(XmlNode parent, XmlNode child, bool deep = true)
        {
            XmlNode node = parent.OwnerDocument.ImportNode(child, deep);
            RemoveOperationAttributes(node);
            parent.AppendChild(node);
            return node;
        }

        private bool SafeRecurse(XmlNode parent, XmlNode child)
        {
            if (child.HasChildNodes && child.FirstChild.HasChildNodes)
            {
                foreach (XmlNode newChild in child.ChildNodes)
                {
                    if (!DoHybridPatch(parent, newChild)) { return false; }
                }
            }
            return true;
        }

        private void RemoveOperationAttributes(XmlNode node)
        {
            XmlAttribute attributeOperation = node.Attributes["Operation"];
            if (attributeOperation != null)
            {
                node.Attributes.Remove(attributeOperation);
            }
            XmlAttribute attributeCompare = node.Attributes["Compare"];
            if (attributeCompare != null)
            {
                node.Attributes.Remove(attributeCompare);
            }
            XmlAttribute attributeCheckAttributes = node.Attributes["CheckAttributes"];
            if (attributeCheckAttributes != null)
            {
                node.Attributes.Remove(attributeCheckAttributes);
            }
            XmlAttribute attributeXPathLocal = node.Attributes["XPathLocal"];
            if (attributeXPathLocal != null)
            {
                node.Attributes.Remove(attributeXPathLocal);
            }
        }

        protected override bool ContainsNode(XmlNode parent, XmlNode nodeToAdd, ref XmlNode foundNode)
        {
            XmlAttributeCollection attrs = nodeToAdd.Attributes;
            XmlNode nodeToCheck = nodeToAdd;

            if (xpathLocal != null)
            {
                nodeToCheck = nodeToAdd.SelectSingleNode(xpathLocal);
            }

            foreach (XmlNode childNode in parent.ChildNodes)
            {
                XmlNode nodeToCheckParent = childNode;
                if (xpathLocal != null)
                {
                    nodeToCheckParent = childNode.SelectSingleNode(xpathLocal);
                }

                if ((nodeToCheckParent.Name == nodeToCheck.Name && compare == Compare.Name)
                    || (nodeToCheckParent.InnerText == nodeToCheck.InnerText && compare == Compare.InnerText)
                    || (nodeToCheckParent.InnerText == nodeToCheck.InnerText && nodeToCheckParent.Name == nodeToCheck.Name && compare == Compare.Both))
                {
                    if (!checkAttributes)
                    {
                        foundNode = childNode;
                        return true;
                    }

                    // Checking attributes
                    XmlAttributeCollection attrsChild = childNode.Attributes;
                    if (attrs == null && attrsChild == null)
                    {
                        foundNode = childNode;
                        return true;
                    }

                    if (attrs != null && attrsChild != null)
                    {
                        // Filter attributes to ignore
                        static bool IsIgnorable(string name) =>
                            name == "Operation" || name == "Compare" || name == "CheckAttributes" || name == "XPathLocal";

                        int filteredCount = 0;
                        foreach (XmlAttribute attr in attrs)
                        {
                            if (IsIgnorable(attr.Name)) continue;
                            filteredCount++;
                        }

                        if (filteredCount != attrsChild.Count)
                            continue;

                        bool matches = true;
                        foreach (XmlAttribute attr in attrs)
                        {
                            if (IsIgnorable(attr.Name)) continue;

                            XmlNode attrChild = attrsChild.GetNamedItem(attr.Name);
                            if (attrChild == null || attrChild.Value != attr.Value)
                            {
                                matches = false;
                                break;
                            }
                        }

                        if (matches)
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