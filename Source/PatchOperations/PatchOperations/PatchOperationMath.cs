using System.Xml;

namespace XmlExtensions
{
    internal class PatchOperationMath : PatchOperationExtendedPathed
    {
        protected string value;
        protected bool fromXml = false;
        protected string operation;
        protected RangeEnd rangeMin = null;
        protected RangeEnd rangeMax = null;

        protected class RangeEnd
        {
            public string value;
            public string operation;
            public bool fromXml = false;
        }

        protected override bool PreCheck(XmlDocument xml)
        {
            if (!base.PreCheck(xml)) { return false; }
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
                bool isRange = xmlNode.InnerText.Contains("~");
                if (isRange)
                {
                    string[] strSplitted = xmlNode.InnerText.Split('~');
                    string valueMin = valueStored;
                    string operationMin = operation;
                    if (rangeMin != null)
                    {
                        if (rangeMin.operation != null)
                        {
                            operationMin = rangeMin.operation;
                        }
                        if (rangeMin.value != null)
                        {
                            valueMin = rangeMin.value;
                            if (rangeMin.fromXml)
                            {
                                XmlNode node = xml.SelectSingleNode(valueMin);
                                if (node == null)
                                {
                                    XPathError("valueMin");
                                    return false;
                                }
                                valueMin = node.InnerText;
                            }
                        }
                    }
                    string valueMax = valueStored;
                    string operationMax = operation;
                    if (rangeMax != null)
                    {
                        if (rangeMax.operation != null)
                        {
                            operationMax = rangeMax.operation;
                        }
                        if (rangeMax.value != null)
                        {
                            valueMax = rangeMax.value;
                            if (rangeMax.fromXml)
                            {
                                XmlNode node = xml.SelectSingleNode(valueMax);
                                if (node == null)
                                {
                                    XPathError("valueMax");
                                    return false;
                                }
                                valueMax = node.InnerText;
                            }
                        }
                    }
                    node2.InnerText = Helpers.OperationOnString(strSplitted[0], valueMin, operationMin) + "~" + Helpers.OperationOnString(strSplitted[1], valueMax, operationMax);
                }
                else
                {
                    node2.InnerText = Helpers.OperationOnString(xmlNode.InnerText, valueStored, operation);
                }
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