using System.Collections.Generic;
using System.Xml;
using Verse;

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

        private string currPrefix;

        protected override void SetException()
        {
            if (currPrefix == null)
            {
                CreateExceptions(storeIn, "storeIn", xpathParent, "xpathParent");
            }
            else
            {
                CreateExceptions(storeIn, "storeIn", currPrefix, "currPrefix", xpathParent, "xpathParent");
            }
        }

        protected override bool Patch(XmlDocument xml)
        {
            List<XmlNode> nodeList = new List<XmlNode>();
            if (!getNodes(nodeList, xpathParent, xml))
            {
                return false;
            }
            if (nodeList.Count == 0)
            {
                Error("Failed to find any descendants of the given parent");
                return false;
            }
            foreach (XmlNode xmlNode in nodeList)
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

        private bool getNodes(List<XmlNode> list, string xpathParent, XmlDocument xml)
        {
            XmlNode parentNode = xml.SelectSingleNode(xpathParent);
            if (parentNode == null)
            {
                Error("Failed to find a parent node with the given xpath");
                return false;
            }
            XmlAttribute att = parentNode.Attributes["Name"];
            if (att == null)
            {
                Error("Parent node does not have a \"Name\" attribute");
                return false;
            }
            string parentName = att.InnerText;
            XmlNodeList nodeList = xml.SelectNodes("/Defs/" + parentNode.Name + "[@ParentName=\"" + parentName + "\"]");
            foreach (XmlNode node in nodeList)
            {
                XmlAttribute abs = node.Attributes["Abstract"];
                if (concreteOnly == false)
                {
                    list.Add(node);
                }
                else if (abs != null)
                {
                    if (abs.InnerText != "True")
                    {
                        list.Add(node);
                    }
                }
                else
                {
                    list.Add(node);
                }
                XmlAttribute attr = node.Attributes["Name"];
                if (attr != null)
                {
                    if (!getNodes(list, node.GetXPath(), xml))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}