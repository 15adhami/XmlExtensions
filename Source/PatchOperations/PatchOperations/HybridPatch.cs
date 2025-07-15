using System.Xml;
using System.Xml.Linq;
using Verse;
using static RimWorld.IdeoFoundation_Deity;

namespace XmlExtensions
{
    internal class HybridPatch : PatchOperationSafe
    {
        protected XmlContainer value;

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
            XmlAttribute attributeCheckAttributes = child.Attributes["CheckAttributes"];
            if (attributeCheckAttributes != null)
            {
                child.Attributes.Remove(attributeCheckAttributes);
                checkAttributes = bool.Parse(attributeCheckAttributes.InnerText);
            }
            if (attributeOperation == null)
            {
                AddNode(parent, child);
            }
            else if (attributeOperation.InnerText != "SafeRecurse")
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
                    foundNode = parent.OwnerDocument.ImportNode(child, false);
                    parent.AppendChild(foundNode);
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

        protected void AddNode(XmlNode parent, XmlNode child)
        {
            parent.AppendChild(parent.OwnerDocument.ImportNode(child, true));
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

        protected override bool ContainsNode(XmlNode parent, XmlNode node, ref XmlNode foundNode)
        { // TODO: Add XPathLocal
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