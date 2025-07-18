﻿using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class PatchOperationSafeAddOrReplace : PatchOperationSafe
    {
        protected XmlContainer value;

        protected override bool Patch(XmlDocument xml)
        {
            XmlNode node = value.node;
            foreach (XmlNode xmlNode in nodes)
            {
                foreach (XmlNode addNode in node.ChildNodes)
                {
                    int d = 0;
                    TryAddOrReplaceNode(xmlNode, addNode, d);
                }
            }
            return true;
        }

        private void TryAddOrReplaceNode(XmlNode parent, XmlNode child, int depth)
        {
            XmlNode foundNode = null;
            if (!ContainsNode(parent, child, ref foundNode))
            {
                parent.AppendChild(parent.OwnerDocument.ImportNode(child, true));
            }
            else if (depth >= safetyDepth)
            {
                parent.InsertAfter(parent.OwnerDocument.ImportNode(child, true), foundNode);
                parent.RemoveChild(foundNode);
            }
            else
            {
                if (child.HasChildNodes && child.FirstChild.HasChildNodes)
                {
                    foreach (XmlNode newChild in child.ChildNodes)
                    {
                        TryAddOrReplaceNode(foundNode, newChild, depth + 1);
                    }
                }
                else
                {
                    parent.InsertAfter(parent.OwnerDocument.ImportNode(child, true), foundNode);
                    parent.RemoveChild(foundNode);
                }
            }
        }
    }
}