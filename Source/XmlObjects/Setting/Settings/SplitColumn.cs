using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class SplitColumn : SettingContainer
    {
        public float split = 0.50f;
        public List<SettingContainer> leftCol;
        public List<SettingContainer> rightCol;
        public bool drawLine = false;
        private int gapSize = 6;

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
            return Math.Max(CalculateHeightSettingsList(width * split - gapSize / 2f, selectedMod, leftCol), CalculateHeightSettingsList(width * (1 - split) - gapSize / 2f, selectedMod, rightCol));
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            Rect leftRect = inRect.LeftPartPixels(inRect.width * split - (gapSize / 2f));
            Rect rightRect = inRect.RightPartPixels(inRect.width * (1 - split) - (gapSize / 2f));
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

        internal override bool PreOpen(string selectedMod)
        {
            if (!PreOpenSettingsList(selectedMod, leftCol, "leftCol"))
            {
                return false;
            }
            else
                return PreOpenSettingsList(selectedMod, rightCol, "rightCol");
        }
    }
}