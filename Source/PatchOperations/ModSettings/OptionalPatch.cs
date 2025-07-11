using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class OptionalPatch : PatchOperationExtended
    {
        public string key;
        public string modId;
        public string defaultValue;
        public List<string> keys;
        public List<string> defaultValues;
        public XmlContainer caseTrue;
        public XmlContainer caseFalse;

        protected override void SetException()
        {
            if (keys == null)
            {
                CreateExceptions(key, "key", defaultValue, "defaultValue");
            }
            else
            {
                CreateExceptions("[list]", "keys", "[list]", "defaultValues");
            }
        }

        protected override bool Patch(XmlDocument xml)
        {
            // Default case
            if (key == null && keys == null)
            {
                NullError("key");
                return false;
            }
            if (modId == null)
            {
                NullError("modId");
                return false;
            }
            if (defaultValue == null && defaultValues == null)
            {
                NullError("defaultValue");
                return false;
            }
            SettingsManager.AddMod(modId);
            string settingValue;
            bool didContain;
            bool flag = true;

            // Handle case of multiple keys first
            if (keys != null)
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
                for (int i = 0; i < keys.Count; i++)
                {
                    SettingsManager.SetDefaultValue(modId, keys[i], defaultValues[i]);
                    didContain = SettingsManager.TryGetSetting(modId, keys[i], out settingValue);
                    if (!didContain)
                    {
                        if (!PatchManager.Coordinator.IsApplyingPatches)
                        {
                            Error("No such key exists");
                            return false;
                        }
                        settingValue = defaultValues[i];
                        SettingsManager.SetSetting(modId, keys[i], defaultValues[i]);
                    }
                    flag = flag && bool.Parse(settingValue);
                }
            }
            else
            { // Standard case
                SettingsManager.SetDefaultValue(modId, key, defaultValue);
                didContain = SettingsManager.TryGetSetting(modId, key, out settingValue);
                if (!didContain)
                {
                    if (!PatchManager.Coordinator.IsApplyingPatches)
                    {
                        Error("No such key exists");
                        return false;
                    }
                    settingValue = defaultValue;
                    SettingsManager.SetSetting(modId, key, defaultValue);
                }
                flag = bool.Parse(settingValue);
            }
            return RunPatchesConditional(flag, caseTrue, caseFalse, xml);
        }
    }
}