using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class UseSettingsExternal : PatchOperationValue
    {
        public string ModClass;
        public List<string> fields;

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            Type modType = GenTypes.GetTypeInAnyAssembly(ModClass);
            if (modType == null)
            {
                Error("Mod could not be found");
                return false;
            }
            Traverse settingsTraverse = Traverse.Create(Traverse.Create(LoadedModManager.GetMod(modType)).Field("modSettings").GetValue());
            for (int i = 0; i < fields.Count; i++)
            {
                object value = settingsTraverse.Field(fields[i]).GetValue();
                if (value == null)
                {
                    Error("Failed to find the field \"" + fields[i] + "\"");
                    return false;
                }
                try
                {
                    vals.Add(value.ToString());
                }
                catch
                {
                    Error("The field \"" + fields[i] + "\" cannot be converted to a string");
                    return false;
                }
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