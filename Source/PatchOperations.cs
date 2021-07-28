using System.Xml;
using Verse;

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


}