using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class CumulativeMath : PatchOperationValue
    {
        protected XmlContainer apply;
        protected string storeIn;
        protected string brackets = "{}";
        protected string operation;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                string temp = "";
                if(!GetValue(ref temp, xml))
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

}
