using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    [Obsolete]
    public class ForEachRooted : PatchOperationPathed
    {
        public PatchContainer apply;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                if (apply == null)
                {
                    PatchManager.errors.Add("XmlExtensions.SetRoot(xpath=" + xpath + "): <apply> is null");
                    return false;
                }
                XmlNodeList nodeList = xml.SelectNodes(xpath);

                if (nodeList == null || nodeList.Count == 0)
                {
                    PatchManager.errors.Add("XmlExtensions.SetRoot(xpath=" + xpath + "): Failed to find a node with the given xpath");
                    return false;
                }

                foreach (XmlNode node in nodeList)
                {
                    XmlDocument newDoc = new XmlDocument();
                    XmlNode newNode = newDoc.ImportNode(node.Clone(), true);
                    newDoc.AppendChild(newNode);

                    int errNum = 0;
                    if (!Helpers.RunPatchesInPatchContainer(apply, newDoc, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.SetRoot(xpath=" + xpath + "): Error in the operation at position=" + errNum.ToString());
                        return false;
                    }
                    XmlNode parentNode = node.ParentNode;
                    parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(newNode, true), node);
                    parentNode.RemoveChild(node);

                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.SetRoot(xpath=" + xpath + "): " + e.Message);
                return false;
            }
        }
    }
}
