using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class PatchOperationSafeAdd : PatchOperationSafe
    {
        public XmlContainer value;

        protected override bool Patch(XmlDocument xml)
        {
            XmlNode node = value.node;
            foreach (XmlNode xmlNode in nodes)
            {
                foreach (XmlNode addNode in node.ChildNodes)
                {
                    int d = 0;
                    tryAddNode(xmlNode, addNode, d);
                }
            }
            return true;
        }
        private void tryAddNode(XmlNode parent, XmlNode child, int depth)
        {
            if (!ContainsNode(parent, child) || depth == safetyDepth)
            {
                parent.AppendChild(parent.OwnerDocument.ImportNode(child, true));
            }
            else
            {
                if (child.HasChildNodes && child.FirstChild.HasChildNodes)
                {
                    foreach (XmlNode newChild in child.ChildNodes)
                    {
                        tryAddNode(parent[child.Name], newChild, depth + 1);
                    }
                }
            }
        }
    }
}