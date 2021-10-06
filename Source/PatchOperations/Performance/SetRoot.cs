using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    [Obsolete]
    public class SetRoot : PatchOperationPathed
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
                XmlNode node = xml.SelectSingleNode(xpath);
                if (node == null)
                {
                    PatchManager.errors.Add("XmlExtensions.SetRoot(xpath=" + xpath + "): Failed to find a node with the given xpath");
                    return false;
                }
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
