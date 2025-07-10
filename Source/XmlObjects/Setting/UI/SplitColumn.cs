using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class SplitColumn : SettingContainer
    {
        public float split = 0.50f;
        public float width = -1f;
        public List<SettingContainer> leftCol;
        public List<SettingContainer> rightCol;
        public Anchor anchor = Anchor.Top;
        public bool drawLine = false;
        public int gapSize = 6;

        public enum Anchor
        {
            Top,
            Middle,
            Bottom
        }

        private float leftHeight = 0;
        private float rightHeight = 0;
        private float totalHeight = 0;

        protected override bool Init(string selectedMod)
        {
            addDefaultSpacing = false;
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
            if (this.width >= 0)
            {
                float leftSize = Math.Min(width - gapSize / 2f, this.width);
                leftHeight = CalculateHeightSettingsList(leftSize, selectedMod, leftCol);
                rightHeight = CalculateHeightSettingsList(width - leftSize - gapSize, selectedMod, leftCol);
            }
            else
            {
                leftHeight = CalculateHeightSettingsList(width * split - gapSize / 2f, selectedMod, leftCol);
                rightHeight = CalculateHeightSettingsList(width * (1 - split) - gapSize / 2f, selectedMod, rightCol);
            }
            totalHeight = Math.Max(leftHeight, rightHeight);
            return totalHeight;
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            float leftWidth, rightWidth;

            if (width >= 0)
            {
                leftWidth = Math.Min(inRect.width - gapSize, width);
                rightWidth = inRect.width - leftWidth - 2 * gapSize;
            }
            else
            {
                leftWidth = inRect.width * split - gapSize / 2f;
                rightWidth = inRect.width * (1 - split) - gapSize / 2f;
            }

            float leftYOffset = GetYOffset(anchor, totalHeight, leftHeight);
            float rightYOffset = GetYOffset(anchor, totalHeight, rightHeight);

            Rect leftRect = new Rect(inRect.x, inRect.y + leftYOffset, leftWidth, leftHeight);
            Rect rightRect = new Rect(inRect.x + inRect.width - rightWidth, inRect.y + rightYOffset, rightWidth, rightHeight);

            DrawSettingsList(leftRect, selectedMod, leftCol);

            if (drawLine)
            {
                Color color = GUI.color;
                GUI.color = color * new Color(1f, 1f, 1f, 0.4f);
                GUI.DrawTexture(new Rect(inRect.center.x, inRect.y, 1f, totalHeight), BaseContent.WhiteTex);
                GUI.color = color;
            }

            DrawSettingsList(rightRect, selectedMod, rightCol);
        }

        private float GetYOffset(Anchor pos, float total, float col)
        {
            return pos switch
            {
                Anchor.Top => 0f,
                Anchor.Middle => (total - col) / 2f,
                Anchor.Bottom => total - col,
                _ => 0f
            };
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