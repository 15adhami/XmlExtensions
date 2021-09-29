using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class PatchOperationSafeAdd : PatchOperationExtendedPathed
    {
        protected XmlContainer value;
        protected int safetyDepth = -1;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                XmlNode node = this.value.node;
                bool result = false;
                XmlNodeList nodeList;
                nodeList = xml.SelectNodes(this.xpath);
                if (nodeList == null || nodeList.Count == 0)
                {
                    PatchManager.errors.Add("XmlExtensions.PatchOperationSafeAdd(xpath=" + xpath + "): Failed to find a node with the given xpath");
                    return false;
                }
                foreach (XmlNode xmlNode in nodeList)
                {
                    foreach (XmlNode addNode in node.ChildNodes)
                    {
                        result = true;
                        int d = 0;
                        tryAddNode(xmlNode, addNode, d);
                    }
                }
                if (!result)
                {
                    PatchManager.errors.Add("XmlExtensions.PatchOperationSafeAdd: Error in finding a node in <value>");
                    return false;
                }
                return result;
            }            
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.PatchOperationSafeAdd(xpath=" + xpath + "): " + e.Message);
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
                        tryAddNode(parent[child.Name], newChild, depth+1);
                    }
                }
            }
        }
    }
}