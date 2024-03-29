﻿using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class PatchOperationAddModExtension : PatchOperationExtendedPathed
    {
        public XmlContainer value;

        protected override bool Patch(XmlDocument xml)
        {
            XmlNode node = value.node;
            foreach (object item in nodes)
            {
                XmlNode xmlNode = item as XmlNode;
                XmlNode xmlNode2 = xmlNode["modExtensions"];
                if (xmlNode2 == null)
                {
                    xmlNode2 = xmlNode.OwnerDocument.CreateElement("modExtensions");
                    xmlNode.AppendChild(xmlNode2);
                }
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    xmlNode2.AppendChild(xmlNode.OwnerDocument.ImportNode(childNode, true));
                }
            }
            return true;
        }
    }
}