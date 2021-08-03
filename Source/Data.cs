using System;
using System.Linq;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class CreateVariable : PatchOperation
    {
        protected XmlContainer apply;
        protected string storeIn;
        protected string brackets = "{}";
        protected string value = "";
        protected string value2 = "";
        protected bool fromXml = false;
        protected bool fromXml2 = false;
        protected string operation = "";

        protected override bool ApplyWorker(XmlDocument xml)
        {
            string newStr1 = this.value;
            string newStr2 = this.value2;
            if (this.fromXml)
            {
                newStr1 = xml.SelectSingleNode(this.value).InnerText;
            }
            if (this.fromXml2)
            {
                newStr2 = xml.SelectSingleNode(this.value2).InnerText;
            }
            if (value2 == "")
            {
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, newStr1, this.brackets);
                Helpers.runPatchesInXmlContainer(newContainer, xml);
            }
            else
            {
                string result = Helpers.operationOnString(newStr1, newStr2, this.operation);
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, result, this.brackets);
                Helpers.runPatchesInXmlContainer(newContainer, xml);
            }
            return true;
        }
    }

    public class CumulativeMath : PatchOperationPathed
    {
        protected XmlContainer apply;
        protected string storeIn;
        protected string brackets = "{}";
        protected string operation;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool result = false;
            float sum = 0;
            XmlNodeList XmlList = xml.SelectNodes(this.xpath);
            int n = XmlList.Count;
            string newStr1 = "";
            if (this.operation != "count")
            {
                float m = float.Parse(XmlList[0].InnerText);
                foreach (object obj in XmlList)
                {
                    result = true;
                    XmlNode xmlNode = obj as XmlNode;
                    if (this.operation == "+")
                    {
                        sum += float.Parse(xmlNode.InnerText);
                    }
                    else if (this.operation == "-")
                    {
                        sum -= float.Parse(xmlNode.InnerText);
                    }
                    else if (this.operation == "*")
                    {
                        sum *= float.Parse(xmlNode.InnerText);
                    }
                    else if (this.operation == "average")
                    {
                        sum += float.Parse(xmlNode.InnerText) / n;
                    }
                    else if (this.operation == "min")
                    {
                        m = Math.Min(m, float.Parse(xmlNode.InnerText));
                    }
                    else if (this.operation == "max")
                    {
                        m = Math.Max(m, float.Parse(xmlNode.InnerText));
                    }
                }
                if (this.operation == "min" || this.operation == "max")
                {
                    sum = m;
                }
            }
            else
            {
                sum = n;
            }
            if (this.operation != "concat")
            {
                newStr1 = sum.ToString();
            }
            XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, newStr1, this.brackets);
            Helpers.runPatchesInXmlContainer(newContainer, xml);
            return result;
        }
    }

    public class getAttribute : PatchOperationPathed
    {
        public XmlContainer apply;
        public string attribute;
        public string storeIn;
        public string brackets = "{}";        

        protected override bool ApplyWorker(XmlDocument xml)
        {
            var attribute = xml.SelectSingleNode(xpath).Attributes[this.attribute];
            string newStr = "";
            if (attribute != null)
            {
                newStr = attribute.Value;
            }
            XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, newStr, this.brackets);
            Helpers.runPatchesInXmlContainer(newContainer, xml);

            return true;
        }
    }
    
}
