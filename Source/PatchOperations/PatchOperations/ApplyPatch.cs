using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class ApplyPatch : PatchOperationExtended
    {
        public string patchName;
        public List<string> arguments;

        protected override void SetException()
        {
            CreateExceptions(patchName, "patchName");
        }

        protected override bool Patch(XmlDocument xml)
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
            return RunPatches(newContainer, xml);
        }
    }
}