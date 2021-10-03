using System.Collections.Generic;
using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{

    public class ApplyPatch : PatchOperationExtended
    {
        public string patchName;
        public List<string> arguments;

        protected override bool Patch(XmlDocument xml)
        {
            try
            {
                XmlNode node = xml.SelectSingleNode("Defs/XmlExtensions.PatchDef[@Name=\"" + patchName + "\"]");
                if (node == null)
                {
                    PatchManager.errors.Add("XmlExtensions.ApplyPatch(patchName=" + patchName + "): Could not find a patch with the given patchName");
                    return false;
                }
                PatchDef patchDef = DirectXmlToObject.ObjectFromXml<PatchDef>(node, false);
                XmlContainer newContainer = patchDef.apply;
                if (arguments != null)
                {
                    newContainer = Helpers.substituteVariablesXmlContainer(patchDef.apply, patchDef.parameters, arguments, patchDef.brackets);
                }
                int errNum = 0;
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.ApplyPatch(patchName=" + patchName + "): Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.ApplyPatch(patchName=" + patchName + "): " + e.Message);
                return false;
            }
        }
    }
}