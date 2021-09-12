using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Verse;
using XmlExtensions.Boolean;

namespace XmlExtensions
{
    public abstract class PatchOperationValue : PatchOperationPathed
    {
        public virtual bool getValue(ref string val, XmlDocument xml)
        {
            return false;
        }

        public virtual bool getVar(ref string var)
        {
            return false;
        }
    }

    public class CreateVariable : PatchOperationValue
    {
        protected XmlContainer apply;
        protected string storeIn;
        protected string brackets = "{}";
        protected string value = "";
        protected string value2 = "";
        protected string defaultValue;
        protected string defaultValue2;
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
                string ans = "";
                if(!getValue(ref ans, xml))
                { // Error message already added
                    return false;
                }
                if (operation == "")
                {
                    XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, storeIn, ans, brackets);
                    if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.CreateVariable(value=" + value + "): Error in the operation at position=" + errNum.ToString());
                        return false;
                    }
                }
                else
                {
                    XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, storeIn, ans, brackets);
                    if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.CreateVariable(calculatedValue=" + ans + "): Error in the operation at position=" + errNum.ToString());
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

        public override bool getVar(ref string var)
        {
            var = storeIn;
            return true;
        }

        public override bool getValue(ref string val, XmlDocument xml)
        {
            try
            {
                string newStr1 = this.value;
                string newStr2 = this.value2;
                if (this.fromXml)
                {
                    XmlNode node = xml.SelectSingleNode(this.value);
                    if (node == null)
                    {
                        if (defaultValue == null)
                        {
                            PatchManager.errors.Add("XmlExtensions.CreateVariable(value=" + value + "): Failed to find a node with the given xpath");
                            return false;
                        }
                        newStr1 = defaultValue;
                    }
                    else
                    {
                        newStr1 = node.InnerText;
                    }
                }
                if (this.fromXml2)
                {
                    XmlNode node = xml.SelectSingleNode(this.value2);
                    if (node == null)
                    {
                        if (defaultValue2 == null)
                        {
                            PatchManager.errors.Add("XmlExtensions.CreateVariable(value2=" + value2 + "): Failed to find a node with the given xpath");
                            return false;
                        }
                        newStr2 = defaultValue2;
                    }
                    else
                    {
                        newStr2 = node.InnerText;
                    }
                }
                if (operation == "")
                {
                    val = newStr1;
                }
                else
                {
                    val = Helpers.operationOnString(newStr1, newStr2, operation);
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.CreateVariable(value=" + value + "): " + e.Message);
                return false;
            }
        }
    }

    public class CumulativeMath : PatchOperationValue
    {
        protected XmlContainer apply;
        protected string storeIn;
        protected string brackets = "{}";
        protected string operation;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                string temp = "";
                if(!getValue(ref temp, xml))
                {
                    return false;
                }
                int errNum = 0;
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, storeIn, temp, this.brackets);
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

        public override bool getVar(ref string var)
        {
            var = storeIn;
            return true;
        }

        public override bool getValue(ref string value, XmlDocument xml)
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
                value = sum.ToString();
            }
            return true;
        }
    }

    public class GetAttribute : PatchOperationValue
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
                string temp = "";
                if (!getValue(ref temp, xml))
                {
                    return false;
                }
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, storeIn, temp, brackets);
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

        public override bool getValue(ref string value, XmlDocument xml)
        {
            try
            {
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
                value = xattribute.Value;
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.GetAttribute(xpath=" + xpath + ", attribute=" + attribute + "): "+e.Message);
                return false;
            }
        }

        public override bool getVar(ref string var)
        {
            var = storeIn;
            return true;
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

    public class EvaluateBoolean : PatchOperationValue
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
                if (apply == null)
                {
                    PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): <apply>=null");
                    return false;
                }                
                int errNum = 0;
                string temp = "";
                if (!getValue(ref temp, xml))
                {
                    return false;
                }
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, storeIn, temp, brackets);
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

        public override bool getVar(ref string var)
        {
            var = storeIn;
            return true;
        }
        
        public override bool getValue(ref string value, XmlDocument xml)
        {
            try
            {
                if (condition == null)
                {
                    PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): <condition>=null");
                    return false;
                }
                bool b = false;
                if (!condition.evaluate(ref b, xml))
                {
                    PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): Failed to evaluate <condition>");
                    return false;
                }
                value = b.ToString();
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): " + e.Message);
                return false;
            }
        }
    }

    public class AggregateValues : PatchOperation
    {
        public XmlContainer valueOperations;
        public XmlContainer apply;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                List<string> values = new List<string>();
                List<string> vars = new List<string>();
                for (int i = 0; i < valueOperations.node.ChildNodes.Count; i++)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(Helpers.substituteVariables(valueOperations.node.ChildNodes[i].OuterXml, vars, values, "{}"));
                    XmlNode newNode = doc.DocumentElement;
                    PatchOperationValue patchOperation = DirectXmlToObject.ObjectFromXml<PatchOperationValue>(newNode, false);
                    string temp = "";
                    if (!patchOperation.getValue(ref temp, xml))
                    {
                        PatchManager.errors.Add("XmlExtensions.AggregateValues: Error in getting a value in <operations> at position=" + i.ToString());
                        return false;
                    }
                    values.Add(temp);
                    if (!patchOperation.getVar(ref temp))
                    {
                        PatchManager.errors.Add("XmlExtensions.AggregateValues: Error in getting a variable name in <operations> at position=" + i.ToString());
                        return false;
                    }
                    vars.Add(temp);
                }
                int errNum = 0;
                XmlContainer newContainer = Helpers.substituteVariablesXmlContainer(apply, vars, values, "{}");
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.AggregateValues: Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.AggregateValues: " + e.Message);
                return false;
            }
        }
    }
    /*
    public class CreateVariableConditional : PatchOperationValue
    {
        public PatchOperationBoolean condition;
        public XmlContainer caseTrue;
        public XmlContainer caseFalse;
        public XmlContainer apply;
        public string storeIn;
        public string brackets = "{}";

        protected override bool ApplyWorker(XmlDocument xml)
        {

        }

        public override bool getValue(ref string value, XmlDocument xml)
        {
            if (condition == null)
            {
                PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): <condition>=null");
                return false;
            }
            bool b = false;
            if (!condition.evaluate(ref b, xml))
            {
                PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): Failed to evaluate <condition>");
                return false;
            }
        }
    }
    */
}
