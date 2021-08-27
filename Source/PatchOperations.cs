using System.Collections.Generic;
using System;
using System.Collections;
using System.Xml;
using Verse;
using System.Linq;

namespace XmlExtensions
{
    public class PatchOperationMath : PatchOperationPathed
    {
        protected string value;
        protected bool fromXml = false;
        protected string operation;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool result = false;
            XmlNodeList nodeList = xml.SelectNodes(this.xpath);
            if(nodeList == null || nodeList.Count == 0)
            {
                PatchManager.errors.Add("Error in XmlExtensions.PatchOperationMath in finding a node with xpath: " + xpath);
                return false;
            }
            foreach (XmlNode xmlNode in nodeList)
            {
                result = true;
                XmlNode parentNode = xmlNode.ParentNode;
                XmlNode node2 = null;
                string valueStored = "";
                if (fromXml)
                {
                    XmlNode node = xml.SelectSingleNode(value);
                    if(node == null)
                    {
                        PatchManager.errors.Add("Error in XmlExtensions.PatchOperationMath in finding a node for <value> with xpath: " + value);
                        return false;
                    }
                    valueStored = node.InnerText;
                }
                else
                {
                    valueStored = value;
                }
                node2 = xmlNode.Clone();
                node2.InnerText = Helpers.operationOnString(xmlNode.InnerText, valueStored, this.operation);
                parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(node2, true), xmlNode);
                parentNode.RemoveChild(xmlNode);
            }
            return result;
        }

    }

    public class PatchOperationAddOrReplace : PatchOperationPathed
    {
        protected XmlContainer value;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlNode node = this.value.node;
            bool result = false;
            XmlNodeList nodeList = xml.SelectNodes(xpath);
            if (nodeList == null || nodeList.Count == 0)
            {
                PatchManager.errors.Add("Error in XmlExtensions.PatchOperationAddOrReplace in finding a node with xpath: " + xpath);
                return false;
            }
            foreach (XmlNode xmlNode in nodeList)
            {
                foreach (XmlNode addNode in node.ChildNodes)
                {
                    result = true;
                    if (!Helpers.containsNode(xmlNode, addNode.Name))
                    {
                        xmlNode.AppendChild(xmlNode.OwnerDocument.ImportNode(addNode, true));
                    }
                    else
                    {
                        xmlNode.InsertAfter(xmlNode.OwnerDocument.ImportNode(addNode, true), xmlNode[addNode.Name]);
                        xmlNode.RemoveChild(xmlNode[addNode.Name]);                        
                    }
                }

            }
            if (!result)
            {
                PatchManager.errors.Add("Error in XmlExtensions.PatchOperationAddOrReplace in finding a node in <value>");
                return false;
            }
            return true;
        }
    }

    public class PatchOperationSafeAdd : PatchOperationPathed
    {
        protected XmlContainer value;
        protected int safetyDepth = -1;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlNode node = this.value.node;
            bool result = false;
            XmlNodeList nodeList = xml.SelectNodes(xpath);
            if (nodeList == null || nodeList.Count == 0)
            {
                PatchManager.errors.Add("Error in XmlExtensions.PatchOperationSafeAdd in finding a node with xpath: " + xpath);
                return false;
            }
            foreach (XmlNode xmlNode in nodeList)
            {
                foreach (XmlNode addNode in node.ChildNodes)
                {
                    result = true;
                    int d = 0;
                    tryAddNode(xmlNode, addNode, d);
                }
            }
            if (!result)
            {
                PatchManager.errors.Add("Error in XmlExtensions.PatchOperationSafeAdd in finding a node in <value>");
                return false;
            }
            return result;
        }
        private void tryAddNode(XmlNode parent, XmlNode child, int depth)
        {
            if (!Helpers.containsNode(parent, child.Name) || depth == safetyDepth)
            {
                parent.AppendChild(parent.OwnerDocument.ImportNode(child, true));
            }
            else
            {
                if (child.HasChildNodes && child.FirstChild.HasChildNodes)
                {
                    foreach (XmlNode newChild in child.ChildNodes)
                    {
                        tryAddNode(parent[child.Name], newChild, depth+1);
                    }
                }
            }
        }
    }

    /*
    public class PatchOperationMerge : PatchOperationPathed
    {
        protected XmlContainer value;
        protected int safetyDepth = -1;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlNode node = this.value.node;
            bool result = false;
            foreach (XmlNode xmlNode in xml.SelectNodes(this.xpath))
            {
                foreach (XmlNode addNode in node.ChildNodes)
                {
                    result = true;
                    int d = 0;
                    tryAddNode(xmlNode, addNode, d);
                }
            }
            return result;
        }
        private void tryAddNode(XmlNode parent, XmlNode child, int depth)
        {
            if (!Helpers.containsNode(parent, child.Name) || depth == safetyDepth)
            {
                parent.AppendChild(parent.OwnerDocument.ImportNode(child, true));
            }
            else
            {
                if (child.HasChildNodes && child.FirstChild.HasChildNodes)
                {
                    foreach (XmlNode newChild in child.ChildNodes)
                    {
                        tryAddNode(parent[child.Name], newChild, depth + 1);
                    }
                }
            }
        }
    }*/

    public class PatchOperationCopy : PatchOperationPathed
    {
        public string paste;
        public bool childNodes = false;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlNodeList nodeList = xml.SelectNodes(xpath);
            if(nodeList == null || nodeList.Count == 0)
            {
                PatchManager.errors.Add("Error in XmlExtensions.PatchOperationCopy in finding a node with xpath: " + xpath);
                return false;
            }
            XmlNode parent = xml.SelectSingleNode(paste);
            if (parent == null)
            {
                PatchManager.errors.Add("Error in XmlExtensions.PatchOperationCopy in finding a node for <paste> with xpath: " + paste);
                return false;
            }
            foreach (XmlNode node in nodeList)
            {                
                if (!childNodes)
                {
                    parent.AppendChild(parent.OwnerDocument.ImportNode(node, true));
                }                    
                else
                {
                    foreach(XmlNode c in node.ChildNodes)
                    {
                        parent.AppendChild(parent.OwnerDocument.ImportNode(c, true));
                    }
                }
            }
            return true;
        }
    }

    public class PatchOperationSafeRemove : PatchOperationPathed
    {
        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool result = false;
            try
            {
                foreach (XmlNode xmlNode in xml.SelectNodes(this.xpath).Cast<XmlNode>().ToArray<XmlNode>())
                {
                    result = true;
                    xmlNode.ParentNode.RemoveChild(xmlNode);
                }
            }
            catch
            {
            }
            return true;
        }
    }
    /*
    public class PatchOperationDelay : PatchOperation
    {
        protected PatchOperation patch;
     
        protected override bool ApplyWorker(XmlDocument xml)
        {
            PatchManager.delayedPatches.Enqueue(patch);
            PatchManager.xmlDoc = xml;
            return true;
        }
    }*/

    public class PatchOperationSafeAddOrReplace : PatchOperationPathed
    {
        protected XmlContainer value;
        protected int safetyDepth = -1;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlNode node = this.value.node;
            bool result = false;
            XmlNodeList nodeList = xml.SelectNodes(xpath);
            if (nodeList == null || nodeList.Count == 0)
            {
                PatchManager.errors.Add("Error in XmlExtensions.PatchOperationCopy in finding a node with xpath: " + xpath);
                return false;
            }
            foreach (XmlNode xmlNode in nodeList)
            {
                foreach (XmlNode addNode in node.ChildNodes)
                {
                    result = true;
                    int d = 0;
                    tryAddOrReplaceNode(xmlNode, addNode, d);
                }
            }
            if (!result)
            {
                PatchManager.errors.Add("Error in XmlExtensions.PatchOperationSafeAddOrReplace in finding a node in <value>");
                return false;
            }
            return result;
        }
        private void tryAddOrReplaceNode(XmlNode parent, XmlNode child, int depth)
        {
            if (!Helpers.containsNode(parent, child.Name))
            {
                parent.AppendChild(parent.OwnerDocument.ImportNode(child, true));
            }
            else if (depth == safetyDepth)
            {
                if (!Helpers.containsNode(parent, child.Name))
                {
                    parent.AppendChild(parent.OwnerDocument.ImportNode(child, true));
                }
                else
                {
                    parent.InsertAfter(parent.OwnerDocument.ImportNode(child, true), parent[child.Name]);
                    parent.RemoveChild(parent[child.Name]);
                }
            }
            else
            {
                if (child.HasChildNodes && child.FirstChild.HasChildNodes)
                {
                    foreach (XmlNode newChild in child.ChildNodes)
                    {
                        tryAddOrReplaceNode(parent[child.Name], newChild, depth + 1);
                    }
                }
            }
        }
    }
}