using System.Xml;
using Verse;
using System;

namespace XmlExtensions
{
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
}

