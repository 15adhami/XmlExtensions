using System;
using System.Linq;
using System.Xml;
using Verse;
using XmlExtensions.Boolean;

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
            try
            {
                string newStr1 = this.value;
                string newStr2 = this.value2;
                int errNum = 0;
                if (this.fromXml)
                {
                    XmlNode node = xml.SelectSingleNode(this.value);
                    if (node == null)
                    {
                        PatchManager.errors.Add("XmlExtensions.CreateVariable(value=" + value + "): Failed to find a node with the given xpath");
                        return false;
                    }
                    newStr1 = node.InnerText;
                }
                if (this.fromXml2)
                {
                    XmlNode node = xml.SelectSingleNode(this.value2);
                    if (node == null)
                    {
                        PatchManager.errors.Add("XmlExtensions.CreateVariable(value2=" + value2 + "): Failed to find a node with the given xpath");
                        return false;
                    }
                    newStr2 = node.InnerText;
                }
                if (operation == "")
                {
                    XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, newStr1, this.brackets);
                    if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.CreateVariable(value=" + value + "): Error in the operation at position=" + errNum.ToString());
                        return false;
                    }
                }
                else
                {
                    string results = Helpers.operationOnString(newStr1, newStr2, operation);
                    XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, results, this.brackets);
                    if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.CreateVariable(calculatedValue=" + results + "): Error in the operation at position=" + errNum.ToString());
                        return false;
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.CreateVariable(value=" + value + "): " + e.Message);
                return false;
            }
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
            try
            {
                float sum = 0;
                XmlNodeList XmlList;
                XmlList = xml.SelectNodes(this.xpath);
                if (XmlList == null || XmlList.Count == 0)
                {
                    PatchManager.errors.Add("XmlExtensions.CumulativeMath(xpath=" + xpath + "): Failed to find a node with the given xpath");
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
                        PatchManager.errors.Add("XmlExtensions.CumulativeMath(xpath=" + xpath + "): Error in getting a value from the node:" + XmlList[0].OuterXml);
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
                            PatchManager.errors.Add("XmlExtensions.CumulativeMath(xpath=" + xpath + "): Error in getting a value from the node:" + xmlNode.OuterXml);
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
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.CumulativeMath(xpath=" + xpath + "): Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }            
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.CumulativeMath(xpath=" + xpath + "): " + e.Message);
                return false;
            }
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
            try
            {
                int errNum = 0;
                XmlAttribute xattribute;
                XmlNode node = xml.SelectSingleNode(xpath);
                if (node == null)
                {
                    PatchManager.errors.Add("XmlExtensions.GetAttribute(xpath=" + xpath + ", attribute=" + attribute + "): Failed to find a node with the given xpath");
                    return false;
                }
                xattribute = node.Attributes[this.attribute];
                if (xattribute == null)
                {
                    PatchManager.errors.Add("XmlExtensions.GetAttribute(xpath=" + xpath + ", attribute=" + attribute + "): Could not find attribute");
                    return false;
                }
                string newStr = xattribute.Value;
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, newStr, this.brackets);
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.GetAttribute(xpath=" + xpath + ", attribute=" + attribute + "): Error in operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.GetAttribute(xpath=" + xpath + ", attribute=" + attribute + "): " + e.Message);
                return false;
            }
        }
    }

    public class Log : PatchOperationPathed
    {
        protected string text = null;
        protected string error = null;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
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
                    XmlNodeList nodeList;
                    nodeList = xml.SelectNodes(this.xpath);
                    if (nodeList == null || nodeList.Count == 0)
                    {
                        PatchManager.errors.Add("XmlExtensions.Log(xpath=" + xpath + "): Failed to find a node with the given xpath");
                        return false;
                    }
                    foreach (XmlNode node in nodeList)
                    {
                        Verse.Log.Message(node.OuterXml);
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.Log: " + e.Message);
                return false;
            }
        }
    }

    public class EvaluateBoolean : PatchOperation
    {
        public PatchOperationBoolean condition;
        public string storeIn;
        public string brackets = "{}";
        public XmlContainer apply;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                if (storeIn == null)
                {
                    PatchManager.errors.Add("XmlExtensions.EvaluateBoolean: <storeIn>=null");
                    return false;
                }
                if (condition == null)
                {
                    PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): <condition>=null");
                    return false;
                }
                if (apply == null)
                {
                    PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): <apply>=null");
                    return false;
                }                
                int errNum = 0;
                string newStr;
                newStr = condition.evaluate(xml).ToString();
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, storeIn, newStr, brackets);
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): Error in operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }            
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): " + e.Message);
                return false;
            }
        }
    }
}
