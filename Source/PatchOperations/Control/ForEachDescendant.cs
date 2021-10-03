using System.Xml;
using System.Collections.Generic;
using Verse;
using System;

namespace XmlExtensions
{
    public class ForEachDescendant : PatchOperationExtended
    {
        public bool concreteOnly = false;
        public string xpathParent;
        protected XmlContainer apply;
        protected string storeIn = "DEF";
        protected string brackets = "{}";
        protected int prefixLength = 2;

        protected override bool Patch(XmlDocument xml)
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
}

