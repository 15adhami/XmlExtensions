using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class SplitColumn : SettingContainer
    {
        public float split = 0.50f;
        public List<SettingContainer> leftCol = new List<SettingContainer>();
        public List<SettingContainer> rightCol = new List<SettingContainer>();
        public bool drawLine = false;
        private Spacing gapSize = Spacing.Small;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            Rect baseRect = listingStandard.GetRect(Math.Max(columnHeight(leftCol, listingStandard.ColumnWidth*split - ((int)gapSize), selectedMod), columnHeight(rightCol, listingStandard.ColumnWidth*(1-split) - ((int)gapSize), selectedMod)));
            Rect leftRect = baseRect.LeftPartPixels(baseRect.width*split - ((int)gapSize));
            Rect rightRect = baseRect.RightPartPixels(baseRect.width * (1 - split) - ((int)gapSize));
            Listing_Standard lListing = new Listing_Standard();
            lListing.Begin(leftRect);
            lListing.verticalSpacing = listingStandard.verticalSpacing;
            foreach (SettingContainer setting in leftCol)
            {
                setting.DrawSetting(lListing, selectedMod);
            }
            lListing.End();
            Listing_Standard rListing = new Listing_Standard();
            if(drawLine)
            {
                Color color = GUI.color;
                GUI.color = color * new Color(1f, 1f, 1f, 0.4f);
                GUI.DrawTexture(new Rect(baseRect.center.x, rightRect.yMin, 1f, rightRect.height), BaseContent.WhiteTex);
                GUI.color = color;
            }                
            rListing.Begin(rightRect);
            rListing.verticalSpacing = listingStandard.verticalSpacing;
            foreach (SettingContainer setting in rightCol)
            {
                setting.DrawSetting(rListing, selectedMod);
            }
            rListing.End();
        }

        private enum Spacing : int
        {
            None = 0,
            Tiny = 2,
            Small = 3,
            Medium = 5,
            Large = 9,
            Huge = 15
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

        public override bool SetDefaultValue(string modId)
        {
            int c = 0;
            if(leftCol != null)
            {
                c = 0;
                foreach(SettingContainer setting in leftCol)
                {
                    if(!setting.SetDefaultValue(modId))
                    {
                        c++;
                        PatchManager.errors.Add("Error in XmlExtensions.Setting.SplitColumn: failed to initialize a setting in <leftCol> at position: " + c.ToString());
                        return false;
                    }
                }
            }
            if (rightCol != null)
            {
                c = 0;
                foreach (SettingContainer setting in rightCol)
                {
                    if (!setting.SetDefaultValue(modId))
                    {
                        c++;
                        PatchManager.errors.Add("Error in XmlExtensions.Setting.SplitColumn: failed to initialize a setting in <rightCol> at position: " + c.ToString());
                        return false;
                    }
                }
            }
            return true;
        }

        public override void Init()
        {
            if(leftCol != null)
            {
                foreach (SettingContainer setting in leftCol)
                {
                    setting.Init();
                }
            }
            if (rightCol != null)
            {
                foreach (SettingContainer setting in rightCol)
                {
                    setting.Init();
                }
            }
        }

        public override int getHeight(float width, string selectedMod) { return Math.Max(columnHeight(leftCol, width * split - ((int)gapSize), selectedMod), columnHeight(rightCol, width * (1 - split) - ((int)gapSize), selectedMod)); }
    }
}
