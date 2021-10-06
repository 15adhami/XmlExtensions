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

        protected override void SetException()
        {
            exceptionVals = new string[] { field };
            exceptionFields = new string[] { "field" };
        }

        protected override bool Patch(XmlDocument xml)
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
            var bindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            FieldInfo fieldInfo = GenTypes.GetTypeInAnyAssembly(ModSettingsClass).GetField(field, bindings);
            if (fieldInfo == null)
            {
                Error("Failed to get field");
                return false;
            }
            object value = fieldInfo.GetValue(null);
            return RunPatchesConditional((bool)value, caseTrue, caseFalse, xml);
        }
    }
}
