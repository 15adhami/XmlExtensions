using System;
using System.Xml;

namespace XmlExtensions.Boolean
{

    public class ConditionalInherited : BooleanBase
    {
        public string xpathDef;
        public string xpathLocal;

        protected override bool Evaluation(ref bool b, XmlDocument xml)
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
