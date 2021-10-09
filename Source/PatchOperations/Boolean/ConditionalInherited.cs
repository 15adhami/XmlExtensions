using System.Xml;

namespace XmlExtensions.Boolean
{
    public class ConditionalInherited : BooleanBase
    {
        public string xpathDef;
        public string xpathLocal;

        protected override void SetException()
        {
            CreateExceptions(xpathDef, "xpathDef", xpathLocal, "xpathLocal");
        }

        protected override bool Evaluation(ref bool b, XmlDocument xml)
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
            XmlNode defNode = xml.SelectSingleNode(xpathDef);
            if (defNode == null)
            {
                XPathError("xpathDef");
                return false;
            }
            b = findNode(defNode, xpathLocal, xml);
            if (!b)
            {
                if (xml == PatchManager.XmlDocs["Defs"])
                {
                    return true;
                }
                else
                {
                    b = findNode(defNode, xpathLocal, PatchManager.XmlDocs["Defs"]);
                    return true;
                }
            }
            return true;
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