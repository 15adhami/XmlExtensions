using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class PatchOperationSafeAdd : PatchOperationSafe
    {
        public XmlContainer value;

        protected override bool Patch(XmlDocument xml)
        {
            XmlNode node = value.node;
            foreach (XmlNode xmlNode in nodes)
            {
                foreach (XmlNode addNode in node.ChildNodes)
                {
                    TryAddNode(xmlNode, addNode, 0);
                }
            }
            return true;
        }

        private void TryAddNode(XmlNode parent, XmlNode addNode, int depth)
        {
            XmlNode foundNode = null;
            if (!ContainsNode(parent, addNode, ref foundNode) || depth >= safetyDepth)
            {
                parent.AppendChild(parent.OwnerDocument.ImportNode(addNode, true));
            }
            else
            {
                if (addNode.HasChildNodes && addNode.FirstChild.HasChildNodes)
                {
                    foreach (XmlNode newChild in addNode.ChildNodes)
                    {
                        TryAddNode(foundNode, newChild, depth + 1);
                    }
                }
            }
        }
    }
}