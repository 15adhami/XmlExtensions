using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class SplitColumns : SettingContainer
    {
        public List<float> splits;
        public List<float> widths;
        public List<List<SettingContainer>> settings;
        public List<Position> positions = [Position.Top];
        public bool drawLine = false;
        public int gapSize = 6;

        public enum Position
        {
            Top,
            Middle,
            Bottom
        }

        private List<float> cachedHeights = new();
        private float totalHeight = 0f;

        internal override bool PreOpen(string selectedMod)
        {
            foreach (List<SettingContainer> list in settings)
            {
                if (!PreOpenSettingsList(selectedMod, list))
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool Init(string selectedMod)
        {
            int c = 0;
            foreach (List<SettingContainer> list in settings)
            {
                c++;
                if (!InitializeSettingsList(selectedMod, list, c.ToString()))
                {
                    return false;
                }
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            cachedHeights.Clear();
            float offset = 0;
            float maxHeight = 0;

            List<float> activeSplits = GetActiveSplits();

            for (int i = 0; i < settings.Count; i++)
            {
                float colWidth = GetColumnWidth(i, width, offset, activeSplits);
                float colHeight = CalculateHeightSettingsList(colWidth, selectedMod, settings[i]);
                cachedHeights.Add(colHeight);
                maxHeight = Math.Max(maxHeight, colHeight);
                offset += colWidth + gapSize;
            }

            totalHeight = maxHeight;
            return totalHeight;
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            float offsetX = 0;
            float totalWidth = inRect.width;

            List<float> activeSplits = GetActiveSplits();

            for (int i = 0; i < settings.Count; i++)
            {
                float colWidth = GetColumnWidth(i, totalWidth, offsetX, activeSplits);
                float colHeight = cachedHeights[i];
                Position colPosition = GetPosition(i);
                float yOffset = GetYOffset(colPosition, totalHeight, colHeight);

                Rect colRect = new Rect(inRect.x + offsetX, inRect.y + yOffset, colWidth, colHeight);
                DrawSettingsList(colRect, selectedMod, settings[i]);

                if (drawLine && i < settings.Count - 1)
                {
                    Color old = GUI.color;
                    GUI.color = old * new Color(1f, 1f, 1f, 0.4f);
                    GUI.DrawTexture(new Rect(inRect.x + offsetX + colWidth + gapSize / 2f, inRect.y, 1f, totalHeight), BaseContent.WhiteTex);
                    GUI.color = old;
                }

                offsetX += colWidth + gapSize;
            }
        }

        private float GetColumnWidth(int index, float totalWidth, float offset, List<float> activeSplits)
        {
            if (widths != null && index < widths.Count)
            {
                return Math.Min(totalWidth - offset - gapSize, widths[index]);
            }
            else if (index < activeSplits.Count)
            {
                return totalWidth * activeSplits[index] - gapSize / 2f;
            }
            else
            {
                return totalWidth - offset - gapSize;
            }
        }

        private Position GetPosition(int index)
        {
            if (positions != null && index < positions.Count)
            {
                return positions[index];
            }
            return Position.Top;
        }

        private float GetYOffset(Position pos, float total, float col)
        {
            return pos switch
            {
                Position.Top => 0f,
                Position.Middle => (total - col) / 2f,
                Position.Bottom => total - col,
                _ => 0f
            };
        }

        private List<float> GetActiveSplits()
        {
            if ((splits == null || splits.Count == 0) &&
                (widths == null || widths.Count == 0) &&
                settings != null && settings.Count > 0)
            {
                float evenSplit = 1f / settings.Count;
                List<float> fallbackSplits = new(settings.Count);
                for (int i = 0; i < settings.Count; i++)
                {
                    fallbackSplits.Add(evenSplit);
                }
                return fallbackSplits;
            }
            return splits ?? new List<float>();
        }
    }
}
