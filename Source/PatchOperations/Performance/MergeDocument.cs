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
                        existingNode.ReplaceWith(node);
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