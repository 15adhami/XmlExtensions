using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class PatchDef : Def
    {
        public List<string> parameters;
        public XmlContainer apply;
        public string brackets = "{}";
        protected bool valueOperation = false;

        public bool ApplyPatch(List<string> arguments, XmlDocument xml = null)
        {
            XmlContainer container = apply;
            try
            {
                if (arguments != null)
                {
                    container = Helpers.SubstituteVariablesXmlContainer(apply, parameters, arguments, brackets);
                }
                for (int j = 0; j < container.node.ChildNodes.Count; j++)
                {
                    PatchOperation patch;
                    try
                    {
                        patch = Helpers.GetPatchFromString(container.node.ChildNodes[j].OuterXml);
                    }
                    catch (Exception e)
                    {
                        ErrorManager.AddError("PatchDef(defName=\"" + defName + "\"): Failed to create patch from XML for the operation at position=" + (j + 1).ToString());
                        ErrorManager.PrintErrors();
                        return false;
                    }
                    if (!patch.Apply(xml))
                    {
                        ErrorManager.AddError("PatchDef(defName=\"" + defName + "\"): Error in the operation at position=" + (j + 1).ToString());
                        ErrorManager.PrintErrors();
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                ErrorManager.AddError("PatchDef(defName=\"" + defName + "\"): " + e.Message);
                ErrorManager.PrintErrors();
                return false;
            }
        }
    }
}