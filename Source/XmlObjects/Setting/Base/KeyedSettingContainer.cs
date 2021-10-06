using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public abstract class KeyedSettingContainer : SettingContainer
    {
        public string key = null;
        public string label = null;
        public string defaultValue = null;

        protected override bool SetDefaultValue(string modId)
        {
            if (key == null)
            {
                ThrowError("<key> is null");
                return false;
            }
            if (!XmlMod.settingsPerMod[modId].keys.Contains(key))
            {
                XmlMod.settingsPerMod[modId].keys.Add(key);
            }
            if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(key))
            {
                if (defaultValue != null)
                {
                    XmlMod.settingsPerMod[modId].defValues.Add(key, defaultValue);
                    if (!XmlMod.allSettings.dataDict.ContainsKey(modId + ";" + key))
                        XmlMod.allSettings.dataDict.Add(modId + ";" + key, defaultValue);
                }
            }
            return true;
        }

        public override void DrawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            try
            {
                DrawSettingContents(listingStandard, selectedMod);
            }
            catch
            {
                GUI.color = Color.red;
                listingStandard.Label("Error drawing setting (maybe <defaultValue> needs to be defined?)");
                errHeight = 22;
                GUI.color = Color.white;
            }
        }
    }
}