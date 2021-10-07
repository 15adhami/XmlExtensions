using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class UseSetting : PatchOperationValue
    {
        public string modId;
        public string key;
        public string defaultValue;

        protected override void SetException()
        {
            exceptionVals = new string[] { key, defaultValue };
            exceptionFields = new string[] { "key", "defaultValue" };
        }

        public override bool getVars(List<string> vars)
        {
            vars.Add(key);
            return true;
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            if (key == null)
            {
                NullError("key");
                return false;
            }
            if (defaultValue == null)
            {
                NullError("defaultValue");
                return false;
            }
            if (modId == null)
            {
                NullError("modId");
                return false;
            }
            SettingsManager.AddMod(modId);
            SettingsManager.SetDefaultValue(modId, key, defaultValue);
            bool didContain = SettingsManager.TryGetSetting(modId, key, out string value);
            if (!didContain)
            {
                value = defaultValue;
                SettingsManager.SetSetting(modId, key, defaultValue);
            }
            vals.Add(value);
            return true;
        }
    }

}
