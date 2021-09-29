﻿using System;
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

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            if (pixels < 0)
            {
                pixels = listingStandard.ColumnWidth / 2 - (int)gapSize;
            }
            float leftSize = Math.Min(listingStandard.ColumnWidth - (int)gapSize, pixels);
            float rightSize = listingStandard.ColumnWidth - leftSize - 2*(int)gapSize;
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

        public override bool SetDefaultValue(string modId)
        {
            int c = 0;
            if (leftCol != null)
            {
                c = 0;
                foreach (SettingContainer setting in leftCol)
                {
                    if (!setting.SetDefaultValue(modId))
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
            if (leftCol != null)
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

        public override int getHeight(float width, string selectedMod)
        {
            if(pixels<0)
            {
                pixels = width / 2 - (int)gapSize;
            }
            float leftSize = Math.Min(width - (int)gapSize, pixels); 
            return Math.Max(columnHeight(leftCol, leftSize, selectedMod), columnHeight(rightCol, width - leftSize - (int)gapSize, selectedMod));
        }
    }
}
