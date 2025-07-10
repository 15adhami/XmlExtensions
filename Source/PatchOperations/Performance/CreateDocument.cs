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
            if (!PatchManager.XmlDocs.Contains(docName))
            {
                PatchManager.XmlDocs.RegisterNodeMap(docName);
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateNode(XmlNodeType.Element, null, docName, null));
                if (!emptyDocument)
                {
                    foreach (XmlNode node in nodes)
                    {
                        XmlNode newNode = doc.ImportNode(node, true);
                        doc.DocumentElement.AppendChild(newNode);
                        PatchManager.XmlDocs.AddNodesToMap(docName, newNode, node);
                    }
                }
                PatchManager.XmlDocs.Add(docName, doc);
            }
            else
            {
                XmlDocument doc = PatchManager.XmlDocs.Get(docName);
                if (!emptyDocument)
                {
                    foreach (XmlNode node in nodes)
                    {
                        XmlNode newNode = doc.ImportNode(node, true);
                        doc.DocumentElement.AppendChild(newNode);
                        PatchManager.XmlDocs.AddNodesToMap(docName, newNode, node);
                    }
                }
            }
            return true;
        }
    }
}