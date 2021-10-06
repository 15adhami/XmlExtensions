using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class Group : SettingContainer
    {
        public List<SettingContainer> settings = new List<SettingContainer>();

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            Rect rect = listingStandard.GetRect(CalcHeight(listingStandard.ColumnWidth, selectedMod));
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.verticalSpacing = listingStandard.verticalSpacing;
            listing_Standard.Begin(rect);
            foreach (SettingContainer setting in settings)
            {
                setting.DrawSetting(listing_Standard, selectedMod);
            }
            listing_Standard.End();
        }

        protected override int CalcHeight(float width, string selectedMod)
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
            return DefaultValueSettingsList(modId, settings);
        }

        protected override bool Init()
        {
            return InitializeSettingsList(settings);
        }
    }
}
