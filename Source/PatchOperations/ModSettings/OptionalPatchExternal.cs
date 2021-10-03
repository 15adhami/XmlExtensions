using System;
using System.Xml;
using Verse;
using System.Reflection;

namespace XmlExtensions
{
    public class OptionalPatchExternal : PatchOperationExtended
    {
        protected string field;
        protected string ModSettingsClass;
        protected XmlContainer caseTrue;
        protected XmlContainer caseFalse;

        protected override bool Patch(XmlDocument xml)
        {
            try
            {
                if (field == null)
                {
                    PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal(ModSettingsClass=" + ModSettingsClass + "): <field>=null");
                    return false;
                }
                if (ModSettingsClass == null)
                {
                    PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal(field=" + field + "): <ModSettingsClass>=null");
                    return false;
                }                
                var bindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
                FieldInfo fieldInfo = GenTypes.GetTypeInAnyAssembly(ModSettingsClass).GetField(field, bindings);
                if (fieldInfo == null)
                {
                    PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal(field=" + field + "): Failed to get field");
                    return false;
                }
                object value = fieldInfo.GetValue(null);

                if ((bool)value)
                {
                    if (this.caseTrue != null)
                    {
                        int errNum = 0;
                        if (!Helpers.runPatchesInXmlContainer(caseTrue, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal(field=" + field + "): Error in <caseTrue> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    if (this.caseFalse != null)
                    {
                        int errNum = 0;
                        if (!Helpers.runPatchesInXmlContainer(caseFalse, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal(field=" + field + "): Error in <caseFalse> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                    return true;
                }
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal(ModSettingsClass=" + ModSettingsClass + ", field=" + field + "): " + e.Message);
                return false;
            }
        }
    }

}
