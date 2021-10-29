using System.Collections.Generic;
using UnityEngine;

namespace XmlExtensions.Setting
{
    internal class SwitchSettings : SettingContainer
    {
        public class SwitchSetting
        {
            public string value;
            public List<SettingContainer> settings;
        }

        public string key;
        public List<SwitchSetting> cases;

        private Dictionary<string, List<SettingContainer>> valSettingDict;

        protected override bool Init(string selectedMod)
        {
            if (cases != null)
            {
                valSettingDict = new Dictionary<string, List<SettingContainer>>();
                foreach (SwitchSetting switchSetting in cases)
                {
                    if (!InitializeSettingsList(selectedMod, switchSetting.settings, switchSetting.value.ToString()))
                    {
                        return false;
                    }
                    valSettingDict.Add(switchSetting.value, switchSetting.settings);
                }
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return CalculateHeightSettingsList(width, selectedMod, valSettingDict[SettingsManager.GetSetting(selectedMod, key)]);
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            DrawSettingsList(inRect, selectedMod, valSettingDict[SettingsManager.GetSetting(selectedMod, key)]);
        }
    }
}