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

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
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

        public override int getHeight(float width, string selectedMod)
        {
            base.getHeight(width, selectedMod);
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

        public override void Init()
        {
            base.Init();
            if (cases != null)
            {
                foreach (SwitchSetting switchSetting in cases)
                {
                    if(switchSetting.settings != null)
                    {
                        foreach (SettingContainer setting in switchSetting.settings)
                        {
                            setting.Init();
                        }
                    }                    
                }
            }            
            if (cases != null)
            {
                valSettingDict = new Dictionary<string, List<SettingContainer>>();
                foreach (SwitchSetting setting in cases)
                {
                    valSettingDict.Add(setting.value, setting.settings);
                }
            }                     
    }

        public override bool SetDefaultValue(string modId)
        {
            if (cases != null)
            {
                int i = 0;
                foreach (SwitchSetting switchSetting in cases)
                {
                    i++;
                    int c = 0;
                    foreach (SettingContainer setting in switchSetting.settings)
                    {
                        c++;
                        if (!setting.SetDefaultValue(modId))
                        {
                            PatchManager.errors.Add("Error in XmlExtensions.Setting.SwitchSettings: failed to initialize a setting in case: " + i.ToString() + ", at position: " + c.ToString());
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
