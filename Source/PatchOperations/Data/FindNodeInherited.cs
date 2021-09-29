using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
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

    public class FindNodeInherited : PatchOperationValue
    {
        public XmlContainer apply;
        public string storeIn;
        public string brackets = "{}";
        public string defaultValue;
        public string xpathDef;
        public string xpathLocal;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                int errNum = 0;
                string ans = "";
                if (!GetValue(ref ans, xml))
                { // Error message already added
                    return false;
                }
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, storeIn, ans, brackets);
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.FindNodeInherited(xpathDef=" + xpathDef + "): Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.FindNodeInherited(xpathDef=" + xpathDef + ", xpathLocal=" + xpathLocal + "): " + e.Message);
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
                if (xpathDef == null)
                {
                    PatchManager.errors.Add("XmlExtensions.FindNodeInherited: <xpathDef> is null");
                    return false;
                }
                if (xpathLocal == null)
                {
                    PatchManager.errors.Add("XmlExtensions.FindNodeInherited(xpathDef=" + xpathDef + "): <xpathLocal> is null");
                    return false;
                }
                string newStr = "";
                XmlNode defNode = xml.SelectSingleNode(xpathDef);
                if (defNode == null)
                {
                    PatchManager.errors.Add("XmlExtensions.FindNodeInherited(xpathDef=" + xpathDef + "): Failed to find a node with the given xpath");
                    return false;
                }
                XmlNode node = findNode(defNode, xpathLocal, xml);
                if (node == null)
                {
                    if(xml != PatchManager.XmlDocs["Defs"])
                    {
                        node = findNode(defNode, xpathLocal, PatchManager.XmlDocs["Defs"]);
                        if (node == null)
                        {
                            if (defaultValue == null)
                            {
                                PatchManager.errors.Add("XmlExtensions.FindNodeInherited(xpathDef=" + xpathDef + ", xpathLocal=" + xpathLocal + "): The Def and all of its ancestors failed to match <xpathLocal>");
                                return false;
                            }
                            newStr = defaultValue;
                        }
                        else
                        {
                            newStr = node.InnerXml;
                        }
                    }
                    else
                    {
                        if (defaultValue == null)
                        {
                            PatchManager.errors.Add("XmlExtensions.FindNodeInherited(xpathDef=" + xpathDef + ", xpathLocal=" + xpathLocal + "): The Def and all of its ancestors failed to match <xpathLocal>");
                            return false;
                        }
                        newStr = defaultValue;
                    }
                }
                else
                {
                    newStr = node.InnerXml;
                }                
                val = newStr;
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.FindNodeInherited(xpathDef=" + xpathDef + ", xpathLocal=" + xpathLocal + "): " + e.Message);
                return false;
            }
        }

        private XmlNode findNode(XmlNode defNode, string path, XmlDocument xml)
        {
            if (defNode == null)
                return null;
            XmlNode node = defNode.SelectSingleNode(path);
            if (node != null)
            {
                return node;
            }
            else
            {
                XmlAttribute att = defNode.Attributes["ParentName"];
                if (att == null)
                {
                    return null;
                }
                string parent = att.InnerText;
                if (parent == null)
                {
                    return null;
                }
                else
                {
                    return findNode(xml.SelectSingleNode("/Defs/" + defNode.Name + "[@Name=\"" + parent + "\"]"), path, xml);
                }
            }
        }
    }

}
