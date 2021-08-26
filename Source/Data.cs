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
            int errNum = 0;
            if (this.fromXml)
            {
                XmlNode node = xml.SelectSingleNode(this.value);
                if (node == null)
                {
                    PatchManager.errors.Add("Error in XmlExtensions.CreateVariable in finding a node for <value> with xpath: " + value);
                    return false;
                }
                newStr1 = node.InnerText;
            }
            if (this.fromXml2)
            {
                XmlNode node = xml.SelectSingleNode(this.value2);
                if (node == null)
                {
                    PatchManager.errors.Add("Error in XmlExtensions.CreateVariable in finding a node for <value2> with xpath: " + value2);
                    return false;
                }
                newStr2 = node.InnerText;
            }
            if (operation == "")
            {
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, newStr1, this.brackets);
                if(!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("Error in XmlExtensions.CreateVariable at operation index: " + errNum.ToString() + ", with value: " + newStr1);
                    return false;
                }
            }
            else
            {
                string results = Helpers.operationOnString(newStr1, newStr2, operation);
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, results, this.brackets);
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("Error in XmlExtensions.CreateVariable at operation index: " + errNum.ToString() + ", with value: " + results);
                    return false;
                }
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
            float sum = 0;
            XmlNodeList XmlList = xml.SelectNodes(xpath);
            if (XmlList == null || XmlList.Count == 0)
            {
                PatchManager.errors.Add("Error in XmlExtensions.CumulativeMath in finding a node with the xpath: " + xpath);
                return false;
            }
            int n = XmlList.Count;
            string newStr1 = "";
            if (this.operation != "count")
            {
                float m;
                try
                {
                    m = float.Parse(XmlList[0].InnerText);
                }
                catch
                {
                    PatchManager.errors.Add("Error in XmlExtensions.CumulativeMath in getting a value from the node:" + XmlList[0].OuterXml);
                    return false;
                }
                foreach (object obj in XmlList)
                {
                    XmlNode xmlNode = obj as XmlNode;
                    float val;
                    try
                    {
                        val = float.Parse(xmlNode.InnerText);
                    }
                    catch
                    {
                        PatchManager.errors.Add("Error in XmlExtensions.CumulativeMath in getting a value from the node:" + xmlNode.OuterXml);
                        return false;
                    }
                    if (this.operation == "+")
                    {
                        sum += val;
                    }
                    else if (this.operation == "-")
                    {
                        sum -= val;
                    }
                    else if (this.operation == "*")
                    {
                        sum *= val;
                    }
                    else if (this.operation == "average")
                    {
                        sum += val / n;
                    }
                    else if (this.operation == "min")
                    {
                        m = Math.Min(m, val);
                    }
                    else if (this.operation == "max")
                    {
                        m = Math.Max(m, val);
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
            int errNum = 0;
            XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, storeIn, newStr1, this.brackets);
            if(!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
            {
                PatchManager.errors.Add("Error in XmlExtensions.CumulativeMath at operation index: " + errNum.ToString());
                return false;
            }
            return true;
        }
    }

    public class GetAttribute : PatchOperationPathed
    {
        public XmlContainer apply;
        public string attribute;
        public string storeIn;
        public string brackets = "{}";        

        protected override bool ApplyWorker(XmlDocument xml)
        {
            int errNum = 0;
            XmlAttribute xattribute;
            XmlNode node = xml.SelectSingleNode(xpath);
            if(node == null)
            {
                PatchManager.errors.Add("Error in XmlExtensions.GetAttribute in finding a node with the xpath: " + xpath);
                return false;
            }
            xattribute = node.Attributes[this.attribute];
            if(xattribute == null)
            {
                PatchManager.errors.Add("Error in XmlExtensions.GetAttribute in finding the attribute: " + attribute);
                return false;
            }            
            string newStr = xattribute.Value;
            XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, newStr, this.brackets);
            if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
            {
                PatchManager.errors.Add("Error in XmlExtensions.GetAttribute at operation index: " + errNum.ToString());
                return false;
            }
            return true;
        }
    }

    public class Log : PatchOperationPathed
    {
        protected string text = null;
        protected string error = null;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            if (text == null && xpath == null && error == null)
            {
                Verse.Log.Message("XmlExtensions.Log");
            }
            if (text != null)
                Verse.Log.Message(text);
            if (error != null)
                Verse.Log.Error(error);
            if (xpath != null)
            {
                XmlNodeList nodeList = xml.SelectNodes(xpath);
                if(nodeList == null || nodeList.Count == 0)
                {
                    PatchManager.errors.Add("Error in XmlExtensions.Log in finding a node with the xpath: " + xpath);
                    return false;
                }
                foreach (XmlNode node in nodeList)
                {
                    Verse.Log.Message(node.OuterXml);
                }          
            }
            return true;
        }
    }


}
