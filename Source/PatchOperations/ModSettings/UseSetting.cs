using System.Collections.Generic;
using System.Xml;

namespace XmlExtensions
{
    public class UseSetting : PatchOperationValue
    {
        public string modId;
        public string key;
        public string defaultValue;

        protected override void SetException()
        {
            CreateExceptions(key, "key", defaultValue, "defaultValue");
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
                if (!PatchManager.applyingPatches)
                {
                    Error("No such key exists");
                    return false;
                }
                value = defaultValue;
                SettingsManager.SetSetting(modId, key, defaultValue);
            }
            vals.Add(value);
            return true;
        }
    }
}