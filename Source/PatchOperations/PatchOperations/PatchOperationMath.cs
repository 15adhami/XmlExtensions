using System.Xml;

namespace XmlExtensions
{
    internal class PatchOperationMath : PatchOperationExtendedPathed
    {
        protected string value;
        protected bool fromXml = false;
        protected string operation;

        protected override bool PreCheck(XmlDocument xml)
        {
            if (!base.PreCheck(xml)) { return false; }
            if (value == null)
            {
                Error("<value> is null");
                return false;
            }
            if (operation == null)
            {
                Error("<operation> is null");
                return false;
            }
            return true;
        }

        protected override bool Patch(XmlDocument xml)
        {
            foreach (XmlNode xmlNode in nodes)
            {
                XmlNode parentNode = xmlNode.ParentNode;
                XmlNode node2 = null;
                string valueStored = "";
                if (fromXml)
                {
                    XmlNode node = xml.SelectSingleNode(value);
                    if (node == null)
                    {
                        XPathError("value");
                        return false;
                    }
                    valueStored = node.InnerText;
                }
                else
                {
                    valueStored = value;
                }
                node2 = xmlNode.Clone();
                node2.InnerText = Helpers.OperationOnString(xmlNode.InnerText, valueStored, operation);
                parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(node2, true), xmlNode);
                parentNode.RemoveChild(xmlNode);
            }
            return true;
        }

        protected override void SetException()
        {
            CreateExceptions(xpath, "xpath", value, "value");
        }
    }
}