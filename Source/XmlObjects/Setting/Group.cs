using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class Group : SettingContainer
    {
        public List<SettingContainer> settings = new List<SettingContainer>();

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            Rect rect = listingStandard.GetRect(getHeight(listingStandard.ColumnWidth, selectedMod));
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.verticalSpacing = listingStandard.verticalSpacing;
            listing_Standard.Begin(rect);
            foreach (SettingContainer setting in settings)
            {
                setting.DrawSetting(listing_Standard, selectedMod);
            }
            listing_Standard.End();
        }

        public override int getHeight(float width, string selectedMod)
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

        public override bool SetDefaultValue(string modId)
        {
            int t = 0;
            foreach (SettingContainer setting in settings)
            {
                t++;
                if (!setting.SetDefaultValue(modId))
                {
                    PatchManager.errors.Add("XmlExtensions.Setting.Group: Failed to initialize a setting at position=" + t.ToString());
                    return false;
                }
            }
            return true;
        }

        public override void Init()
        {
            base.Init();
            if (settings != null)
            {
                foreach (SettingContainer setting in settings)
                {
                    setting.Init();
                }
            }
        }
    }
}
