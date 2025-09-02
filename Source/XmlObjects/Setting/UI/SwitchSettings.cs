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

        public List<SwitchSetting> cases;

        private Dictionary<string, List<SettingContainer>> valSettingDict;

        protected override bool Init()
        {
            searchType = SearchType.SearchCustom;
            addDefaultSpacing = false;
            if (cases != null)
            {
                valSettingDict = new Dictionary<string, List<SettingContainer>>();
                foreach (SwitchSetting switchSetting in cases)
                {
                    if (!InitializeContainers(switchSetting.settings, switchSetting.value.ToString()))
                    {
                        return false;
                    }
                    valSettingDict.Add(switchSetting.value, switchSetting.settings);
                }
            }
            return true;
        }

        protected override float CalculateHeight(float width)
        {
            return CalculateHeightSettingsList(width, valSettingDict[SettingsManager.GetSetting(modId, key)]);
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            DrawSettingsList(inRect, valSettingDict[SettingsManager.GetSetting(modId, key)]);
        }

        protected override bool FilterSettingsCustom()
        {
            bool flag = false;
            List<SettingContainer> selectedContainer = valSettingDict[SettingsManager.GetSetting(modId, key)];
            if (FilterSettings(selectedContainer))
            {
                flag = true;
                containedFiltered[selectedContainer] = true;
            }
            return flag;
        }
    }
}