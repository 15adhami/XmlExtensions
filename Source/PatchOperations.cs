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
        //Make this an optional input (unary operations)
        protected string value = "0";
        protected bool fromXml = false;
        protected string operation = "+";

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
                        if (this.order == PatchOperationAddOrReplace.Order.Append)
                        {
                            xmlNode.AppendChild(xmlNode.OwnerDocument.ImportNode(addNode, true));
                        }
                        if (this.order == PatchOperationAddOrReplace.Order.Prepend)
                        {
                            xmlNode.PrependChild(xmlNode.OwnerDocument.ImportNode(addNode, true));
                        }
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

        protected PatchOperationAddOrReplace.Order order;

        protected enum Order
        {
            
            Append,
            
            Prepend
        }
    }


}