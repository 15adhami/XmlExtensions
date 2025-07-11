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
        public int gapSize = 6;
        public bool drawLine = false;
        public Anchor anchor = Anchor.Top;
        public List<SettingContainer> leftCol;
        public List<SettingContainer> rightCol;
        public List<float> padColumns;

        public enum Anchor
        {
            Top,
            Middle,
            Bottom,
            Aligned
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
            float leftPad = padColumns?.Count > 0 ? padColumns[0] : 0f;
            float rightPad = padColumns?.Count > 1 ? padColumns[1] : 0f;

            if (anchor == Anchor.Aligned)
            {
                float leftWidth, rightWidth;

                if (this.width >= 0)
                {
                    leftWidth = Math.Min(width, this.width - gapSize / 2f);
                    rightWidth = Math.Max(width - leftWidth - gapSize, 0);
                }
                else
                {
                    leftWidth = width * split - gapSize / 2f;
                    rightWidth = width * (1 - split) - gapSize / 2f;
                }

                int count = Math.Max(leftCol?.Count ?? 0, rightCol?.Count ?? 0);
                float height = 0f;

                for (int i = 0; i < count; i++)
                {
                    float lh = (i < (leftCol?.Count ?? 0)) ? leftCol[i].GetHeight(leftWidth, selectedMod) : 0f;
                    float rh = (i < (rightCol?.Count ?? 0)) ? rightCol[i].GetHeight(rightWidth, selectedMod) : 0f;
                    height += Math.Max(lh, rh);
                }

                leftHeight = height + leftPad;
                rightHeight = height + rightPad;
                totalHeight = Math.Max(leftHeight, rightHeight);
                return totalHeight;
            }
            else
            {
                if (this.width >= 0)
                {
                    float leftSize = Math.Min(width, this.width - gapSize / 2f);
                    leftHeight = CalculateHeightSettingsList(leftSize, selectedMod, leftCol) + leftPad;
                    rightHeight = CalculateHeightSettingsList(Math.Max(width - leftSize - gapSize, 0), selectedMod, rightCol) + rightPad;
                }
                else
                {
                    leftHeight = CalculateHeightSettingsList(width * split - gapSize / 2f, selectedMod, leftCol) + leftPad;
                    rightHeight = CalculateHeightSettingsList(width * (1 - split) - gapSize / 2f, selectedMod, rightCol) + rightPad;
                }

                totalHeight = Math.Max(leftHeight, rightHeight);
                return totalHeight;
            }
        }



        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            float leftPad = padColumns?.Count > 0 ? padColumns[0] : 0f;
            float rightPad = padColumns?.Count > 1 ? padColumns[1] : 0f;

            float leftWidth, rightWidth;

            if (width >= 0)
            {
                leftWidth = Math.Min(inRect.width, width - gapSize / 2f);
                rightWidth = Math.Max(inRect.width - leftWidth - gapSize, 0);
            }
            else
            {
                leftWidth = inRect.width * split - gapSize / 2f;
                rightWidth = inRect.width * (1 - split) - gapSize / 2f;
            }

            if (anchor == Anchor.Aligned)
            {
                float leftY = inRect.y + leftPad;
                float rightY = inRect.y + rightPad;
                int count = Math.Max(leftCol?.Count ?? 0, rightCol?.Count ?? 0);

                for (int i = 0; i < count; i++)
                {
                    SettingContainer left = (i < (leftCol?.Count ?? 0)) ? leftCol[i] : null;
                    SettingContainer right = (i < (rightCol?.Count ?? 0)) ? rightCol[i] : null;

                    float lh = left?.GetHeight(leftWidth, selectedMod) ?? 0f;
                    float rh = right?.GetHeight(rightWidth, selectedMod) ?? 0f;
                    float pairHeight = Math.Max(lh, rh);

                    if (left != null)
                    {
                        float yOff = (pairHeight - lh) / 2f;
                        Rect rect = new Rect(inRect.x, leftY + yOff, leftWidth, lh);
                        left.DrawSetting(rect, selectedMod);
                    }

                    if (right != null)
                    {
                        float yOff = (pairHeight - rh) / 2f;
                        Rect rect = new Rect(inRect.x + inRect.width - rightWidth, rightY + yOff, rightWidth, rh);
                        right.DrawSetting(rect, selectedMod);
                    }

                    leftY += pairHeight;
                    rightY += pairHeight;
                }

                if (drawLine)
                {
                    Color color = GUI.color;
                    GUI.color = color * new Color(1f, 1f, 1f, 0.4f);
                    GUI.DrawTexture(new Rect(inRect.center.x, inRect.y, 1f, totalHeight), BaseContent.WhiteTex);
                    GUI.color = color;
                }
            }
            else
            {
                float leftYOffset = GetYOffset(anchor, totalHeight, leftHeight) + leftPad;
                float rightYOffset = GetYOffset(anchor, totalHeight, rightHeight) + rightPad;

                Rect leftRect = new Rect(inRect.x, inRect.y + leftYOffset, leftWidth, leftHeight - leftPad);
                Rect rightRect = new Rect(inRect.x + inRect.width - rightWidth, inRect.y + rightYOffset, rightWidth, rightHeight - rightPad);

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