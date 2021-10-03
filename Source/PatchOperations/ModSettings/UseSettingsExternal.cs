using System;
using System.Collections.Generic;
using System.Xml;
using Verse;
using System.Reflection;

namespace XmlExtensions
{
    public class UseSettingsExternal : PatchOperationExtended
    {
        protected string ModSettingsClass;
        protected List<string> fields;
        protected string brackets = "{}";
        protected XmlContainer apply;

        protected override bool Patch(XmlDocument xml)
        {
            try
            {
                if (ModSettingsClass == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettingsExternal: <ModSettingsClass>=null");
                    return false;
                }
                if (fields == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettingsExternal(ModSettingsClass=" + ModSettingsClass + "): <fields>=null");
                    return false;
                }
                if (apply == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettingsExternal(ModSettingsClass=" + ModSettingsClass + "): <apply>=null");
                    return false;
                }
                List<string> values = new List<string>();
                var bindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
                FieldInfo fieldInfo;
                for (int i = 0; i < fields.Count; i++)
                {
                    try
                    {

                        fieldInfo = GenTypes.GetTypeInAnyAssembly(ModSettingsClass).GetField(fields[i], bindings);
                    }
                    catch
                    {
                        PatchManager.errors.Add("XmlExtensions.UseSettingsExternal(ModSettingsClass=" + ModSettingsClass + "): Failed to get field");
                        return false;
                    }
                    if (fieldInfo == null)
                    {
                        PatchManager.errors.Add("XmlExtensions.UseSettingsExternal(ModSettingsClass=" + ModSettingsClass + "): Failed to get field");
                        return false;
                    }
                    object value = fieldInfo.GetValue(null);
                    values.Add((string)value);
                }
                XmlContainer newContainer = Helpers.substituteVariablesXmlContainer(apply, fields, values, brackets);
                int errNum = 0;
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettingsExternal(ModSettingsClass=" + ModSettingsClass + "): Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingsExternal(ModSettingsClass=" + ModSettingsClass + "): " + e.Message);
                return false;
            }
        }
    }

}
