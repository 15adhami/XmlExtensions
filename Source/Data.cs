using System.Linq;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class CreateVariable : PatchOperation
    {
        protected XmlContainer apply;
        protected string storeIn = "var";
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
                newStr1 = xml.SelectSingleNode(this.value).InnerXml;
            }
            if (this.fromXml2)
            {
                newStr2 = xml.SelectSingleNode(this.value2).InnerXml;
            }
            string oldXml = this.apply.node.OuterXml;
            string newXml;
            if (value2 == "")
            {
                newXml = Helpers.substituteVariable(oldXml, this.storeIn, newStr1, this.brackets);
                XmlContainer newContainer = new XmlContainer() { node = Helpers.getNodeFromString(newXml) };
                for (int j = 0; j < this.apply.node.ChildNodes.Count; j++)
                {
                    PatchOperation patch = Helpers.getPatchFromString(newContainer.node.ChildNodes[j].OuterXml);
                    patch.Apply(xml);
                }
            }
            else
            {
                string result = Helpers.operationOnString(newStr1, newStr2, this.operation);
                newXml = Helpers.substituteVariable(oldXml, this.storeIn, result, this.brackets);
                XmlContainer newContainer = new XmlContainer() { node = Helpers.getNodeFromString(newXml) };
                for (int j = 0; j < this.apply.node.ChildNodes.Count; j++)
                {
                    PatchOperation patch = Helpers.getPatchFromString(newContainer.node.ChildNodes[j].OuterXml);
                    patch.Apply(xml);
                }
            }
            return true;
        }
    }

    public class CumulativeMath : PatchOperationPathed
    {
        protected XmlContainer apply;
        protected string storeIn = "total";
        protected string brackets = "{}";
        protected string operation = "+";

        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool result = false;
            float sum = 0;
            XmlNodeList XmlList = xml.SelectNodes(this.xpath);
            int n = XmlList.Count;
            string newStr1 = "";
            if (this.operation != "count")
            {
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
                    else if (this.operation == "%")
                    {
                        sum %= float.Parse(xmlNode.InnerText);
                    }
                    else if (this.operation == "average")
                    {
                        sum += float.Parse(xmlNode.InnerText) / n;
                    }
                    else if (this.operation == "concat")
                    {
                        newStr1 += xmlNode.InnerText;
                    }
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
            string oldXml = this.apply.node.OuterXml;
            string newXml = Helpers.substituteVariable(oldXml, this.storeIn, newStr1, this.brackets);
            XmlContainer newContainer = new XmlContainer() { node = Helpers.getNodeFromString(newXml) };
            for (int j = 0; j < this.apply.node.ChildNodes.Count; j++)
            {
                PatchOperation patch = Helpers.getPatchFromString(newContainer.node.ChildNodes[j].OuterXml);
                patch.Apply(xml);
            }


            return result;
        }
    }

    public class Negate : PatchOperation
    {
        protected XmlContainer apply;
        protected string storeIn = "x";
        protected string brackets = "{}";
        protected string value = "1";
        protected bool fromXml = true;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            string newStr = this.value;
            if (this.fromXml)
            {
                newStr = xml.SelectSingleNode(this.value).InnerXml;
            }
            float f;
            if (float.TryParse(this.value, out f))
            {
                newStr = (-1 * f).ToString();
            }
            if (this.value == "true")
            {
                newStr = "false";
            }
            else if (this.value == "false")
            {
                newStr = "true";
            }
            string oldXml = this.apply.node.OuterXml;
            string newXml;
            newXml = Helpers.substituteVariable(oldXml, this.storeIn, newStr, this.brackets);
            XmlContainer newContainer = new XmlContainer() { node = Helpers.getNodeFromString(newXml) };
            for (int j = 0; j < this.apply.node.ChildNodes.Count; j++)
            {
                PatchOperation patch = Helpers.getPatchFromString(newContainer.node.ChildNodes[j].OuterXml);
                patch.Apply(xml);
            }
            return true;
        }
    }
}
