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
                    XPathError(xpath, "xpath");
                    return false;
                }
                XmlNodeList parents = xml.SelectNodes(paste);
                if (parents == null || nodeList.Count == 0)
                {
                    XPathError(paste, "paste");
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
                ExceptionError(e, new string[] { xpath, paste }, new string[] { "xpath", "paste" });
                return false;
            }
        }
    }
}