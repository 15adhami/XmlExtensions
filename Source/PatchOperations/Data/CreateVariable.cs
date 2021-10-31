using System.Collections.Generic;
using System.Xml;

namespace XmlExtensions
{
    internal class CreateVariable : PatchOperationValue
    {
        public string value = "";
        public string value2 = "";
        public string defaultValue;
        public string defaultValue2;
        public bool fromXml = false;
        public bool fromXml2 = false;
        public string operation = "";

        protected override void SetException()
        {
            CreateExceptions(storeIn, "storeIn", value, "value", value2, "value2");
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            string val;
            string newStr1 = value;
            string newStr2 = value2;
            if (fromXml)
            {
                XmlNode node = xml.SelectSingleNode(value);
                if (node == null)
                {
                    if (defaultValue == null)
                    {
                        XPathError("value");
                        return false;
                    }
                    newStr1 = defaultValue;
                }
                else
                {
                    newStr1 = node.InnerXml;
                }
            }
            if (fromXml2)
            {
                XmlNode node = xml.SelectSingleNode(value2);
                if (node == null)
                {
                    if (defaultValue2 == null)
                    {
                        XPathError("value2");
                        return false;
                    }
                    newStr2 = defaultValue2;
                }
                else
                {
                    newStr2 = node.InnerXml;
                }
            }
            if (operation == "")
            {
                val = newStr1;
            }
            else
            {
                val = Helpers.OperationOnString(newStr1, newStr2, operation);
            }
            vals.Add(val);
            return true;
        }
    }
}