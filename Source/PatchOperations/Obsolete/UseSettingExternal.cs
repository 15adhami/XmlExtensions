using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class UseSettingExternal : PatchOperationValue
    {
        public string ModClass;
        public string field;

        protected override void SetException()
        {
            CreateExceptions(field, "field");
        }

        public override bool getVars(List<string> vars)
        {
            if (field == null)
            {
                NullError("field");
                return false;
            }
            vars.Add(field);
            return true;
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
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
            try
            {
                vals.Add(value.ToString());
                return true;
            }
            catch
            {
                Error("The given field cannot be converted to a string");
                return false;
            }
        }
    }
}