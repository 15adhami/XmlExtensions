using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class StopwatchStart : PatchOperation
    {
        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                PatchManager.watch.Reset();
                PatchManager.watch.Start();
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.StopwatchStart: " + e.Message);
                return false;
            }
        }
    }

    public class StopwatchStop : PatchOperation
    {

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                PatchManager.watch.Stop();
                Verse.Log.Message("XmlExtensions.Stopwatch: " + PatchManager.watch.ElapsedMilliseconds.ToString() + "ms");
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.StopwatchStop: " + e.Message);
                return false;
            }
        }
    }

    public class StopwatchPause : PatchOperation
    {

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                if (PatchManager.watch.IsRunning)
                {
                    PatchManager.watch.Stop();
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.StopwatchStop: " + e.Message);
                return false;
            }
        }
    }

    public class StopwatchResume : PatchOperation
    {
        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                PatchManager.watch.Start();
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.StopwatchStart: " + e.Message);
                return false;
            }
        }
    }

    [Obsolete]
    public class SetRoot : PatchOperationPathed
    {
        public PatchContainer apply;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                if (apply == null)
                {
                    PatchManager.errors.Add("XmlExtensions.SetRoot(xpath=" + xpath + "): <apply> is null");
                    return false;
                }
                XmlNode node = xml.SelectSingleNode(xpath);
                if (node == null)
                {
                    PatchManager.errors.Add("XmlExtensions.SetRoot(xpath=" + xpath + "): Failed to find a node with the given xpath");
                    return false;
                }
                XmlDocument newDoc = new XmlDocument();
                XmlNode newNode = newDoc.ImportNode(node.Clone(), true);
                newDoc.AppendChild(newNode);
                int errNum = 0;
                if (!Helpers.runPatchesInPatchContainer(apply, newDoc, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.SetRoot(xpath=" + xpath + "): Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                XmlNode parentNode = node.ParentNode;
                parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(newNode, true), node);
                parentNode.RemoveChild(node);
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.SetRoot(xpath=" + xpath + "): " + e.Message);
                return false;
            }
        }
    }

    [Obsolete]
    public class ForEachRooted : PatchOperationPathed
    {
        public PatchContainer apply;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                if (apply == null)
                {
                    PatchManager.errors.Add("XmlExtensions.SetRoot(xpath=" + xpath + "): <apply> is null");
                    return false;
                }
                XmlNodeList nodeList = xml.SelectNodes(xpath);

                if (nodeList == null || nodeList.Count == 0)
                {
                    PatchManager.errors.Add("XmlExtensions.SetRoot(xpath=" + xpath + "): Failed to find a node with the given xpath");
                    return false;
                }

                foreach (XmlNode node in nodeList)
                {
                    XmlDocument newDoc = new XmlDocument();
                    XmlNode newNode = newDoc.ImportNode(node.Clone(), true);
                    newDoc.AppendChild(newNode);

                    int errNum = 0;
                    if (!Helpers.runPatchesInPatchContainer(apply, newDoc, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.SetRoot(xpath=" + xpath + "): Error in the operation at position=" + errNum.ToString());
                        return false;
                    }
                    XmlNode parentNode = node.ParentNode;
                    parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(newNode, true), node);
                    parentNode.RemoveChild(node);

                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.SetRoot(xpath=" + xpath + "): " + e.Message);
                return false;
            }
        }
    }

    public class CreateDocument : PatchOperationExtendedPathed
    {
        public string docName;

        protected override bool applyWorker(XmlDocument xml)
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

    public class ApplyInDocument : PatchOperation
    {
        public string docName = "Defs";
        public PatchContainer apply;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                if (apply == null)
                {
                    PatchManager.errors.Add("XmlExtensions.ApplyInDocument(docName=" + docName + "): <apply> is null");
                    return false;
                }
                int errNum = 0;
                XmlDocument doc = PatchManager.XmlDocs[docName];
                if (!Helpers.runPatchesInPatchContainer(apply, doc, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.ApplyInDocument(docName=" + docName + "): Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.ApplyInDocument(docName=" + docName + "): " + e.Message);
                return false;
            }
        }
    }

    public class MergeDocument : PatchOperation
    {
        public string docName;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                XmlDocument doc = PatchManager.XmlDocs[docName];
                foreach(XmlNode node in doc.DocumentElement.ChildNodes)
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
                PatchManager.errors.Add("XmlExtensions.MergeDocument(docName=" + docName + "): " + e.Message);
                return false;
            }
        }
    }
}
