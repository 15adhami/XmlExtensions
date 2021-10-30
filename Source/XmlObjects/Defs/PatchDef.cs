using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    /// <summary>
    /// A Def that contains a patch
    /// </summary>
    public class PatchDef : Def
    {
        /// <summary>
        /// The list of parameters
        /// </summary>
        public List<string> parameters;

        /// <summary>
        /// The patch operations to apply
        /// </summary>
        public XmlContainer apply;

        /// <summary>
        /// Brackets used in variable substitution
        /// </summary>
        public string brackets = "{}";

        /// <summary>
        /// Whether or not the PatchDef returns a value
        /// </summary>
        protected bool valueOperation = false;

        /// <summary>
        /// Run the patches in the PatchDef. Error handling is done automatically
        /// </summary>
        /// <param name="arguments">The input arguments</param>
        /// <param name="xml">The XML document to run the patches on. Set to null if applying in-game</param>
        /// <returns>Reeturns false if there was an error, true otherwise</returns>
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