using System.Collections.Generic;
using System.Xml;

namespace XmlExtensions
{
    internal static class XmlNodeExtensions
    {
        extension(XmlNode node)
        {
            /// <summary>
            /// Replace all attributes and children of the current node with those of the given node.
            /// </summary>
            /// <param name="newNode">The node to replace with</param>
            internal void ReplaceWith(XmlNode newNode)
            {
                XmlElement origElem = (XmlElement)node;
                XmlElement patchedElem = (XmlElement)newNode;
                XmlDocument doc = node.OwnerDocument;

                origElem.RemoveAll();

                if (patchedElem.HasAttributes)
                {
                    foreach (XmlAttribute attr in patchedElem.Attributes)
                    {
                        XmlAttribute importedAttr = (XmlAttribute)doc.ImportNode(attr, true);
                        origElem.Attributes.Append(importedAttr);
                    }
                }

                foreach (XmlNode child in patchedElem.ChildNodes)
                {
                    XmlNode importedChild = doc.ImportNode(child, true);
                    origElem.AppendChild(importedChild);
                }
            }
        }

        extension(XmlElement element)
        {
            /// <summary>
            /// Replace all attributes and children of the current node with those of the given node.
            /// </summary>
            /// <param name="newElement">The node to replace with</param>
            internal void ReplaceWith(XmlElement newElement)
            {
                XmlDocument doc = element.OwnerDocument;
                element.RemoveAll();

                if (newElement.HasAttributes)
                {
                    foreach (XmlAttribute attr in newElement.Attributes)
                    {
                        XmlAttribute importedAttr = (XmlAttribute)doc.ImportNode(attr, true);
                        element.Attributes.Append(importedAttr);
                    }
                }

                foreach (XmlNode child in newElement.ChildNodes)
                {
                    XmlNode importedChild = doc.ImportNode(child, true);
                    element.AppendChild(importedChild);
                }
            }
        }

        extension(XmlNodeList nodeList)
        {
            internal List<XmlNode> ToList()
            {
                List<XmlNode> nodes = [];
                foreach (XmlNode node in nodeList)
                {
                    nodes.Add(node);
                }
                return nodes;
            }
        }
    }
}
