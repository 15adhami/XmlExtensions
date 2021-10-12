using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class ApplyPatch : PatchOperationExtended
    {
        public string patchName;
        public string defName;
        public List<string> arguments;

        protected override void SetException()
        {
            if (patchName != null)
            {
                CreateExceptions(patchName, "patchName");
            }
            else
            {
                CreateExceptions(defName, "defName");
            }
        }

        protected override bool Patch(XmlDocument xml)
        {
            PatchDef patchDef;
            if (PatchManager.applyingPatches)
            {
                XmlNode node;
                if (patchName != null)
                {
                    node = xml.SelectSingleNode("Defs/XmlExtensions.PatchDef[@Name=\"" + patchName + "\"]");
                }
                else
                {
                    node = xml.SelectSingleNode("Defs/XmlExtensions.PatchDef[defName=\"" + defName + "\"]");
                }
                if (node == null)
                {
                    Error("No such PatchDef exists");
                    return false;
                }
                patchDef = DirectXmlToObject.ObjectFromXml<PatchDef>(node, false);
            }
            else
            {
                patchDef = DefDatabase<PatchDef>.GetNamed(defName);
                if (patchDef == null)
                {
                    Error("No such PatchDef exists");
                    return false;
                }
            }
            XmlContainer newContainer = patchDef.apply;
            if (arguments != null)
            {
                newContainer = Helpers.SubstituteVariablesXmlContainer(patchDef.apply, patchDef.parameters, arguments, patchDef.brackets);
            }
            return RunPatches(newContainer, xml);
        }
    }
}