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
                if (!PatchManager.XmlDocs.Contains(docName))
                {
                    ErrorManager.AddError($"XmlExtensions.MergeDocument(docName={docName}): No document exists with the given name");
                    return false;
                }

                XmlDocument doc = PatchManager.XmlDocs.Get(docName);

                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (PatchManager.XmlDocs.NodeMapContainsKey(docName, node))
                    {
                        XmlNode existingNode = PatchManager.XmlDocs.GetNodeFromNodeMap(docName, node);

                        // Clone node into temp doc
                        XmlDocument tempDoc = new();
                        XmlNode importedNode = tempDoc.ImportNode(node, true);
                        tempDoc.AppendChild(importedNode);

                        // Clear original node completely
                        existingNode.RemoveAll();

                        // Restore attributes
                        foreach (XmlAttribute attr in importedNode.Attributes)
                        {
                            XmlAttribute newAttr = xml.CreateAttribute(attr.Name);
                            newAttr.Value = attr.Value;
                            ((XmlElement)existingNode).SetAttributeNode(newAttr);
                        }

                        // Restore child nodes
                        foreach (XmlNode child in importedNode.ChildNodes)
                        {
                            XmlNode newChild = xml.ImportNode(child, true);
                            existingNode.AppendChild(newChild);
                        }

                        PatchManager.XmlDocs.RemoveNodeFromNodeMap(docName, node);
                    }
                    else
                    {
                        // New node: import and add directly
                        XmlNode imported = PatchManager.XmlDocs.MainDocument.ImportNode(node, true);
                        PatchManager.XmlDocs.MainDocument.DocumentElement.AppendChild(imported);
                    }
                }

                // Remove deleted nodes
                foreach (XmlNode leftover in PatchManager.XmlDocs.GetNodeMap(docName).Values)
                {
                    leftover.ParentNode?.RemoveChild(leftover);
                }

                PatchManager.XmlDocs.Remove(docName);
                PatchManager.XmlDocs.RemoveNodeMap(docName);
                return true;
            }
            catch (Exception e)
            {
                ErrorManager.AddError($"XmlExtensions.MergeDocument(docName={docName}): {e.Message}");
                return false;
            }
        }

    }
}