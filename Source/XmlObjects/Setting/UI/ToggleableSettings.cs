using System.Collections.Generic;
using UnityEngine;

namespace XmlExtensions.Setting
{
    internal class ToggleableSettings : SettingContainer
    {
        public string key;
        public List<SettingContainer> caseTrue;
        public List<SettingContainer> caseFalse;

        protected override bool Init()
        {
            addDefaultSpacing = false;
            if (!InitializeContainers(menuDef, caseTrue, "caseTrue"))
            {
                return false;
            }
            if (!InitializeContainers(menuDef, caseFalse, "caseFalse"))
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

        internal override bool PreOpen()
        {
            if (!PreOpenContainers(caseTrue, "caseTrue"))
            {
                return false;
            }
            else
                return PreOpenContainers(caseFalse, "caseFalse");
        }

        internal override bool PostClose()
        {
            if (!PostCloseContainers(caseTrue, "caseTrue"))
            {
                return false;
            }
            else
                return PostCloseContainers(caseFalse, "caseFalse");
        }
    }
}