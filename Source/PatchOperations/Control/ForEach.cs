using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class ForEach : PatchOperationExtended
    {
        public string xpath;
        protected XmlContainer apply;
        protected string storeIn = "DEF";
        protected string brackets = "{}";
        protected int prefixLength = 2;

        private string currPrefix;

        protected override void SetException()
        {
            if (currPrefix == null)
            {
                CreateExceptions(storeIn, "storeIn", xpath, "xpath");
            }
            else
            {
                CreateExceptions(currPrefix, storeIn, xpath, "xpath");
            }
        }

        protected override bool Patch(XmlDocument xml)
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
                    currPrefix = Helpers.GetPrefix(path, prefixLength);
                    XmlContainer newContainer = Helpers.SubstituteVariableXmlContainer(apply, storeIn, currPrefix, brackets);
                    if (!RunPatches(newContainer, xml)) { return false; }
                }
            }
            return true;
        }
    }
}