using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class SplitColumnPixels : SettingContainer
    {
        public float pixels = -1f;
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
            if (pixels < 0)
            {
                pixels = width / 2 - (int)gapSize;
            }
            float leftSize = Math.Min(width - (int)gapSize, pixels);
            return Math.Max(CalculateHeightSettingsList(leftSize, selectedMod, leftCol), CalculateHeightSettingsList(width - leftSize - (int)gapSize, selectedMod, leftCol));
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