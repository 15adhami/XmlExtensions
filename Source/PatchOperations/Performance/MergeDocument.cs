using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class MergeDocument : PatchOperation
    {
        public string docName;

        public override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                if (!PatchManager.XmlDocs.ContainsKey(docName))
                {
                    ErrorManager.AddError("XmlExtensions.MergeDocument(docName=" + docName + "): No document exists with the given name");
                    return false;
                }
                XmlDocument doc = PatchManager.XmlDocs[docName];
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (PatchManager.nodeMap[docName].ContainsKey(node))
                    {
                        // Replace the given node
                        XmlNode oldNode = PatchManager.nodeMap[docName][node];
                        XmlNode parentNode = oldNode.ParentNode;
                        if (parentNode != null)
                        {
                            parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(node, true), oldNode);
                            parentNode.RemoveChild(oldNode);
                        }
                        PatchManager.nodeMap[docName].Remove(node);
                    }
                    else
                    {
                        // Add the new node
                        PatchManager.XmlDocs["Defs"].DocumentElement.AppendChild(PatchManager.XmlDocs["Defs"].ImportNode(node, true));
                    }
                }
                // Remove deleted nodes
                foreach (XmlNode node in PatchManager.nodeMap[docName].Values)
                {
                    XmlNode parentNode = node.ParentNode;
                    if (parentNode != null)
                    {
                        parentNode.RemoveChild(node);
                    }
                }
                PatchManager.XmlDocs.Remove(docName);
                PatchManager.nodeMap.Remove(docName);
                return true;
            }
            catch (Exception e)
            {
                ErrorManager.AddError("XmlExtensions.MergeDocument(docName=" + docName + "): " + e.Message);
                return false;
            }
        }
    }
}