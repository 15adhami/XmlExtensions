using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationMath : PatchOperationExtendedPathed
    {
        protected string value;
        protected bool fromXml = false;
        protected string operation;

        protected override bool Patch(XmlDocument xml)
        {
            try
            {
                bool result = false;
                XmlNodeList nodeList;
                nodeList = xml.SelectNodes(this.xpath);
                if (nodeList == null || nodeList.Count == 0)
                {
                    XPathError(xpath, "xpath");
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
                        if (node == null)
                        {
                            XPathError(value, "value");
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
            catch (Exception e)
            {
                ExceptionError(e, new string[] { xpath, value }, new string[] { "xpath", "value" }); 
                return false;
            }
        }

    }
}