﻿using System.Xml;

namespace XmlExtensions
{
    internal class PatchOperationAttributeRemove : PatchOperationExtendedAttribute
    {
        protected override bool Patch(XmlDocument xml)
        {
            foreach (XmlNode xmlNode in nodes)
            {
                if (xmlNode.Attributes[attribute] != null)
                {
                    xmlNode.Attributes.Remove(xmlNode.Attributes[attribute]);
                }
            }
            return true;
        }
    }
}