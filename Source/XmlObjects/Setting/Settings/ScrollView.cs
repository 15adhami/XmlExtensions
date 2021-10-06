using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class ScrollView : SettingContainer
    {
        public float height = 72;
        public List<SettingContainer> settings = new List<SettingContainer>();

        private Vector2 scrollPos = Vector2.zero;

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            Rect baseRect = listingStandard.GetRect(height);            
            Rect scrollRect = new Rect(0, 0, baseRect.width - 20f, calcHeight(settings, baseRect.width - 20f, selectedMod));
            Widgets.BeginScrollView(baseRect, ref scrollPos, scrollRect);
            Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect2);
            listing.verticalSpacing = listingStandard.verticalSpacing;
            foreach (SettingContainer setting in settings)
            {
                setting.DrawSetting(listing, selectedMod);
            }
            listing.End();
            Widgets.EndScrollView();
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
            return DefaultValueSettingsList(modId, settings);
        }

        protected override bool Init()
        {
            return InitializeSettingsList(settings);
        }

        protected override int CalcHeight(float width, string selectedMod) { return ((int)height); }

        protected override bool PreClose(string selectedMod)
        {
            return DoPreCloseSettingsList(selectedMod, settings);
        }
    }
}
