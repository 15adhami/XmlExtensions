using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    // TODO: Complete
    public class ApplyValueOperation : PatchOperationValue
    {
        public string patchName;
        public List<string> arguments;

        protected override void SetException()
        {
            CreateExceptions(storeIn, "storeIn", patchName, "patchName");
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            XmlNode node = xml.SelectSingleNode("Defs/XmlExtensions.PatchDef[@Name=\"" + patchName + "\"]");
            if (node == null)
            {
                Error("No PatchDef exists with the given patchName");
                return false;
            }
            PatchDef patchDef = DirectXmlToObject.ObjectFromXml<PatchDef>(node, false);
            XmlContainer newContainer = patchDef.apply;
            if (arguments != null)
            {
                newContainer = Helpers.SubstituteVariablesXmlContainer(patchDef.apply, patchDef.parameters, arguments, patchDef.brackets);
            }
            XmlDocument newDoc = new XmlDocument();
            newDoc.AppendChild(newDoc.CreateNode("element", "root", null));
            if (!RunPatches(newContainer, newDoc))
            {
                Error("Error while running PatchDef");
                return false;
            }
            node = newDoc.SelectSingleNode("root/return");
            string str = "";
            if (node != null)
            {
                str = node.InnerText;
            }
            vals.Add(str);
            return true;
        }

        public override bool getVars(List<string> vars)
        {
            vars.Add(storeIn);
            return true;
        }
    }
}