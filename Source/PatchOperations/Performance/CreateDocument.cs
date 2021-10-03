using System;
using System.Collections.Generic;
using System.Xml;

namespace XmlExtensions
{
    public class CreateDocument : PatchOperationExtendedPathed
    {
        public string docName;

        protected override bool Patch(XmlDocument xml)
        {
            try
            {
                if(!PatchManager.XmlDocs.ContainsKey(docName))
                {
                    PatchManager.nodeMap.Add(docName, new Dictionary<XmlNode, XmlNode>());
                    XmlNodeList nodeList = xml.SelectNodes(xpath);
                    if (nodeList == null || nodeList.Count == 0)
                    {
                        PatchManager.errors.Add("XmlExtensions.CreateDocument(xpath=" + xpath + "): Failed to find a node with the given xpath");
                        return false;
                    }
                    XmlDocument doc = new XmlDocument();
                    doc.AppendChild(doc.CreateNode(XmlNodeType.Element, null, docName, null));
                    foreach (XmlNode node in nodeList)
                    {
                        XmlNode newNode = doc.ImportNode(node, true);
                        doc.DocumentElement.AppendChild(newNode);
                        PatchManager.nodeMap[docName].Add(newNode, node);
                    }
                    PatchManager.XmlDocs.Add(docName, doc);
                }
                else
                {
                    XmlNodeList nodeList = xml.SelectNodes(xpath);
                    if (nodeList == null || nodeList.Count == 0)
                    {
                        PatchManager.errors.Add("XmlExtensions.CreateDocument(xpath=" + xpath + "): Failed to find a node with the given xpath");
                        return false;
                    }
                    XmlDocument doc = PatchManager.XmlDocs[docName];
                    foreach (XmlNode node in nodeList)
                    {
                        XmlNode newNode = doc.ImportNode(node, true);
                        doc.DocumentElement.AppendChild(newNode);
                        PatchManager.nodeMap[docName].Add(newNode, node);
                    }
                    PatchManager.XmlDocs.Add(docName, doc);
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.CreateDocument(xpath=" + xpath + ", docName=" + docName + "): " + e.Message);
                return false;
            }
        }
    }
}
