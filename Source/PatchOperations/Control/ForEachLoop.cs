﻿using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class ForEachLoop : PatchOperationExtended
    {
        public string xpath;
        protected XmlContainer apply;
        protected string storeIn = "value";
        protected string brackets = "{}";
        protected List<string> values;
        protected Mode mode = Mode.XPath;

        public enum Mode
        {
            XPath,
            InnerText,
            Node,
            LocalPatch
        }

        protected override void SetException()
        {
            if (values == null)
                CreateExceptions(storeIn, "storeIn", xpath, "xpath");
            else
                CreateExceptions(storeIn, "storeIn");
        }

        protected override bool Patch(XmlDocument xml)
        {
            if (values != null)
            {
                foreach (string value in values)
                {
                    XmlContainer newContainer = Helpers.SubstituteVariableXmlContainer(apply, storeIn, value, brackets);
                    if (!RunPatches(newContainer, xml, false))
                    {
                        Error("Error with value=" + value);
                        return false;
                    }
                }
            }
            else if (mode == Mode.XPath)
            {
                XmlNodeList nodes = xml.SelectNodes(xpath);
                if (nodes.Count == 0)
                {
                    XPathError();
                    return false;
                }
                foreach (XmlNode xmlNode in nodes)
                {
                    string path = xmlNode.GetXPath();
                    XmlContainer newContainer = Helpers.SubstituteVariableXmlContainer(apply, storeIn, path, brackets);
                    if (!RunPatches(newContainer, xml)) { return false; }
                }
            }
            else if (mode == Mode.InnerText)
            {
                XmlNodeList nodes = xml.SelectNodes(xpath);
                if (nodes.Count == 0)
                {
                    XPathError();
                    return false;
                }
                foreach (XmlNode xmlNode in nodes)
                {
                    string innerText = xmlNode.InnerText;
                    XmlContainer newContainer = Helpers.SubstituteVariableXmlContainer(apply, storeIn, innerText, brackets);
                    if (!RunPatches(newContainer, xml)) { return false; }
                }
            }
            else if (mode == Mode.Node)
            {
                XmlNodeList nodes = xml.SelectNodes(xpath);
                if (nodes.Count == 0)
                {
                    XPathError();
                    return false;
                }
                foreach (XmlNode xmlNode in nodes)
                {
                    string outerXml = xmlNode.OuterXml;
                    XmlContainer newContainer = Helpers.SubstituteVariableXmlContainer(apply, storeIn, outerXml, brackets);
                    if (!RunPatches(newContainer, xml)) { return false; }
                }
            }
            else if (mode == Mode.LocalPatch)
            {
                XmlNodeList nodes = xml.SelectNodes(xpath);
                if (nodes.Count == 0)
                {
                    XPathError();
                    return false;
                }

                foreach (XmlNode xmlNode in nodes)
                {
                    XmlDocument tempDoc = new();
                    XmlNode importedNode = tempDoc.ImportNode(xmlNode, true);
                    tempDoc.AppendChild(importedNode);

                    XmlContainer newContainer = Helpers.SubstituteVariableXmlContainer(apply, storeIn, xmlNode.InnerText, brackets);
                    if (!RunPatches(newContainer, tempDoc))
                    {
                        Error("Patch failed on local node:\n" + xmlNode.OuterXml);
                        return false;
                    }

                    xmlNode.RemoveAll();
                    foreach (XmlAttribute attr in importedNode.Attributes)
                    {
                        XmlAttribute newAttr = xml.OwnerDocument.CreateAttribute(attr.Name);
                        newAttr.Value = attr.Value;
                        ((XmlElement)xmlNode).SetAttributeNode(newAttr);
                    }

                    foreach (XmlNode child in importedNode.ChildNodes)
                    {
                        XmlNode newChild = xml.ImportNode(child, true);
                        xmlNode.AppendChild(newChild);
                    }
                }
            }
            return true;
        }
    }
}