using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class ConditionalInherited : PatchOperationExtended
    {
        public string xpathDef;
        public string xpathLocal;
        public XmlContainer caseTrue;
        public XmlContainer caseFalse;

        protected override void SetException()
        {
            CreateExceptions(xpathDef, "xpathDef", xpathLocal, "xpathLocal");
        }

        protected override bool Patch(XmlDocument xml)
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
            bool b = findNode(defNode, xpathLocal, xml);
            if (xml != PatchManager.defaultDoc && !b)
            {
                b = findNode(defNode, xpathLocal, PatchManager.defaultDoc);
            }
            return RunPatchesConditional(b, caseTrue, caseFalse, xml);
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