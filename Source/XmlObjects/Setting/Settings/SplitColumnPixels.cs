using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class SplitColumnPixels : SettingContainer
    {
        public float pixels = -1f;
        public List<SettingContainer> leftCol = new List<SettingContainer>();
        public List<SettingContainer> rightCol = new List<SettingContainer>();
        public bool drawLine = false;
        private Spacing gapSize = Spacing.Small;

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            if (pixels < 0)
            {
                pixels = listingStandard.ColumnWidth / 2 - (int)gapSize;
            }
            float leftSize = Math.Min(listingStandard.ColumnWidth - (int)gapSize, pixels);
            float rightSize = listingStandard.ColumnWidth - leftSize - 2 * (int)gapSize;
            Rect baseRect = listingStandard.GetRect(Math.Max(columnHeight(leftCol, leftSize, selectedMod), columnHeight(rightCol, rightSize, selectedMod)));
            Rect leftRect = baseRect.LeftPartPixels(leftSize);
            Rect rightRect = baseRect.RightPartPixels(rightSize);
            Listing_Standard lListing = new Listing_Standard();
            lListing.Begin(leftRect);
            lListing.verticalSpacing = listingStandard.verticalSpacing;
            foreach (SettingContainer setting in leftCol)
            {
                setting.DrawSetting(lListing, selectedMod);
            }
            lListing.End();
            Listing_Standard rListing = new Listing_Standard();
            if (drawLine)
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

        protected override bool SetDefaultValue(string modId)
        {
            if (!DefaultValueSettingsList(modId, leftCol, "leftCol"))
            {
                return false;
            }
            if (!DefaultValueSettingsList(modId, rightCol, "rightCol"))
            {
                return false;
            }
            return true;
        }

        protected override bool Init()
        {
            if (!InitializeSettingsList(leftCol, "leftCol"))
            {
                return false;
            }
            if (!InitializeSettingsList(rightCol, "rightCol"))
            {
                return false;
            }
            return true;
        }

        protected override int CalcHeight(float width, string selectedMod)
        {
            if (pixels < 0)
            {
                pixels = width / 2 - (int)gapSize;
            }
            float leftSize = Math.Min(width - (int)gapSize, pixels);
            return Math.Max(columnHeight(leftCol, leftSize, selectedMod), columnHeight(rightCol, width - leftSize - (int)gapSize, selectedMod));
        }

        protected override bool PreClose(string selectedMod)
        {
            if (!DoPreCloseSettingsList(selectedMod, leftCol, "leftCol"))
            {
                return false;
            }
            if (!DoPreCloseSettingsList(selectedMod, rightCol, "rightCol"))
            {
                return false;
            }
            return true;
        }
    }
}