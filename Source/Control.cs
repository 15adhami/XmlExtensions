using System.Xml;
using System.Collections.Generic;
using Verse;
using XmlExtensions.Boolean;

namespace XmlExtensions
{
    public class ForLoop : PatchOperation
    {
        protected XmlContainer apply;
        protected string storeIn = "i";
        protected string brackets = "{}";
        protected int from;
        protected int to;
        protected int increment = 1;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            int errNum = 0;
            string oldXml = this.apply.node.OuterXml;
            if (this.increment > 0)
            {
                for (int i = this.from; i < this.to; i += increment)
                {
                    XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, i.ToString(), this.brackets);
                    if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                    {
                        PatchManager.errors.Add("Error in XmlExtensions.ForLoop at iteration: " + i.ToString() + ", operation index: " + errNum.ToString());
                        return false;
                    }                        
                }
            }
            else if (this.increment < 0)
            {
                for (int i = this.from - 1; i >= this.to; i -= increment)
                {
                    XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, i.ToString(), this.brackets);
                    if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                    {
                        PatchManager.errors.Add("Error in XmlExtensions.ForLoop at iteration: " + i.ToString() + ", operation index: " + errNum.ToString());
                        return false;
                    }
                }
            }       
            return true;
        }
    }

    public class ForEach : PatchOperationPathed
    {
        protected XmlContainer apply;
        protected string storeIn = "DEF";
        protected string brackets = "{}";
        protected int prefixLength = 2;
        protected override bool ApplyWorker(XmlDocument xml)
        {
            int errNum = 0;
            XmlNodeList nodeList = xml.SelectNodes(this.xpath);
            if(nodeList == null || nodeList.Count == 0)
            {
                PatchManager.errors.Add("[XmlExtensions.ForEach]: Could not find a node that matches the xpath: " + xpath);
                return false;
            }
            foreach (XmlNode xmlNode in nodeList)
            {
                string path = xmlNode.GetXPath();
                string prefix = Helpers.getPrefix(path, prefixLength);
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, prefix, this.brackets);
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("[XmlExtensions.ForEach]: Error in operation at location: " + errNum.ToString() + ", current prefix: " + prefix);
                    return false;
                }
            }
            return true;
        }

    }
  
    public class IfStatement : PatchOperationPathed
    {
        protected PatchOperationBoolean condition = null;
        protected XmlContainer caseTrue =  null;
        protected XmlContainer caseFalse = null;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            int errNum = 0;
            // TODO: Add better error reporting to Boolean.evaluate()
            bool flag = false;
            try
            {
                flag = this.condition.evaluate(xml);
            }
            catch
            {
                PatchManager.errors.Add("Error in XmlExtensions.IfStatement in evaluating the condition");
                return false;
            }
            if (flag)
            {
                if (this.caseTrue != null)
                {
                    if(!Helpers.runPatchesInXmlContainer(this.caseTrue, xml, ref errNum))
                    {
                        PatchManager.errors.Add("Error in XmlExtensions.IfStatement in caseTrue, operation: " + errNum.ToString());
                        return false;
                    }
                }
            }
            else
            {
                if (this.caseFalse != null)
                {
                    if (!Helpers.runPatchesInXmlContainer(this.caseFalse, xml, ref errNum))
                    {
                        PatchManager.errors.Add("Error in XmlExtensions.IfStatement in caseFalse, operation: " + errNum.ToString());
                        return false;
                    }
                }
            }
                return true;
        }

    }

    public class Case : PatchOperation
    {
        public string value;
        public XmlContainer apply;
    }

    public class PatchByCase : PatchOperationPathed
    {
        public string value;
        public List<Case> cases;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            int errNum = 0;
            if (value == null)
            {
                PatchManager.errors.Add("XmlExtensions.PatchByCase: <value> is null");
                return false;
            }
            if (cases == null)
            {
                PatchManager.errors.Add("XmlExtensions.PatchByCase: <cases> is null");
                return false;
            }
            int c = 0;
            foreach(Case casePatch in cases)
            {
                c++;
                if (value == casePatch.value)
                {
                    if (!Helpers.runPatchesInXmlContainer(casePatch.apply, xml, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.PatchByCase: Error while running case with <value>="+value+", in the operation at position=" + errNum.ToString());
                        return false;
                    }
                    return true;
                }

                // run first case as default case
                if(c == cases.Count)
                {
                    if (!Helpers.runPatchesInXmlContainer(cases[0].apply, xml, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.PatchByCase: Error while running the first case as default case, in the operation at position=" + errNum.ToString());
                        return false;
                    }
                    return true;
                }
            }            
            return true;
        }

    }

}

