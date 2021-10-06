using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class Section : SettingContainer
    {
        public float height = -1f;
        public List<SettingContainer> settings = new List<SettingContainer>();
        public float padding = 4f;

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            if(settings.Count > 0)
            {
                Rect rect = listingStandard.GetRect(CalcHeight(listingStandard.ColumnWidth, selectedMod));
                Color curColor = GUI.color;
                GUI.color = Widgets.MenuSectionBGFillColor * curColor;
                GUI.DrawTexture(rect, BaseContent.WhiteTex);
                GUI.color = new ColorInt(135, 135, 135).ToColor * curColor;
                Widgets.DrawBox(rect, 1, null);
                GUI.color = curColor;
                Listing_Standard listing_Standard = new Listing_Standard();
                listing_Standard.verticalSpacing = listingStandard.verticalSpacing;
                Rect rect2 = new Rect(rect.x + padding, rect.y + padding, rect.width - padding * 2f, rect.height - padding * 2f);
                listing_Standard.Begin(rect2);
                foreach (SettingContainer setting in settings)
                {
                    setting.DrawSetting(listing_Standard, selectedMod);
                }
                listing_Standard.End();
            }            
        }

        protected override int CalcHeight(float width, string selectedMod)
        {
            if (settings.Count == 0)
                return 0;
            if(height<0)
            {
                int h = 0;
                if (settings != null)
                {
                    foreach (SettingContainer setting in settings)
                    {
                        h += setting.GetHeight(width - padding * 2f, selectedMod);
                    }
                }
                return h + (int)padding * 2;
            }
            else
            {
                return (int)height + (int)padding * 2;
            }
        }

        protected override bool SetDefaultValue(string modId)
        {
            return DefaultValueSettingsList(modId, settings);
        }

        protected override bool Init()
        {
            return InitializeSettingsList(settings);
        }

        protected override bool PreClose(string selectedMod)
        {
            return DoPreCloseSettingsList(selectedMod, settings);
        }
    }
}
