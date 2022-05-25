using System.Collections.Generic;
using System.Xml;

namespace XmlExtensions
{
    internal class CreateDocument : PatchOperationExtendedPathed
    {
        public string docName;
        public bool emptyDocument = false;

        protected override bool PreCheck(XmlDocument xml)
        {
            if (!emptyDocument)
            {
                return base.PreCheck(xml);
            }
            else
            {
                return true;
            } 
        }

        protected override void SetException()
        {
            if (!emptyDocument)
            {
                CreateExceptions(docName, "docName", xpath, "xpath");
            }  
            else
            {
                CreateExceptions(docName, "docName");
            }
        }

        protected override bool Patch(XmlDocument xml)
        {
            if (!PatchManager.XmlDocs.ContainsKey(docName))
            {
                PatchManager.nodeMap.Add(docName, new Dictionary<XmlNode, XmlNode>());
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateNode(XmlNodeType.Element, null, docName, null));
                if (!emptyDocument)
                {
                    foreach (XmlNode node in nodes)
                    {
                        XmlNode newNode = doc.ImportNode(node, true);
                        doc.DocumentElement.AppendChild(newNode);
                        PatchManager.nodeMap[docName].Add(newNode, node);
                    }
                }
                PatchManager.XmlDocs.Add(docName, doc);
            }
            else
            {
                XmlDocument doc = PatchManager.XmlDocs[docName];
                if (!emptyDocument)
                {
                    foreach (XmlNode node in nodes)
                    {
                        XmlNode newNode = doc.ImportNode(node, true);
                        doc.DocumentElement.AppendChild(newNode);
                        PatchManager.nodeMap[docName].Add(newNode, node);
                    }
                }
            }
            return true;
        }
    }
}