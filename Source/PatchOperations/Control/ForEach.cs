using System.Xml;
using Verse;
using System;

namespace XmlExtensions
{
    public class ForEach : PatchOperationExtendedPathed
    {
        protected XmlContainer apply;
        protected string storeIn = "DEF";
        protected string brackets = "{}";
        protected int prefixLength = 2;
        protected override bool Patch(XmlDocument xml)
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
}

