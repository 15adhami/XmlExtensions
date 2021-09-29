using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationSafeCopy : PatchOperationExtendedPathed
    {
        public string paste;
        protected int safetyDepth = -1;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                XmlNodeList nodeList;
                nodeList = xml.SelectNodes(this.xpath);
                if (nodeList == null || nodeList.Count == 0)
                {
                    PatchManager.errors.Add("XmlExtensions.PatchOperationSafeCopy(xpath=" + xpath + "): Failed to find a node with the given xpath");
                    return false;
                }
                XmlNodeList parents = xml.SelectNodes(paste);
                if (parents == null || nodeList.Count == 0)
                {
                    PatchManager.errors.Add("XmlExtensions.PatchOperationSafeCopy(paste=" + paste + "): Failed to find a node with the given xpath");
                    return false;
                }
                foreach (XmlNode node in nodeList)
                {
                    foreach (XmlNode parent in parents)
                    {
                        int d = 0;
                        tryAddNode(parent, node, d);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.PatchOperationSafeCopy(xpath=" + xpath + ", paste=" + paste + "): " + e.Message);
                return false;
            }
        }
        private void tryAddNode(XmlNode parent, XmlNode child, int depth)
        {
            if (!Helpers.containsNode(parent, child.Name) || depth == safetyDepth)
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