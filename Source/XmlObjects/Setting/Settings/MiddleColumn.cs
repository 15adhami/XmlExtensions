using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class MiddleColumn : SettingContainer
    {
        public float split = 0.50f;
        public List<SettingContainer> settings = new List<SettingContainer>();

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            Rect baseRect = listingStandard.GetRect(columnHeight(settings, listingStandard.ColumnWidth * split, selectedMod)).LeftPart(split/2f+0.5f).RightPart(split/(split / 2f + 0.5f));
            Listing_Standard lListing = new Listing_Standard();
            lListing.Begin(baseRect);
            lListing.verticalSpacing = listingStandard.verticalSpacing;
            foreach (SettingContainer setting in settings)
            {
                setting.DrawSetting(lListing, selectedMod);
            }
            lListing.End();
        }

        private int columnHeight(List<SettingContainer> settings, float width, string selectedMod)
        {
            int h = 0;
            foreach (SettingContainer setting in settings)
            {
                h += setting.GetHeight(width, selectedMod);
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

        protected override int CalcHeight(float width, string selectedMod) { return columnHeight(settings, width * split, selectedMod); }

        protected override bool PreClose(string selectedMod)
        {
            return DoPreCloseSettingsList(selectedMod, settings);
        }
    }
}
