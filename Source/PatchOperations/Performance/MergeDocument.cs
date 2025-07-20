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
                    ErrorManager.AddError("XmlExtensions.MergeDocument(docName=" + docName + "): No document exists with the given name");
                    return false;
                }
                XmlDocument doc = PatchManager.XmlDocs.Get(docName);
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (PatchManager.XmlDocs.NodeMapContainsKey(docName, node))
                    {
                        XmlNode existingNode = PatchManager.XmlDocs.GetNodeFromNodeMap(docName, node);

                        // Remove child nodes
                        for (int i = existingNode.ChildNodes.Count - 1; i >= 0; i--)
                        {
                            existingNode.RemoveChild(existingNode.ChildNodes[i]);
                        }

                        // Import and append all children from the temp node
                        foreach (XmlNode child in node.ChildNodes)
                        {
                            XmlNode imported = existingNode.OwnerDocument.ImportNode(child, true);
                            existingNode.AppendChild(imported);
                        }

                        PatchManager.XmlDocs.RemoveNodeFromNodeMap(docName, node);
                    }
                    else
                    {
                        // Add the new node
                        PatchManager.XmlDocs.MainDocument.DocumentElement.AppendChild(PatchManager.XmlDocs.MainDocument.ImportNode(node, true));
                    }
                }
                // Remove deleted nodes
                foreach (XmlNode node in PatchManager.XmlDocs.GetNodeMap(docName).Values)
                {
                    XmlNode parentNode = node.ParentNode;
                    if (parentNode != null)
                    {
                        parentNode.RemoveChild(node);
                    }
                }
                PatchManager.XmlDocs.Remove(docName);
                PatchManager.XmlDocs.RemoveNodeMap(docName);
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