using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationSafeCopy : PatchOperationSafe
    {
        public string paste;

        protected override bool Patch(XmlDocument xml)
        {
            XmlNodeList parents = xml.SelectNodes(paste);
            if (parents == null || nodes.Count == 0)
            {
                XPathError("paste");
                return false;
            }
            foreach (XmlNode node in nodes)
            {
                foreach (XmlNode parent in parents)
                {
                    int d = 0;
                    tryAddNode(parent, node, d);
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