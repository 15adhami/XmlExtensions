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

        private float cachedLeftHeight, cachedRightHeight, cachedTotalHeight;
        private float cachedLeftWidth, cachedRightWidth;
        private List<float> cachedRowHeights = new();

        protected override bool Init()
        {
            addDefaultSpacing = false;
            if (!InitializeContainers(modId, leftCol, "leftCol"))
            {
                return false;
            }
            if (!InitializeContainers(modId, rightCol, "rightCol"))
            {
                return false;
            }
            return true;
        }

        protected override float CalculateHeight(float width)
        {
            cachedRowHeights.Clear();
            float leftPad = padColumns?.Count > 0 ? padColumns[0] : 0f;
            float rightPad = padColumns?.Count > 1 ? padColumns[1] : 0f;
            float leftWidth, rightWidth;

            // Compute and cache column widths
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
            cachedLeftWidth = leftWidth;
            cachedRightWidth = rightWidth;

            // Compute and cache column heights
            if (anchor == Anchor.Aligned)
            {
                int count = Math.Max(leftCol?.Count ?? 0, rightCol?.Count ?? 0);
                float height = 0f;

                for (int i = 0; i < count; i++)
                {
                    float lh = (i < (leftCol?.Count ?? 0)) ? leftCol[i].GetHeight(cachedLeftWidth) : 0f;
                    float rh = (i < (rightCol?.Count ?? 0)) ? rightCol[i].GetHeight(cachedRightWidth) : 0f;
                    float rowHeight = Math.Max(lh, rh);
                    height += rowHeight;
                    cachedRowHeights.Add(rowHeight);
                }

                cachedLeftHeight = height;
                cachedRightHeight = height;
            }
            else
            {
                cachedLeftHeight = CalculateHeightSettingsList(cachedLeftWidth, leftCol);
                cachedRightHeight = CalculateHeightSettingsList(cachedRightWidth, rightCol);
            }

            // Add padding
            float leftTotal = anchor == Anchor.Bottom ? cachedLeftHeight + leftPad : cachedLeftHeight + (anchor == Anchor.Aligned ? leftPad : 0f);
            float rightTotal = anchor == Anchor.Bottom ? cachedRightHeight + rightPad : cachedRightHeight + (anchor == Anchor.Aligned ? rightPad : 0f);

            // Compute and cache overall height
            cachedTotalHeight = Math.Max(leftTotal, rightTotal);
            return cachedTotalHeight;
        }




        protected override void DrawSettingContents(Rect inRect)
        {
            float leftPad = padColumns?.Count > 0 ? padColumns[0] : 0f;
            float rightPad = padColumns?.Count > 1 ? padColumns[1] : 0f;

            float leftWidth = cachedLeftWidth;
            float rightWidth = cachedRightWidth;

            if (anchor == Anchor.Aligned)
            {
                float leftY = inRect.y + leftPad;
                float rightY = inRect.y + rightPad;
                int count = Math.Max(leftCol?.Count ?? 0, rightCol?.Count ?? 0);

                for (int i = 0; i < cachedRowHeights.Count; i++)
                {
                    SettingContainer left = (i < (leftCol?.Count ?? 0)) ? leftCol[i] : null;
                    SettingContainer right = (i < (rightCol?.Count ?? 0)) ? rightCol[i] : null;

                    float lh = left?.GetHeight(leftWidth) ?? 0f;
                    float rh = right?.GetHeight(rightWidth) ?? 0f;
                    float pairHeight = cachedRowHeights[i];

                    if (left != null)
                    {
                        float yOff = (pairHeight - lh) / 2f;
                        Rect rect = new Rect(inRect.x, leftY + yOff, leftWidth, lh);
                        left.DrawSetting(rect);
                    }

                    if (right != null)
                    {
                        float yOff = (pairHeight - rh) / 2f;
                        Rect rect = new Rect(inRect.x + inRect.width - rightWidth, rightY + yOff, rightWidth, rh);
                        right.DrawSetting(rect);
                    }

                    leftY += pairHeight;
                    rightY += pairHeight;
                }

                if (drawLine)
                {
                    Color color = GUI.color;
                    GUI.color = color * new Color(1f, 1f, 1f, 0.4f);
                    GUI.DrawTexture(new Rect(inRect.center.x, inRect.y, 1f, cachedTotalHeight), BaseContent.WhiteTex);
                    GUI.color = color;
                }
            }
            else
            {
                float leftYOffset = GetYOffset(anchor, cachedTotalHeight, cachedLeftHeight);
                float rightYOffset = GetYOffset(anchor, cachedTotalHeight, cachedRightHeight);

                if (anchor != Anchor.Bottom) leftYOffset += leftPad;
                if (anchor != Anchor.Bottom) rightYOffset += rightPad;

                Rect leftRect = new Rect(inRect.x, inRect.y + leftYOffset, leftWidth, cachedLeftHeight);
                Rect rightRect = new Rect(inRect.x + inRect.width - rightWidth, inRect.y + rightYOffset, rightWidth, cachedRightHeight);

                DrawSettingsList(leftRect, leftCol);

                if (drawLine)
                {
                    Color color = GUI.color;
                    GUI.color = color * new Color(1f, 1f, 1f, 0.4f);
                    GUI.DrawTexture(new Rect(inRect.center.x, inRect.y, 1f, cachedTotalHeight), BaseContent.WhiteTex);
                    GUI.color = color;
                }

                DrawSettingsList(rightRect, rightCol);
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


        internal override bool PreOpen()
        {
            if (!PreOpenContainers(leftCol, "leftCol"))
            {
                return false;
            }
            else
                return PreOpenContainers(rightCol, "rightCol");
        }

        internal override bool PostClose()
        {
            if (!PostCloseContainers(leftCol, "leftCol"))
            {
                return false;
            }
            else
                return PostCloseContainers(rightCol, "rightCol");
        }
    }
}