using System.Collections.Generic;
using UnityEngine;

namespace XmlExtensions.Setting
{
    public class ToggleableSettings : SettingContainer
    {
        public string key;
        public List<SettingContainer> caseTrue;
        public List<SettingContainer> caseFalse;

        protected override bool SetDefaultValue(string modId)
        {
            if (!SetDefaultValueSettingsList(modId, caseTrue, "caseTrue"))
            {
                return false;
            }
            if (!SetDefaultValueSettingsList(modId, caseFalse, "caseFalse"))
            {
                return false;
            }
            return true;
        }

        protected override bool Init()
        {
            if (!InitializeSettingsList(caseTrue, "caseTrue"))
            {
                return false;
            }
            if (!InitializeSettingsList(caseFalse, "caseFalse"))
            {
                return false;
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return bool.Parse(SettingsManager.GetSetting(selectedMod, key)) ? GetHeightSettingsList(width, selectedMod, caseTrue) : GetHeightSettingsList(width, selectedMod, caseFalse);
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            List<SettingContainer> settings;
            if (bool.Parse(SettingsManager.GetSetting(selectedMod, key)))
            {
                settings = caseTrue;
            }
            else
            {
                settings = caseFalse;
            }
            if (settings != null)
            {
                DrawSettingsList(inRect, selectedMod, settings);
            }
        }

        protected override bool PreClose(string selectedMod)
        {
            if (!DoPreCloseSettingsList(selectedMod, caseTrue, "caseTrue"))
            {
                return false;
            }
            if (!DoPreCloseSettingsList(selectedMod, caseFalse, "caseFalse"))
            {
                return false;
            }
            return true;
        }
    }
}