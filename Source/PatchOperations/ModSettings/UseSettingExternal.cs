using System;
using System.Xml;
using Verse;
using System.Reflection;

namespace XmlExtensions
{
    public class UseSettingExternal : PatchOperationValue
    {
        protected string ModSettingsClass;
        protected string field;
        protected string brackets = "{}";
        protected XmlContainer apply;

        protected override bool Patch(XmlDocument xml)
        {
            try
            {
                string temp = "";
                if (!getValue(ref temp, xml))
                {
                    return false;
                }
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, field, temp, brackets);
                int errNum = 0;
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettingExternal(field=" + field + "): Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal(ModSettingsClass=" + ModSettingsClass + ", field=" + field + "): " + e.Message);
                return false;
            }
        }

        public override bool getVar(ref string var)
        {
            var = field;
            return true;
        }

        public override bool getValue(ref string val, XmlDocument xml)
        {
            if (field == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal(ModSettingsClass=" + ModSettingsClass + "): <field>=null");
                return false;
            }
            if (ModSettingsClass == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal(field=" + field + "): <ModSettingsClass>=null");
                return false;
            }
            if (apply == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal(field=" + field + "): <apply>=null");
                return false;
            }
            var bindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            FieldInfo fieldInfo = GenTypes.GetTypeInAnyAssembly(ModSettingsClass).GetField(field, bindings);
            if (fieldInfo == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal(field=" + field + "): Failed to get field");
                return false;
            }
            val = fieldInfo.GetValue(null).ToString();
            return true;
        }
    }

}
