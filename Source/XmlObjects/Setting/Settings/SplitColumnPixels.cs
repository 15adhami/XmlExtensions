using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class SplitColumnPixels : SettingContainer
    {
        public float pixels = -1f;
        public List<SettingContainer> leftCol;
        public List<SettingContainer> rightCol;
        public bool drawLine = false;
        private Spacing gapSize = Spacing.Small;

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

        protected override float CalcHeight(float width, string selectedMod)
        {
            if (pixels < 0)
            {
                pixels = width / 2 - (int)gapSize;
            }
            float leftSize = Math.Min(width - (int)gapSize, pixels);
            return Math.Max(GetHeightSettingsList(leftSize, selectedMod, leftCol), GetHeightSettingsList(width - leftSize - (int)gapSize, selectedMod, leftCol));
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            float leftSize = Math.Min(inRect.width - (int)gapSize, pixels);
            float rightSize = inRect.width - leftSize - 2 * (int)gapSize;
            Rect leftRect = inRect.LeftPartPixels(leftSize);
            Rect rightRect = inRect.RightPartPixels(rightSize);
            DrawSettingsList(leftRect, selectedMod, leftCol);
            if (drawLine)
            {
                Color color = GUI.color;
                GUI.color = color * new Color(1f, 1f, 1f, 0.4f);
                GUI.DrawTexture(new Rect(inRect.center.x, rightRect.yMin, 1f, rightRect.height), BaseContent.WhiteTex);
                GUI.color = color;
            }
            DrawSettingsList(rightRect, selectedMod, rightCol);
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

        private enum Spacing : int
        {
            None = 0,
            Tiny = 2,
            Small = 3,
            Medium = 5,
            Large = 9,
            Huge = 15
        }
    }
}