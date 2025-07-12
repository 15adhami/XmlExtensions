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

        protected override bool Init()
        {
            addDefaultSpacing = false;
            if (cases != null)
            {
                valSettingDict = new Dictionary<string, List<SettingContainer>>();
                foreach (SwitchSetting switchSetting in cases)
                {
                    if (!InitializeContainers(modId, switchSetting.settings, switchSetting.value.ToString()))
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

        internal override bool PreOpen()
        {
            foreach (SwitchSetting switch_setting in cases)
            {
                if (!PreOpenContainers(switch_setting.settings))
                {
                    return false;
                }
            }
            return true;
        }

        internal override bool PostClose()
        {
            foreach (SwitchSetting switch_setting in cases)
            {
                if (!PostCloseContainers(switch_setting.settings))
                {
                    return false;
                }
            }
            return true;
        }
    }
}