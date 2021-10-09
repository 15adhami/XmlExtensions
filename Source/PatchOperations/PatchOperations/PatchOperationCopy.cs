using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationCopy : PatchOperationExtendedPathed
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
                    parent.AppendChild(parent.OwnerDocument.ImportNode(node, true));
                }
            }
            return true;
        }

        protected override void SetException()
        {
            CreateExceptions(xpath, "xpath", paste, "paste");
        }
    }
}