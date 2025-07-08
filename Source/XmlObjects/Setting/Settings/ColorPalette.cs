using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    class ColorPalette : KeyedSettingContainer
    {
        public int colorSize = 22;
        public int spacing = 2;
        public Anchor anchor = Anchor.Middle;
        public List<List<Color>> colors;

        public enum Anchor
        {
            Left,
            Middle,
            Right
        }

        private float height = -1;

        protected override float CalculateHeight(float width, string selectedMod)
        {
            if (height == -1 && colors != null && colors[0] != null)
            {
                int color_size = colorSize + spacing * 2;
                height = colors.Count * (color_size + spacing);
            }
            else if (height == -1)
            {
                height = 0;
            }
            return height;
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            if (height >= 0)
            {
                float height_temp = height;
                Color color = ParseHelper.FromString<Color>(SettingsManager.GetSetting(selectedMod, key));
                int row_num = 0;
                foreach (List<Color> row in colors)
                {
                    row_num++;
                    Rect rect;
                    int color_size = (colorSize + spacing * 2);
                    float rowWidth = row.Count * color_size + (row.Count - 1) * spacing;
                    if (anchor == Anchor.Left)
                    {
                        rect = inRect.LeftPartPixels(rowWidth).TopPartPixels(row_num * (color_size + spacing)).BottomPartPixels(color_size);
                    }
                    else if (anchor == Anchor.Middle)
                    {
                        rect = inRect.MiddlePartPixels(rowWidth, inRect.height).TopPartPixels(row_num * (color_size + spacing)).BottomPartPixels(color_size);
                    }
                    else
                    {
                        rect = inRect.RightPartPixels(rowWidth).TopPartPixels(row_num * (color_size + spacing)).BottomPartPixels(color_size);
                    }
                    Widgets.ColorSelector(rect, ref color, row, out height_temp, null, colorSize, spacing);
                }
                SettingsManager.SetSetting(selectedMod, key, color.ToString());
            }
        }
    }
}
