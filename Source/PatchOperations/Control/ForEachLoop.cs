using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class ForEachLoop : PatchOperationExtended
    {
        public string xpath;
        protected XmlContainer apply;
        protected string storeIn = "path";
        protected string brackets = "{}";
        protected List<string> values;

        protected override void SetException()
        {
            if (values == null)
                CreateExceptions(storeIn, "storeIn", xpath, "xpath");
            else
                CreateExceptions(storeIn, "storeIn");
        }

        protected override bool Patch(XmlDocument xml)
        {
            if (values == null)
            {
                XmlNodeList nodes = xml.SelectNodes(xpath);
                if (nodes.Count == 0)
                {
                    XPathError();
                    return false;
                }
                foreach (XmlNode xmlNode in nodes)
                {
                    string path = xmlNode.GetXPath();
                    XmlContainer newContainer = Helpers.SubstituteVariableXmlContainer(apply, storeIn, path, brackets);
                    if (!RunPatches(newContainer, xml)) { return false; }
                }
            }
            else
            {
                foreach (string value in values)
                {
                    XmlContainer newContainer = Helpers.SubstituteVariableXmlContainer(apply, storeIn, value, brackets);
                    if (!RunPatches(newContainer, xml, false))
                    {
                        Error("Error with value=" + value);
                        return false;
                    }
                }
            }
            return true;
        }
    }
}