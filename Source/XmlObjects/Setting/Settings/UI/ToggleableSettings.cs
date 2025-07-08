using System.Collections.Generic;
using UnityEngine;

namespace XmlExtensions.Setting
{
    internal class ToggleableSettings : SettingContainer
    {
        public string key;
        public List<SettingContainer> caseTrue;
        public List<SettingContainer> caseFalse;

        protected override bool Init(string selectedMod)
        {
            if (!InitializeSettingsList(selectedMod, caseTrue, "caseTrue"))
            {
                return false;
            }
            if (!InitializeSettingsList(selectedMod, caseFalse, "caseFalse"))
            {
                return false;
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return bool.Parse(SettingsManager.GetSetting(selectedMod, key)) ? CalculateHeightSettingsList(width, selectedMod, caseTrue) : CalculateHeightSettingsList(width, selectedMod, caseFalse);
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

        internal override bool PreOpen(string selectedMod)
        {
            if (!PreOpenSettingsList(selectedMod, caseTrue, "caseTrue"))
            {
                return false;
            }
            else
                return PreOpenSettingsList(selectedMod, caseFalse, "caseFalse");
        }
    }
}