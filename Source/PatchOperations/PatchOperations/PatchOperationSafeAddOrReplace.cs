using System.Xml;
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
                    tryAddOrReplaceNode(xmlNode, addNode, d);
                }
            }
            return true;
        }

        private void tryAddOrReplaceNode(XmlNode parent, XmlNode child, int depth)
        {
            if (!ContainsNode(parent, child))
            {
                parent.AppendChild(parent.OwnerDocument.ImportNode(child, true));
            }
            else if (depth == safetyDepth)
            {
                if (!ContainsNode(parent, child))
                {
                    parent.AppendChild(parent.OwnerDocument.ImportNode(child, true));
                }
                else
                {
                    parent.InsertAfter(parent.OwnerDocument.ImportNode(child, true), parent[child.Name]);
                    parent.RemoveChild(parent[child.Name]);
                }
            }
            else
            {
                if (child.HasChildNodes && child.FirstChild.HasChildNodes)
                {
                    foreach (XmlNode newChild in child.ChildNodes)
                    {
                        tryAddOrReplaceNode(parent[child.Name], newChild, depth + 1);
                    }
                }
            }
        }
    }
}