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
            XmlNode foundNode = null;
            if (ContainsNode(parent, child, ref foundNode))
            {
                if (depth < safetyDepth)
                {
                    if (child.HasChildNodes && child.FirstChild.HasChildNodes)
                    {
                        foreach (XmlNode newChild in child.ChildNodes)
                        {
                            tryReplaceNode(foundNode, newChild, depth + 1);
                        }
                    }
                    else
                    {
                        XmlNode xmlNode = foundNode;
                        parent.InsertBefore(parent.OwnerDocument.ImportNode(child, true), xmlNode);
                        parent.RemoveChild(xmlNode);
                    }
                }
                else
                {
                    XmlNode xmlNode = foundNode;
                    parent.InsertBefore(parent.OwnerDocument.ImportNode(child, true), xmlNode);
                    parent.RemoveChild(xmlNode);
                }
            }
        }
    }
}