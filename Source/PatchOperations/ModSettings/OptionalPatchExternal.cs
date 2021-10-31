using HarmonyLib;
using System;
using System.Reflection;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class OptionalPatchExternal : PatchOperationExtended
    {
        protected string field;
        protected string ModSettingsClass;
        public string ModClass;
        protected XmlContainer caseTrue;
        protected XmlContainer caseFalse;

        protected override void SetException()
        {
            CreateExceptions(field, "field");
        }

        protected override bool Patch(XmlDocument xml)
        {
            if (ModSettingsClass != null)
            {
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
            else
            {
                Type modType = GenTypes.GetTypeInAnyAssembly(ModClass);
                if (modType == null)
                {
                    Error("Mod could not be found");
                    return false;
                }
                Traverse settingsTraverse = Traverse.Create(Traverse.Create(LoadedModManager.GetMod(modType)).Field("modSettings").GetValue());
                object value = settingsTraverse.Field(field).GetValue();
                if (value == null)
                {
                    Error("Failed to find field");
                    return false;
                }
                return RunPatchesConditional((bool)value, caseTrue, caseFalse, xml);
            }
        }
    }
}