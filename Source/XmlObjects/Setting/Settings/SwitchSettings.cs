using System.Collections.Generic;
using Verse;

namespace XmlExtensions.Setting
{
    public class SwitchSetting : SettingContainer
    {
        public string value;
        public List<SettingContainer> settings;
    }

    public class SwitchSettings : SettingContainer
    {
        public string key;
        public List<SwitchSetting> cases = new List<SwitchSetting>();
        private Dictionary<string, List<SettingContainer>> valSettingDict;

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            try
            {
                List<SettingContainer> settings = valSettingDict[XmlMod.allSettings.dataDict[selectedMod + ";" + key]];
                foreach (SettingContainer setting in settings)
                {
                    setting.DrawSetting(listingStandard, selectedMod);
                }
            }
            catch
            {
            }
        }

        protected override int CalcHeight(float width, string selectedMod)
        {
            base.CalcHeight(width, selectedMod);
            int h = 0;
            try
            {
                List<SettingContainer> settings = valSettingDict[XmlMod.allSettings.dataDict[selectedMod + ";" + key]];
                foreach (SettingContainer setting in settings)
                {
                    h += setting.GetHeight(width, selectedMod);
                }
            }
            catch
            {
            }
            return h;
        }

        protected override bool Init()
        {
            if (cases != null)
            {
                foreach (SwitchSetting switchSetting in cases)
                {
                    if (!InitializeSettingsList(switchSetting.settings, switchSetting.value.ToString()))
                    {
                        return false;
                    }
                }
                valSettingDict = new Dictionary<string, List<SettingContainer>>();
                foreach (SwitchSetting setting in cases)
                {
                    valSettingDict.Add(setting.value, setting.settings);
                }
            }
            return true;
        }

        protected override bool SetDefaultValue(string modId)
        {
            if (cases != null)
            {
                foreach (SwitchSetting switchSetting in cases)
                {
                    if (!DefaultValueSettingsList(modId, switchSetting.settings, switchSetting.value.ToString()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected override bool PreClose(string selectedMod)
        {
            foreach (SwitchSetting switchSetting in cases)
            {
                if (!DoPreCloseSettingsList(selectedMod, switchSetting.settings, switchSetting.value.ToString()))
                {
                    return false;
                }
            }
            return true;
        }
    }
}