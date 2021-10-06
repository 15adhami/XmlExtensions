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

        private string currPrefix;

        protected override void SetException()
        {
            if (currPrefix == null)
            {
                exceptionVals = new string[] { storeIn, xpath };
                exceptionFields = new string[] { "storeIn", "xpath" };
            }
            else
            {
                exceptionVals = new string[] { storeIn, currPrefix, xpath, };
                exceptionFields = new string[] { "storeIn", "currPrefix", "xpath" };
            }
        }

        protected override bool Patch(XmlDocument xml)
        {
            foreach (XmlNode xmlNode in nodes)
            {
                // TODO: Make sure node wasn't deleted
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

