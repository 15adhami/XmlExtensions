﻿using System;
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
            bool result = false;
            XmlNodeList nodeList;
            nodeList = xml.SelectNodes(xpath);
            if (nodeList == null || nodeList.Count == 0)
            {
                XPathError(xpath, "xpath");
                return false;
            }
            foreach (XmlNode xmlNode in nodeList)
            {
                foreach (XmlNode addNode in node.ChildNodes)
                {
                    result = true;
                    if (!Helpers.containsNode(xmlNode, addNode.Name))
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
            if (!result)
            {
                PatchManager.errors.Add("XmlExtensions.PatchOperationAddOrReplace: Error in finding a node in <value>");
                return false;
            }
            return true;
        }
    }
}