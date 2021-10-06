using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class PatchOperationSafeReplace : PatchOperationExtendedPathed
    {
        private XmlContainer value;

        protected override bool Patch(XmlDocument xml)
        {
            XmlNode node = value.node;
            foreach (XmlNode xmlNode in nodes)
            {
                XmlNode parentNode = xmlNode.ParentNode;
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(childNode, deep: true), xmlNode);
                }
                parentNode.RemoveChild(xmlNode);
            }
            return true;
        }
    }
}
