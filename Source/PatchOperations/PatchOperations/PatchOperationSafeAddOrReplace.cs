using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    /*
    public class PatchOperationDelay : PatchOperation
    {
        protected PatchOperation patch;
     
        protected override bool ApplyWorker(XmlDocument xml)
        {
            PatchManager.delayedPatches.Enqueue(patch);
            PatchManager.xmlDoc = xml;
            return true;
        }
    }*/

    public class PatchOperationSafeAddOrReplace : PatchOperationExtendedPathed
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
                    PatchManager.errors.Add("XmlExtensions.PatchOperationSafeAddOrReplace(xpath=" + xpath + "): Failed to find a node with the given xpath");
                    return false;
                }
                foreach (XmlNode xmlNode in nodeList)
                {
                    foreach (XmlNode addNode in node.ChildNodes)
                    {
                        result = true;
                        int d = 0;
                        tryAddOrReplaceNode(xmlNode, addNode, d);
                    }
                }
                if (!result)
                {
                    PatchManager.errors.Add("XmlExtensions.PatchOperationSafeAddOrReplace: Error in finding a node in <value>");
                    return false;
                }
                return result;
            }            
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.PatchOperationSafeAddOrReplace(xpath=" + xpath + "): " + e.Message);
                return false;
            }
        }
        private void tryAddOrReplaceNode(XmlNode parent, XmlNode child, int depth)
        {
            if (!Helpers.containsNode(parent, child.Name))
            {
                parent.AppendChild(parent.OwnerDocument.ImportNode(child, true));
            }
            else if (depth == safetyDepth)
            {
                if (!Helpers.containsNode(parent, child.Name))
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