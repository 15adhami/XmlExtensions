using System;
using System.Collections.Generic;
using System.Xml;

namespace XmlExtensions
{
    public class CreateDocument : PatchOperationExtendedPathed
    {
        public string docName;

        protected override void SetException()
        {
            exceptionVals = new string[] { docName, xpath };
            exceptionFields = new string[] { "docName", "xpath" };
        }

        protected override bool Patch(XmlDocument xml)
        {
            if (!PatchManager.XmlDocs.ContainsKey(docName))
            {
                PatchManager.nodeMap.Add(docName, new Dictionary<XmlNode, XmlNode>());
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateNode(XmlNodeType.Element, null, docName, null));
                foreach (XmlNode node in nodes)
                {
                    XmlNode newNode = doc.ImportNode(node, true);
                    doc.DocumentElement.AppendChild(newNode);
                    PatchManager.nodeMap[docName].Add(newNode, node);
                }
                PatchManager.XmlDocs.Add(docName, doc);
            }
            else
            {
                XmlDocument doc = PatchManager.XmlDocs[docName];
                foreach (XmlNode node in nodes)
                {
                    XmlNode newNode = doc.ImportNode(node, true);
                    doc.DocumentElement.AppendChild(newNode);
                    PatchManager.nodeMap[docName].Add(newNode, node);
                }
                PatchManager.XmlDocs.Add(docName, doc);
            }
            return true;
        }
    }
}
