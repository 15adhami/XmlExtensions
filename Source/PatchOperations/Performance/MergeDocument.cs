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
                        // Replace the given node
                        XmlNode oldNode = PatchManager.XmlDocs.GetNodeFromNodeMap(docName, node);
                        XmlNode parentNode = oldNode.ParentNode;
                        if (parentNode != null) // Delete all parent nodes, replace with child nodes
                        {
                            parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(node, true), oldNode);
                            parentNode.RemoveChild(oldNode);
                        }
                        PatchManager.XmlDocs.RemoveNodeFromNodeMap(docName, node);
                        /*
                        LoadableXmlAsset value = null;
                        if(PatchManager.Coordinator.assetlookup.TryGetValue(node, out value))
                        {
                            XmlInheritance.TryRegister(node, value?.mod);
                        }*/
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