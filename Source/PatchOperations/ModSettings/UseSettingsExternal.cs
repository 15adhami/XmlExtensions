using System;
using System.Collections.Generic;
using System.Xml;
using Verse;
using System.Reflection;

namespace XmlExtensions
{
    public class UseSettingsExternal : PatchOperationData
    {
        public string ModSettingsClass;
        public List<string> fields;

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            if (ModSettingsClass == null)
            {
                NullError("ModSettingsClass");
                return false;
            }
            if (fields == null)
            {
                NullError("fields");
                return false;
            }
            if (apply == null)
            {
                NullError("apply");
                return false;
            }
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
                    Error(new string[] { ModSettingsClass }, new string[] { "ModSettingsClass" }, "Failed to find ModSettingsClass");
                    return false;
                }
                if (fieldInfo == null)
                {
                    Error("Failed to find the field \"" + fields[i] + "\"");
                    return false;
                }
                object value = fieldInfo.GetValue(null);
                vals.Add((string)value);
            }
            return true;
        }

        public override bool getVars(List<string> vars)
        {
            foreach (string field in fields)
            {
                vars.Add(field);
            }
            return true;
        }
    }
}
