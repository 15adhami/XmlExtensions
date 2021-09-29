﻿using System.Xml;
using System.Collections.Generic;
using Verse;
using XmlExtensions.Boolean;
using System.Linq;
using System;
using System.Reflection;

namespace XmlExtensions
{
    public class ForLoop : PatchOperationExtended
    {
        protected XmlContainer apply;
        protected string storeIn = "i";
        protected string brackets = "{}";
        protected int from;
        protected int to;
        protected int increment = 1;

        protected override bool applyWorker(XmlDocument xml)
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

    public class ForEach : PatchOperationExtendedPathed
    {
        protected XmlContainer apply;
        protected string storeIn = "DEF";
        protected string brackets = "{}";
        protected int prefixLength = 2;
        protected override bool applyWorker(XmlDocument xml)
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
                    if (path[path.Length - 1] == '/')
                    {
                        path = path.Substring(0, path.Length - 1);
                    }
                    if (path.Split('/').Length >= prefixLength)
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
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.ForEach(xpath=" + xpath + "): " + e.Message);
                return false;
            }
        }

    }

    public class ForEachDescendant : PatchOperationExtended
    {
        public bool concreteOnly = false;
        public string xpathParent;
        protected XmlContainer apply;
        protected string storeIn = "DEF";
        protected string brackets = "{}";
        protected int prefixLength = 2;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                List<XmlNode> nodeList = new List<XmlNode>();
                if(!getNodes(nodeList, xpathParent, xml))
                {
                    return false;
                }
                if(nodeList.Count == 0)
                {
                    PatchManager.errors.Add("XmlExtensions.ForEachDescendant(xpathParent=" + xpathParent + "): Failed to find any descendants of the given parent");
                    return false;
                }
                foreach(XmlNode xmlNode in nodeList)
                {
                    int errNum = 0;
                    string path = xmlNode.GetXPath();
                    if (path[0] == '/')
                    {
                        path = path.Substring(1);
                    }
                    if (path[path.Length - 1] == '/')
                    {
                        path = path.Substring(0, path.Length - 1);
                    }
                    if (path.Split('/').Length >= prefixLength)
                    {
                        string prefix = Helpers.getPrefix(path, prefixLength);
                        XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, storeIn, prefix, brackets);
                        if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.ForEachDescendant(xpathParent=" + xpathParent + ", curr_prefix=" + prefix + "): Error in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.ForEachDescendant(xpathParent=" + xpathParent + "): " + e.Message);
                return false;
            }
        }

        private bool getNodes(List<XmlNode> list, string xpathParent, XmlDocument xml)
        {
            try
            {
                XmlNode parentNode = xml.SelectSingleNode(xpathParent);
                if(parentNode == null)
                {
                    PatchManager.errors.Add("XmlExtensions.ForEachDescendant(xpathParent=" + xpathParent + "): Failed to find a parent node with the given xpath");
                    return false;
                }
                XmlAttribute att = parentNode.Attributes["Name"];
                if(att == null)
                {
                    PatchManager.errors.Add("XmlExtensions.ForEachDescendant(xpathParent=" + xpathParent + "): Parent node does not have a \"Name\" attribute");
                    return false;
                }
                string parentName = att.InnerText;
                XmlNodeList nodeList = xml.SelectNodes("/Defs/" + parentNode.Name + "[@ParentName=\"" + parentName + "\"]");
                foreach(XmlNode node in nodeList)
                {
                    XmlAttribute abs = node.Attributes["Abstract"];
                    if (concreteOnly == false)
                    {
                        list.Add(node);
                    }
                    else if (abs != null)
                    {
                        if(abs.InnerText != "True")
                        {
                            list.Add(node);
                        }
                    }
                    else
                    {
                        list.Add(node);
                    }
                    XmlAttribute attr = node.Attributes["Name"];
                    if(attr != null)
                    {
                        if (!getNodes(list, node.GetXPath(), xml))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.ForEachDescendant(xpathParent=" + xpathParent + "): " + e.Message);
                return false;
            }
        }
    }

    public class IfStatement : PatchOperationExtendedPathed
    {
        protected PatchOperationBoolean condition = null;
        protected XmlContainer caseTrue = null;
        protected XmlContainer caseFalse = null;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                int errNum = 0;
                bool flag = false;
                try
                {
                    bool b = false;
                    if (!condition.evaluate(ref b, xml))
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
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.IfStatement: " + e.Message);
                return false;
            }
        }

    }

    public class Case
    {
        public string value;
        public XmlContainer apply;
    }

    public class PatchByCase : PatchOperationExtended
    {
        public string value;
        public List<Case> cases;

        protected override bool applyWorker(XmlDocument xml)
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
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.PatchByCase(value=" + value + "): " + e.Message);
                return false;
            }
        }

    }

    public class FindMod : PatchOperationExtended
    {
        public List<string> mods;
        public bool packageId = false;
        public string logic = "or";
        public XmlContainer caseTrue;
        public XmlContainer caseFalse;

        protected override bool applyWorker(XmlDocument xml)
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
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.FindMod: " + e.Message);
                return false;
            }
        }
    }

    public class Conditional : PatchOperationExtendedPathed
    {
        public XmlContainer caseTrue;
        public XmlContainer caseFalse;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                int errNum = 0;
                XmlNode node = xml.SelectSingleNode(xpath);
                if (node != null)
                {
                    if(caseTrue != null)
                    {
                        if (!Helpers.runPatchesInXmlContainer(caseTrue, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.Conditional(xpath=" + xpath + "): Error in <caseTrue> in the operation at position=" + errNum.ToString());
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
                            PatchManager.errors.Add("XmlExtensions.Conditional(xpath=" + xpath + "): Error in <caseFalse> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }                    
                }
                return caseTrue != null || caseFalse != null;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.Conditional(xpath=" + xpath + "): " + e.Message);
                return false;
            }
        }
    }

    public class ConditionalInherited : PatchOperationExtended
    {
        public string xpathDef;
        public string xpathLocal;
        public XmlContainer caseTrue;
        public XmlContainer caseFalse;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                if (xpathDef == null)
                {
                    PatchManager.errors.Add("XmlExtensions.ConditionalInherited: <xpathDef> is null");
                    return false;
                }
                if (xpathLocal == null)
                {
                    PatchManager.errors.Add("XmlExtensions.ConditionalInherited(xpathDef=" + xpathDef + "): <xpathLocal> is null");
                    return false;
                }
                XmlNode defNode = xml.SelectSingleNode(xpathDef);
                if (defNode == null)
                {
                    PatchManager.errors.Add("XmlExtensions.ConditionalInherited(xpathDef=" + xpathDef + "): Failed to find a node with the given xpath");
                    return false;
                }
                bool b = findNode(defNode, xpathLocal, xml);
                int errNum = 0;
                if(b)
                {
                    if(caseTrue != null)
                    {
                        if (!Helpers.runPatchesInXmlContainer(caseTrue, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.ConditionalInherited(xpathDef=" + xpathDef + ", xpathLocal=" + xpathLocal + "): Error in <caseTrue> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                }
                else
                {
                    if (xml == PatchManager.defaultDoc)
                    {
                        if (caseFalse != null)
                        {
                            if (!Helpers.runPatchesInXmlContainer(caseFalse, xml, ref errNum))
                            {
                                PatchManager.errors.Add("XmlExtensions.ConditionalInherited(xpathDef=" + xpathDef + ", xpathLocal=" + xpathLocal + "): Error in <caseFalse> in the operation at position=" + errNum.ToString());
                                return false;
                            }
                        }
                    }
                    else
                    {
                        b = findNode(defNode, xpathLocal, PatchManager.defaultDoc);
                        errNum = 0;
                        if (b)
                        {
                            if (caseTrue != null)
                            {
                                if (!Helpers.runPatchesInXmlContainer(caseTrue, xml, ref errNum))
                                {
                                    PatchManager.errors.Add("XmlExtensions.ConditionalInherited(xpathDef=" + xpathDef + ", xpathLocal=" + xpathLocal + "): Error in <caseTrue> in the operation at position=" + errNum.ToString());
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
                                    PatchManager.errors.Add("XmlExtensions.ConditionalInherited(xpathDef=" + xpathDef + ", xpathLocal=" + xpathLocal + "): Error in <caseFalse> in the operation at position=" + errNum.ToString());
                                    return false;
                                }
                            }
                        }
                    }            
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.ConditionalInherited(xpathDef=" + xpathDef + ", xpathLocal=" + xpathLocal + "): " + e.Message);
                return false;
            }
        }

        private bool findNode(XmlNode defNode, string path, XmlDocument xml)
        {
            XmlNode node = defNode.SelectSingleNode(path);
            if (node != null)
            {
                return true;
            }
            else
            {
                XmlAttribute att = defNode.Attributes["ParentName"];
                if (att == null)
                {
                    return false;
                }
                string parent = att.InnerText;
                if (parent == null)
                {
                    return false;
                }
                else
                {
                    return findNode(xml.SelectSingleNode("/Defs/" + defNode.Name + "[@Name=\"" + parent + "\"]"), path, xml);
                }
            }
        }
    }

    public class WhileLoop : PatchOperationExtended
    {
        public PatchOperationBoolean condition;
        public PatchContainer apply;

        protected override bool applyWorker(XmlDocument xml)
        {
            int c = 0;
            try
            {
                c++;
                if (condition == null)
                {
                    PatchManager.errors.Add("XmlExtensions.WhileLoop: <condition> is null");
                    return false;
                }
                if (apply == null)
                {
                    PatchManager.errors.Add("XmlExtensions.WhileLoop: <apply> is null");
                    return false;
                }
                bool b = false;
                if(!condition.evaluate(ref b, xml))
                {
                    PatchManager.errors.Add("XmlExtensions.WhileLoop(iter_num=" + c.ToString() + "): Failed to evaluate condition");
                    return false;
                }
                while(b)
                {
                    int errNum = 0;
                    if(!Helpers.runPatchesInPatchContainer(apply, xml, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.WhileLoop(iter_num=" + c.ToString() + "): Error in the operation at position=" + errNum.ToString());
                        return false;
                    }
                    c++;
                    if(c>=10000)
                    {
                        PatchManager.errors.Add("XmlExtensions.WhileLoop(iter_num=" + c.ToString() + "): Loop limit reached");
                        return false;
                    }
                    if (!condition.evaluate(ref b, xml))
                    {
                        PatchManager.errors.Add("XmlExtensions.WhileLoop(iter_num=" + c.ToString() + "): Failed to evaluate condition");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.WhileLoop(iter_num=" + c.ToString() + "): " + e.Message);
                return false;
            }
        }
    }

    public class TryCatch : PatchOperationExtended
    {
        public PatchContainer tryApply;
        public XmlContainer catchApply;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                int errNum = 0;
                if (!Helpers.runPatchesInPatchContainer(tryApply, xml, ref errNum))
                {
                    errNum = 0;
                    PatchManager.errors.Clear();
                    if (catchApply != null)
                    {
                        if (!Helpers.runPatchesInXmlContainer(catchApply, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.TryCatch: Error in <catchApply> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.TryCatch: " + e.Message);
                return false;
            }
        }
    }
}

