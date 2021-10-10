using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class PatchOperationAddOrReplace : PatchOperationSafe
    {
        public XmlContainer value;

        protected override bool Patch(XmlDocument xml)
        {
            XmlNode node = value.node;
            foreach (XmlNode xmlNode in nodes)
            {
                foreach (XmlNode addNode in node.ChildNodes)
                {
                    if (!ContainsNode(xmlNode, addNode))
                    {
                        xmlNode.AppendChild(xmlNode.OwnerDocument.ImportNode(addNode, true));
                    }
                    else
                    {
                        xmlNode.InsertAfter(xmlNode.OwnerDocument.ImportNode(addNode, true), xmlNode[addNode.Name]);
                        xmlNode.RemoveChild(xmlNode[addNode.Name]);
                    }
                }
            }
            return true;
        }
    }
}