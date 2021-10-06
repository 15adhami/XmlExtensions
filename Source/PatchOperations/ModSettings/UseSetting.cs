using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class UseSetting : PatchOperationValue
    {
        protected string modId;
        protected string key;
        protected string defaultValue;

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
            XmlMod.loadedMod = modId;
            XmlMod.addXmlMod(modId);
            string value;
            bool didContain = SettingsManager.TryGetSetting(modId, key, out value);
            if (!didContain)
            {
                value = defaultValue;
                XmlMod.addSetting(modId, key, defaultValue);
            }
            if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(key))
            {
                XmlMod.settingsPerMod[modId].defValues.Add(key, defaultValue);
            }
            if (!XmlMod.settingsPerMod[modId].keys.Contains(key))
            {
                XmlMod.settingsPerMod[modId].keys.Add(key);
            }
            vals.Add(value);
            return true;
        }
    }

}
