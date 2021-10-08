using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class SplitColumn : SettingContainer
    {
        public float split = 0.50f;
        public List<SettingContainer> leftCol;
        public List<SettingContainer> rightCol;
        public bool drawLine = false;
        private Spacing gapSize = Spacing.Small;

        protected override bool Init(string selectedMod)
        {
            if (!InitializeSettingsList(selectedMod, leftCol, "leftCol"))
            {
                return false;
            }
            if (!InitializeSettingsList(selectedMod, rightCol, "rightCol"))
            {
                return false;
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return Math.Max(GetHeightSettingsList(width * split - (float)gapSize, selectedMod, leftCol), GetHeightSettingsList(width * (1 - split) - (float)gapSize, selectedMod, rightCol));
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            Rect leftRect = inRect.LeftPartPixels(inRect.width*split - ((int)gapSize));
            Rect rightRect = inRect.RightPartPixels(inRect.width * (1 - split) - ((int)gapSize));
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
