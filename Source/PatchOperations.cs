﻿using System.Collections.Generic;
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
            foreach (object obj in xml.SelectNodes(this.xpath))
            {
                result = true;
                XmlNode xmlNode = obj as XmlNode;
                XmlNode parentNode = xmlNode.ParentNode;
                XmlNode node2 = null;
                string valueStored = "";
                if (fromXml)
                {
                    valueStored = xml.SelectSingleNode(value).InnerText;
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

            foreach (XmlNode xmlNode in xml.SelectNodes(this.xpath).Cast<XmlNode>().ToArray<XmlNode>())
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

            return result;
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
            bool result = false;
            foreach (XmlNode node in xml.SelectNodes(xpath))
            {
                XmlNode parent = xml.SelectSingleNode(paste);
                if (!childNodes)
                {
                    result = true;
                    parent.AppendChild(parent.OwnerDocument.ImportNode(node, true));
                }                    
                else
                {
                    foreach(XmlNode c in node.ChildNodes)
                    {
                        result = true;
                        parent.AppendChild(parent.OwnerDocument.ImportNode(c, true));
                    }
                }
            }
            return result;
        }
    }

    public class PatchOperationSafeRemove : PatchOperationPathed
    {
        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool result = false;
            try
            {
                result = true;
                foreach (XmlNode xmlNode in xml.SelectNodes(this.xpath).Cast<XmlNode>().ToArray<XmlNode>())
                {
                    xmlNode.ParentNode.RemoveChild(xmlNode);
                }
            }
            catch
            {
            }
            return result;
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
}