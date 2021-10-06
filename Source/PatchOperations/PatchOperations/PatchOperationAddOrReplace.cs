using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class PatchOperationAddOrReplace : PatchOperationExtendedPathed
    {
        protected XmlContainer value;

        protected override bool Patch(XmlDocument xml)
        {
            XmlNode node = value.node;
            foreach (XmlNode xmlNode in nodes)
            {
                foreach (XmlNode addNode in node.ChildNodes)
                {
                    if (!Helpers.ContainsNode(xmlNode, addNode.Name))
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