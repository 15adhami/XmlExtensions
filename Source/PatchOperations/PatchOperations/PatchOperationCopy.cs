using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationCopy : PatchOperationExtendedPathed
    {
        public string paste;

        protected override bool Patch(XmlDocument xml)
        {
            try
            {
                XmlNodeList nodeList;
                nodeList = xml.SelectNodes(this.xpath);
                if (nodeList == null || nodeList.Count == 0)
                {
                    PatchManager.errors.Add("XmlExtensions.PatchOperationCopy(xpath=" + xpath + "): Failed to find a node with the given xpath");
                    return false;
                }
                XmlNodeList parents = xml.SelectNodes(paste);
                if (parents == null || nodeList.Count == 0)
                {
                    PatchManager.errors.Add("XmlExtensions.PatchOperationCopy(paste=" + paste + "): Failed to find a node with the given xpath");
                    return false;
                }
                foreach (XmlNode node in nodeList)
                {
                    foreach(XmlNode parent in parents)
                    {
                        parent.AppendChild(parent.OwnerDocument.ImportNode(node, true));
                    }                    
                }
                return true;
            }            
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.PatchOperationCopy(xpath=" + xpath + ", paste=" + paste + "): " + e.Message);
                return false;
            }
        }
    }
}