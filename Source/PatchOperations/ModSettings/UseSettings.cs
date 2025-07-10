using System.Collections.Generic;
using System.Xml;

namespace XmlExtensions
{
    internal class UseSettings : PatchOperationValue
    {
        public string modId;
        public List<string> keys;
        public List<string> defaultValues;

        public override bool getVars(List<string> vars)
        {
            foreach (string key in keys)
            {
                vars.Add(key);
            }
            return true;
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            if (modId == null)
            {
                NullError("modId");
                return false;
            }
            if (PatchManager.Coordinator.IsApplyingPatches)
            {
                if (keys.Count > defaultValues.Count)
                {
                    Error("There are more keys than defaultValues");
                    return false;
                }
                else if (keys.Count < defaultValues.Count)
                {
                    Error("There are more defaultValues than keys");
                    return false;
                }
            }
            SettingsManager.AddMod(modId);
            for (int i = 0; i < keys.Count; i++)
            {
                SettingsManager.SetDefaultValue(modId, keys[i], defaultValues[i]);
                bool didContain = SettingsManager.TryGetSetting(modId, keys[i], out string value);
                if (!didContain)
                {
                    if (!PatchManager.Coordinator.IsApplyingPatches)
                    {
                        Error("No such key exists");
                        return false;
                    }
                    value = defaultValues[i];
                    SettingsManager.SetSetting(modId, keys[i], defaultValues[i]);
                }
                vals.Add(value);
            }
            return true;
        }
    }
}