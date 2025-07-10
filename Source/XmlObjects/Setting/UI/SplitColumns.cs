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
        public List<Anchor> anchors = [Anchor.Top];
        public bool drawLine = false;
        public int gapSize = 6;

        public enum Anchor
        {
            Top,
            Middle,
            Bottom,
            Aligned
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
            addDefaultSpacing = false;

            if (anchors != null && anchors.Contains(Anchor.Aligned))
            {
                int alignedCount = 0;
                for (int i = 0; i < anchors.Count; i++)
                {
                    if (anchors[i] == Anchor.Aligned)
                        alignedCount++;
                }

                if (alignedCount < 2 && settings.Count >= 2)
                {
                    Error("At least two columns must use 'Aligned' if alignment is to be applied.");
                    return false;
                }
            }

            for (int i = 0; i < settings.Count; i++)
            {
                if (!InitializeSettingsList(selectedMod, settings[i], i.ToString()))
                {
                    return false;
                }
            }

            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            cachedHeights.Clear();
            float offset = 0f;

            List<float> activeSplits = GetActiveSplits();
            bool anyAligned = anchors != null && anchors.Contains(Anchor.Aligned);

            if (anyAligned)
            {
                // Determine column widths
                List<float> columnWidths = new();
                for (int i = 0; i < settings.Count; i++)
                {
                    float colWidth = GetColumnWidth(i, width, offset, activeSplits);
                    columnWidths.Add(colWidth);
                    offset += colWidth + gapSize;
                }

                // Determine maximum number of rows (longest column)
                int maxRowCount = 0;
                foreach (var column in settings)
                {
                    maxRowCount = Math.Max(maxRowCount, column?.Count ?? 0);
                }

                // Determine per-row max height
                List<float> rowHeights = new();
                for (int row = 0; row < maxRowCount; row++)
                {
                    float maxRowHeight = 0f;
                    for (int col = 0; col < settings.Count; col++)
                    {
                        if (anchors[col] == Anchor.Aligned && row < (settings[col]?.Count ?? 0))
                        {
                            float h = settings[col][row].GetHeight(columnWidths[col], selectedMod);
                            maxRowHeight = Math.Max(maxRowHeight, h);
                        }
                    }
                    rowHeights.Add(maxRowHeight);
                }

                // Accumulate per-column total heights
                for (int col = 0; col < settings.Count; col++)
                {
                    float totalColHeight = 0f;
                    if (anchors[col] == Anchor.Aligned)
                    {
                        // Use rowHeights for aligned columns
                        for (int row = 0; row < rowHeights.Count; row++)
                        {
                            if (row < (settings[col]?.Count ?? 0))
                            {
                                totalColHeight += rowHeights[row];
                            }
                        }
                    }
                    else
                    {
                        // Use standard stacked height
                        float colHeight = CalculateHeightSettingsList(columnWidths[col], selectedMod, settings[col]);
                        totalColHeight = colHeight;
                    }
                    cachedHeights.Add(totalColHeight);
                }

                totalHeight = 0f;
                foreach (float rowH in rowHeights)
                {
                    totalHeight += rowH;
                }

                // Include unaligned columns that might be taller
                for (int col = 0; col < settings.Count; col++)
                {
                    if (anchors[col] != Anchor.Aligned)
                    {
                        totalHeight = Math.Max(totalHeight, cachedHeights[col]);
                    }
                }

                return totalHeight;
            }

            // If no aligned anchors, fallback to default stacked mode
            float maxHeight = 0;
            offset = 0;

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
                Anchor colPosition = GetPosition(i);
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

        private Anchor GetPosition(int index)
        {
            if (anchors != null && index < anchors.Count)
            {
                return anchors[index];
            }
            return Anchor.Top;
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
