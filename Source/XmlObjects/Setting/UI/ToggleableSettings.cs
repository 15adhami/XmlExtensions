using System.Collections.Generic;
using UnityEngine;

namespace XmlExtensions.Setting
{
    internal class ToggleableSettings : SettingContainer
    {
        public List<SettingContainer> caseTrue;
        public List<SettingContainer> caseFalse;

        private bool parsedSetting = false;

        protected override bool Init()
        {
            searchType = SearchType.SearchCustom;
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
            parsedSetting = bool.Parse(SettingsManager.GetSetting(modId, key));
            return parsedSetting ? CalculateHeightSettingsList(width, caseTrue) : CalculateHeightSettingsList(width, caseFalse);
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            List<SettingContainer> settings;
            if (parsedSetting)
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

        protected override bool FilterSettingsCustom()
        {
            bool flag = false;
            if (bool.Parse(SettingsManager.GetSetting(modId, key)))
            {
                if (FilterSettings(caseTrue))
                {
                    flag = true;
                    containedFiltered[caseTrue] = true;
                }
            }
            else
            {
                if (FilterSettings(caseFalse))
                {
                    flag = true;
                    containedFiltered[caseFalse] = true;
                }
            }
            return flag;
        }
    }
}