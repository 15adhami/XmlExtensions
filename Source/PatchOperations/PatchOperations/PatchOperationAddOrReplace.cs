using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class PatchOperationAddOrReplace : PatchOperationSafe
    {
        public XmlContainer value;

        protected override bool Patch(XmlDocument xml)
        {
            XmlNode node = value.node;
            XmlNode foundNode = null;
            foreach (XmlNode xmlNode in nodes)
            {
                foreach (XmlNode addNode in node.ChildNodes)
                {
                    if (!ContainsNode(xmlNode, addNode, ref foundNode))
                    {
                        xmlNode.AppendChild(xmlNode.OwnerDocument.ImportNode(addNode, true));
                    }
                    else
                    {
                        xmlNode.InsertAfter(xmlNode.OwnerDocument.ImportNode(addNode, true), foundNode);
                        xmlNode.RemoveChild(foundNode);
                    }
                }
            }
            return true;
        }
    }
}