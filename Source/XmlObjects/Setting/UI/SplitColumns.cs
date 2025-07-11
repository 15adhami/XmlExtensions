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
        public List<float> padColumns;

        public enum Anchor
        {
            Top,
            Middle,
            Bottom,
            Aligned
        }

        // Caches for optimization
        private List<float> cachedHeights = new();
        private float totalHeight = 0f;
        private List<float> columnWidths = [];
        private List<float> rowHeights = [];
        private List<float> colPads = [];

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
            columnWidths.Clear();
            rowHeights.Clear();
            colPads.Clear();

            float offset = 0f;
            List<float> activeSplits = GetActiveSplits();
            bool anyAligned = anchors != null && anchors.Contains(Anchor.Aligned);

            // Precompute column widths and padding
            for (int i = 0; i < settings.Count; i++)
            {
                float colWidth = GetColumnWidth(i, width, offset, activeSplits);
                columnWidths.Add(colWidth);
                offset += colWidth + gapSize;

                float pad = padColumns?.Count > i ? padColumns[i] : 0f;
                colPads.Add(pad);
            }

            if (anyAligned)
            {
                // Determine max row count
                int maxRowCount = 0;
                foreach (var column in settings)
                {
                    maxRowCount = Math.Max(maxRowCount, column?.Count ?? 0);
                }

                // Calculate max height for each row
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

                // Calculate per-column height
                for (int col = 0; col < settings.Count; col++)
                {
                    float totalColHeight = 0f;
                    if (anchors[col] == Anchor.Aligned)
                    {
                        for (int row = 0; row < rowHeights.Count; row++)
                        {
                            if (row < (settings[col]?.Count ?? 0))
                                totalColHeight += rowHeights[row];
                        }
                    }
                    else
                    {
                        totalColHeight = CalculateHeightSettingsList(columnWidths[col], selectedMod, settings[col]);
                    }

                    float pad = colPads[col];
                    cachedHeights.Add(totalColHeight + pad);
                }

                // Final layout height considering bottom padding
                totalHeight = 0f;
                for (int col = 0; col < settings.Count; col++)
                {
                    float pad = colPads[col];
                    float rawHeight = cachedHeights[col];

                    if (anchors[col] == Anchor.Bottom)
                        totalHeight = Math.Max(totalHeight, rawHeight + pad);
                    else
                        totalHeight = Math.Max(totalHeight, rawHeight);
                }

                return totalHeight;
            }

            // Fallback for non-aligned layout
            float maxHeight = 0f;
            offset = 0f;

            for (int i = 0; i < settings.Count; i++)
            {
                float colHeight = CalculateHeightSettingsList(columnWidths[i], selectedMod, settings[i]) + colPads[i];
                cachedHeights.Add(colHeight);
                maxHeight = Math.Max(maxHeight, colHeight);
            }

            totalHeight = maxHeight;
            return totalHeight;
        }



        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            float offsetX = 0f;
            bool anyAligned = anchors.Contains(Anchor.Aligned);

            if (anyAligned)
            {
                int maxRows = rowHeights.Count;

                offsetX = 0f;
                for (int col = 0; col < settings.Count; col++)
                {
                    float x = inRect.x + offsetX;
                    float w = columnWidths[col];
                    float pad = colPads[col];

                    if (anchors[col] == Anchor.Aligned)
                    {
                        float y = inRect.y + pad;
                        for (int row = 0; row < maxRows; row++)
                        {
                            if (row < (settings[col]?.Count ?? 0))
                            {
                                SettingContainer setting = settings[col][row];
                                float h = setting.GetHeight(w, selectedMod);
                                float yOffset = (rowHeights[row] - h) / 2f;
                                Rect r = new Rect(x, y + yOffset, w, h);
                                setting.DrawSetting(r, selectedMod);
                            }
                            y += rowHeights[row];
                        }
                    }
                    else
                    {
                        float h = cachedHeights[col];
                        float yOffset = GetYOffset(anchors[col], totalHeight, h);
                        if (anchors[col] == Anchor.Bottom)
                            yOffset += pad;

                        Rect r = new Rect(x, inRect.y + yOffset, w, h);
                        DrawSettingsList(r, selectedMod, settings[col]);
                    }

                    // Divider
                    if (drawLine && col < settings.Count - 1)
                    {
                        Color old = GUI.color;
                        GUI.color = old * new Color(1f, 1f, 1f, 0.4f);
                        GUI.DrawTexture(new Rect(x + w + gapSize / 2f, inRect.y, 1f, totalHeight), BaseContent.WhiteTex);
                        GUI.color = old;
                    }

                    offsetX += w + gapSize;
                }

                return;
            }

            // Non-aligned fallback
            offsetX = 0f;
            for (int col = 0; col < settings.Count; col++)
            {
                float w = columnWidths[col];
                float h = cachedHeights[col];
                float pad = colPads[col];
                Anchor anchor = anchors[col];

                float yOffset = GetYOffset(anchor, totalHeight, h);
                if (anchor == Anchor.Bottom)
                    yOffset += pad;

                Rect colRect = new Rect(inRect.x + offsetX, inRect.y + yOffset, w, h);
                DrawSettingsList(colRect, selectedMod, settings[col]);

                if (drawLine && col < settings.Count - 1)
                {
                    Color old = GUI.color;
                    GUI.color = old * new Color(1f, 1f, 1f, 0.4f);
                    GUI.DrawTexture(new Rect(inRect.x + offsetX + w + gapSize / 2f, inRect.y, 1f, totalHeight), BaseContent.WhiteTex);
                    GUI.color = old;
                }

                offsetX += w + gapSize;
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
