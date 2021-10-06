using System;
using System.Xml;
using Verse;
using System.Reflection;
using System.Collections.Generic;

namespace XmlExtensions
{
    public class UseSettingExternal : PatchOperationValue
    {
        protected string ModSettingsClass;
        protected string field;

        protected override void SetException()
        {
            exceptionVals = new string[] { field };
            exceptionFields = new string[] { "field" };
        }

        public override bool getVars(List<string> vars)
        {
            vars.Add(field);
            return true;
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            if (field == null)
            {
                NullError("field");
                return false;
            }
            if (ModSettingsClass == null)
            {
                NullError("ModSettingsClass");
                return false;
            }
            if (apply == null)
            {
                NullError("apply");
                return false;
            }
            var bindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            FieldInfo fieldInfo = GenTypes.GetTypeInAnyAssembly(ModSettingsClass).GetField(field, bindings);
            if (fieldInfo == null)
            {
                Error("Failed to get field");
                return false;
            }
            vals.Add(fieldInfo.GetValue(null).ToString());
            return true;
        }
    }

}
