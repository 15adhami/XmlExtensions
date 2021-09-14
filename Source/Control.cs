using System.Xml;
using System.Collections.Generic;
using Verse;
using XmlExtensions.Boolean;
using System.Linq;
using System;

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
                        PatchManager.errors.Add("XmlExtensions.ForLoop: Error at iteration " + i.ToString() + ", in the operation as position=" + errNum.ToString());
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
                        PatchManager.errors.Add("XmlExtensions.ForLoop: Error at iteration " + i.ToString() + ", in the operation as position=" + errNum.ToString());
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
            try
            {
                int errNum = 0;
                XmlNodeList nodeList;
                nodeList = xml.SelectNodes(this.xpath);
                if (nodeList == null || nodeList.Count == 0)
                {
                    PatchManager.errors.Add("XmlExtensions.ForEach(xpath=" + xpath + "): Error in finding a node with <xpath>=" + xpath);
                    return false;
                }
                foreach (XmlNode xmlNode in nodeList)
                {
                    // Make sure node wasn't deleted
                    string path = xmlNode.GetXPath();
                    if (path[0] == '/')
                    {
                        path = path.Substring(1);
                    }
                    if (path[path.Length-1] == '/')
                    {
                        path = path.Substring(0, path.Length-1);
                    }
                    if (path.Split('/').Length>=prefixLength)
                    {
                        string prefix = Helpers.getPrefix(path, prefixLength);
                        XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, storeIn, prefix, brackets);
                        if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.ForEach(xpath=" + xpath + ", curr_prefix=" + prefix + "): Error in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                }
                return true;
            }            
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.ForEach(xpath=" + xpath + "): " + e.Message);
                return false;
            }
        }

    }
  
    public class IfStatement : PatchOperationPathed
    {
        protected PatchOperationBoolean condition = null;
        protected XmlContainer caseTrue =  null;
        protected XmlContainer caseFalse = null;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                int errNum = 0;
                bool flag = false;
                try
                {
                    bool b = false;
                    if(!condition.evaluate(ref b, xml))
                    {
                        PatchManager.errors.Add("XmlExtensions.IfStatement: Failed to evaluate <condition>");
                        return false;
                    }
                    flag = b;
                }
                catch
                {
                    PatchManager.errors.Add("XmlExtensions.IfStatement: Error in evaluating the condition");
                    return false;
                }
                if (flag)
                {
                    if (this.caseTrue != null)
                    {
                        if (!Helpers.runPatchesInXmlContainer(this.caseTrue, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.IfStatement: Error in <caseTrue> in the operation at position=" + errNum.ToString());
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
                            PatchManager.errors.Add("XmlExtensions.IfStatement: Error in <caseFalse> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.IfStatement: " + e.Message);
                return false;
            }
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
            try
            {
                int errNum = 0;
                if (value == null)
                {
                    PatchManager.errors.Add("XmlExtensions.PatchByCase: <value> is null");
                    return false;
                }
                if (cases == null)
                {
                    PatchManager.errors.Add("XmlExtensions.PatchByCase(value=" + value + "): <cases> is null");
                    return false;
                }
                int c = 0;
                foreach (Case casePatch in cases)
                {
                    c++;
                    if (value == casePatch.value)
                    {
                        if (!Helpers.runPatchesInXmlContainer(casePatch.apply, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.PatchByCase(value=" + value + "): Error in case with <value>=" + value + ", in the operation at position=" + errNum.ToString());
                            return false;
                        }
                        return true;
                    }

                    // run first case as default case
                    if (c == cases.Count)
                    {
                        if (!Helpers.runPatchesInXmlContainer(cases[0].apply, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.PatchByCase(value=" + value + "): Error while running the first case as default case, in the operation at position=" + errNum.ToString());
                            return false;
                        }
                        return true;
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.PatchByCase(value=" + value + "): " + e.Message);
                return false;
            }
        }

    }

    public class FindMod : PatchOperation
    {
        public List<string> mods;
        public bool packageId = false;
        public string logic = "or";
        public XmlContainer caseTrue;
        public XmlContainer caseFalse;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                bool flag = false;
                int errNum = 0;
                if (mods == null)
                {
                    PatchManager.errors.Add("XmlExtensions.FindMod: <mods> is null");
                    return false;
                }
                if (logic == "or")
                {
                    if (!packageId)
                        flag = mods.Any(x => LoadedModManager.RunningMods.Any(y => y.Name == x));
                    else
                        flag = mods.Any(x => LoadedModManager.RunningMods.Any(y => y.PackageId.ToLower() == x.ToLower()));
                }
                else
                {
                    if (!packageId)
                        flag = mods.All(x => LoadedModManager.RunningMods.Any(y => y.Name == x));
                    else
                        flag = mods.All(x => LoadedModManager.RunningMods.Any(y => y.PackageId.ToLower() == x.ToLower()));
                }

                if (flag)
                {
                    if (caseTrue != null)
                    {
                        if (!Helpers.runPatchesInXmlContainer(caseTrue, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.FindMod: Error in <caseTrue> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                }
                else
                {
                    if (caseFalse != null)
                    {
                        if (!Helpers.runPatchesInXmlContainer(caseFalse, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.FindMod: Error in <caseFalse> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.FindMod: " + e.Message);
                return false;
            }
        }
    }

}

