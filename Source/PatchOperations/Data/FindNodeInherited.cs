using System.Collections.Generic;
using System.Xml;

namespace XmlExtensions
{
    internal class FindNodeInherited : PatchOperationValue
    {
        public string defaultValue;
        public string xpathDef;
        public string xpathLocal;

        protected override void SetException()
        {
            CreateExceptions(storeIn, "storeIn", xpathDef, "xpathDef", xpathLocal, "xpathLocal");
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            if (xpathDef == null)
            {
                NullError("xpathDef");
                return false;
            }
            if (xpathLocal == null)
            {
                NullError("xpathLocal");
                return false;
            }
            string newStr = "";
            XmlNode defNode = xml.SelectSingleNode(xpathDef);
            if (defNode == null)
            {
                XPathError("xpathDef");
                return false;
            }
            XmlNode node = findNode(defNode, xpathLocal, xml);
            if (node == null)
            {
                if (xml != PatchManager.XmlDocs["Defs"])
                {
                    node = findNode(defNode, xpathLocal, PatchManager.XmlDocs["Defs"]);
                    if (node == null)
                    {
                        if (defaultValue == null)
                        {
                            Error("The Def and all of its ancestors failed to match <xpathLocal>");
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
                        Error("The Def and all of its ancestors failed to match <xpathLocal>");
                        return false;
                    }
                    newStr = defaultValue;
                }
            }
            else
            {
                newStr = node.InnerXml;
            }
            vals.Add(newStr);
            return true;
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