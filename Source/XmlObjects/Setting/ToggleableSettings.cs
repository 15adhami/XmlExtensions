using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class ToggleableSettings : KeyedSettingContainer
    {
        public List<SettingContainer> caseTrue = new List<SettingContainer>();
        public List<SettingContainer> caseFalse = new List<SettingContainer>();

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            List<SettingContainer> settings;
            if (bool.Parse(XmlMod.allSettings.dataDict[XmlMod.selectedMod + ";" + key]))
            {
                settings = caseTrue;
            }
            else
            {
                settings = caseFalse;
            }
            if (settings != null)
            {
                Rect baseRect = listingStandard.GetRect(calcHeight(settings, listingStandard.ColumnWidth, selectedMod));
                Listing_Standard listing = new Listing_Standard();
                listing.Begin(baseRect);
                listing.verticalSpacing = listingStandard.verticalSpacing;
                foreach (SettingContainer setting in settings)
                {
                    setting.DrawSetting(listing, selectedMod);
                }
                listing.End();
            }
        }

        private int calcHeight(List<SettingContainer> settings, float width, string selectedMod)
        {
            int h = 0;
            if (settings != null)
            {
                foreach (SettingContainer setting in settings)
                {
                    h += setting.GetHeight(width, selectedMod);
                }
            }
            return h;
        }

        public override bool SetDefaultValue(string modId)
        {
            if (key == null)
            {
                PatchManager.errors.Add("Error in " + this.GetType().ToString() + ": <key> is null");
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
            if (caseTrue != null)
            {
                int c = 0;
                foreach (SettingContainer setting in caseTrue)
                {
                    c++;
                    if(!setting.SetDefaultValue(modId))
                    {
                        PatchManager.errors.Add("Error in XmlExtensions.Setting.ToggleableSettings: failed to initialize a setting in <caseTrue> at position: " + c.ToString());
                        return false;
                    }
                }
            }
            if (caseFalse != null)
            {
                int c = 0;
                foreach (SettingContainer setting in caseFalse)
                {
                    c++;
                    if (!setting.SetDefaultValue(modId))
                    {
                        PatchManager.errors.Add("Error in XmlExtensions.Setting.ToggleableSettings: failed to initialize a setting in <caseFalse> at position: " + c.ToString());
                        return false;
                    }
                }
            }
            return true;
        }

        public override void Init()
        {
            base.Init();
            if (caseTrue != null)
            {
                foreach (SettingContainer setting in caseTrue)
                {
                    setting.Init();
                }
            }
            if (caseFalse != null)
            {
                foreach (SettingContainer setting in caseFalse)
                {
                    setting.Init();
                }
            }
        }

        public override int getHeight(float width, string selectedMod) { return (bool.Parse(XmlMod.allSettings.dataDict[XmlMod.selectedMod + ";" + key]) ? calcHeight(caseTrue, width, selectedMod) : calcHeight(caseFalse, width, selectedMod)); }
    }
}
