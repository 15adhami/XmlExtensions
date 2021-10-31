using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class PatchOperationSafeReplace : PatchOperationSafe
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
                    tryReplaceNode(xmlNode, addNode, d);
                }
            }
            return true;
        }

        private void tryReplaceNode(XmlNode parent, XmlNode child, int depth)
        {
            if (ContainsNode(parent, child))
            {
                if (safetyDepth != depth)
                {
                    if (child.HasChildNodes && child.FirstChild.HasChildNodes)
                    {
                        foreach (XmlNode newChild in child.ChildNodes)
                        {
                            tryReplaceNode(parent[child.Name], newChild, depth + 1);
                        }
                    }
                    else
                    {
                        XmlNode xmlNode = parent[child.Name];
                        parent.InsertBefore(parent.OwnerDocument.ImportNode(child, true), xmlNode);
                        parent.RemoveChild(xmlNode);
                    }
                }
                else
                {
                    XmlNode xmlNode = parent[child.Name];
                    parent.InsertBefore(parent.OwnerDocument.ImportNode(child, true), xmlNode);
                    parent.RemoveChild(xmlNode);
                }
            }
        }
    }
}