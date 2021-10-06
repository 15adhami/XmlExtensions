using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class ToggleableSettings : KeyedSettingContainer
    {
        public List<SettingContainer> caseTrue = new List<SettingContainer>();
        public List<SettingContainer> caseFalse = new List<SettingContainer>();

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            List<SettingContainer> settings;
            if (bool.Parse(XmlMod.allSettings.dataDict[XmlMod.selectedMod + ";" + key]))
            {
                settings = caseTrue;
            }
            else
            {
                settings = caseFalse;
            }
            if (settings != null)
            {
                Rect baseRect = listingStandard.GetRect(calcHeight(settings, listingStandard.ColumnWidth, selectedMod));
                Listing_Standard listing = new Listing_Standard();
                listing.Begin(baseRect);
                listing.verticalSpacing = listingStandard.verticalSpacing;
                foreach (SettingContainer setting in settings)
                {
                    setting.DrawSetting(listing, selectedMod);
                }
                listing.End();
            }
        }

        private int calcHeight(List<SettingContainer> settings, float width, string selectedMod)
        {
            int h = 0;
            if (settings != null)
            {
                foreach (SettingContainer setting in settings)
                {
                    h += setting.GetHeight(width, selectedMod);
                }
            }
            return h;
        }

        protected override bool SetDefaultValue(string modId)
        {
            if (!DefaultValueSettingsList(modId, caseTrue, "caseTrue"))
            {
                return false;
            }
            if (!DefaultValueSettingsList(modId, caseFalse, "caseFalse"))
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

        protected override int CalcHeight(float width, string selectedMod)
        {
            return (bool.Parse(XmlMod.allSettings.dataDict[XmlMod.selectedMod + ";" + key]) ? calcHeight(caseTrue, width, selectedMod) : calcHeight(caseFalse, width, selectedMod));
        }
    }
}