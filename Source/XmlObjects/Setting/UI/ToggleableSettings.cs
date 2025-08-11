using System.Collections.Generic;
using UnityEngine;

namespace XmlExtensions.Setting
{
    internal class ToggleableSettings : SettingContainer
    {
        public List<SettingContainer> caseTrue;
        public List<SettingContainer> caseFalse;

        protected override bool Init()
        {
            searchType = SearchType.SearchDrawn;
            addDefaultSpacing = false;
            if (!InitializeContainers(caseTrue, "caseTrue"))
            {
                return false;
            }
            if (!InitializeContainers(caseFalse, "caseFalse"))
            {
                return false;
            }
            return true;
        }

        protected override float CalculateHeight(float width)
        {
            return bool.Parse(SettingsManager.GetSetting(modId, key)) ? CalculateHeightSettingsList(width, caseTrue) : CalculateHeightSettingsList(width, caseFalse);
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            List<SettingContainer> settings;
            if (bool.Parse(SettingsManager.GetSetting(modId, key)))
            {
                settings = caseTrue;
            }
            else
            {
                settings = caseFalse;
            }
            if (settings != null)
            {
                DrawSettingsList(inRect, settings);
            }
        }
    }
}