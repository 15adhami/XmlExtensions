using System.Xml;
using System.Xml.Linq;
using Verse;
using static RimWorld.IdeoFoundation_Deity;

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
            if (attributeOperation != null) { child.Attributes.Remove(attributeOperation); }
            XmlAttribute attributeCompare = child.Attributes["Compare"];
            if (attributeCompare != null) 
            { 
                child.Attributes.Remove(attributeCompare);
                compare = (Compare)Compare.Parse(typeof(Compare), attributeCompare.InnerText);
            }
            else
            {
                compare = Compare.Name;
            }
            XmlAttribute attributeCheckAttributes = child.Attributes["CheckAttributes"];
            if (attributeCheckAttributes != null)
            {
                child.Attributes.Remove(attributeCheckAttributes);
                checkAttributes = bool.Parse(attributeCheckAttributes.InnerText);
            }
            else
            {
                checkAttributes = false;
            }
            XmlAttribute attributeXPathLocal = child.Attributes["XPathLocal"];
            if (attributeXPathLocal != null)
            {
                child.Attributes.Remove(attributeXPathLocal);
                xpathLocal = attributeXPathLocal.InnerText;
            }
            else
            {
                xpathLocal = null;
            }
            if (attributeOperation == null)
            {
                AddNode(parent, child);
            }
            else if (attributeOperation.InnerText != "Safe")
            {
                if (attributeOperation.InnerText == "Add")
                {
                    AddNode(parent, child);
                }
                else if (attributeOperation.InnerText == "Replace")
                {
                    XmlNode foundNode = null;
                    if (!ContainsNode(parent, child, ref foundNode))
                    {
                        Error("No matching node found for <" + child.Name + "> with Operation=Replace");
                        return false;
                    }
                    ReplaceNode(parent, child, foundNode);
                }
                else if (attributeOperation.InnerText == "Remove")
                {
                    XmlNode foundNode = null;
                    if (!ContainsNode(parent, child, ref foundNode))
                    {
                        Error("No matching node found for <" + child.Name + "> with Operation=Remove");
                        return false;
                    }
                    parent.RemoveChild(foundNode);
                }
                else if (attributeOperation.InnerText == "AddOrReplace")
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
                else if (attributeOperation.InnerText == "SafeAdd")
                {
                    XmlNode foundNode = null;
                    if (!ContainsNode(parent, child, ref foundNode))
                    {
                        AddNode(parent, child);
                    }
                }
                else if (attributeOperation.InnerText == "SafeReplace")
                {
                    XmlNode foundNode = null;
                    if (ContainsNode(parent, child, ref foundNode))
                    {
                        ReplaceNode(parent, child, foundNode);
                    }
                }
                else if (attributeOperation.InnerText == "SafeRemove")
                {
                    XmlNode foundNode = null;
                    if (ContainsNode(parent, child, ref foundNode))
                    {
                        parent.RemoveChild(foundNode);
                    }
                }
                else
                {
                    Error("<" + child.Name + "> using invalid Operation: " + attributeOperation.InnerText);
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

        protected void ReplaceNode(XmlNode parent, XmlNode child, XmlNode foundNode)
        {
            parent.InsertAfter(parent.OwnerDocument.ImportNode(child, true), foundNode);
            parent.RemoveChild(foundNode);
        }

        protected XmlNode AddNode(XmlNode parent, XmlNode child, bool deep = true)
        {
            XmlNode node = parent.OwnerDocument.ImportNode(child, deep);
            parent.AppendChild(node);
            return node;
        }

        protected bool SafeRecurse(XmlNode parent, XmlNode child)
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

        protected override bool ContainsNode(XmlNode parent, XmlNode nodeToAdd, ref XmlNode foundNode)
        {
            try
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
            catch
            {
                Verse.Log.Error("errroror");
                return false;
            }
        }

    }
}