using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class PatchOperationSafeReplace : PatchOperationExtendedPathed
    {
        protected XmlContainer value;
        protected int safetyDepth = -1;

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
            if (Helpers.ContainsNode(parent, child.Name))
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