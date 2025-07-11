﻿using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class DrawColor : SettingContainer
    {
        public Color color = new();
        public string key;
        public int colorSize = 22;
        public int border = 2;
        public Anchor anchor = Anchor.Middle;
        public string tooltip;
        public string tKeyTip;

        public enum Anchor
        {
            Left,
            Middle,
            Right
        }

        protected override float CalculateHeight(float width)
        {
            return colorSize + border * 2;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            Color drawColor = color;
            if (!string.IsNullOrEmpty(key))
            {
                string settingValue = SettingsManager.GetSetting(modId, key);
                if (!string.IsNullOrEmpty(settingValue))
                {
                    try
                    {
                        drawColor = ParseHelper.FromString<Color>(settingValue);
                    }
                    catch
                    {
                        // Parsing failed; keep default color
                    }
                }
            }

            float boxSize = colorSize + border * 2;
            Rect rect = inRect.TopPartPixels(boxSize);

            Rect alignedRect = anchor switch
            {
                Anchor.Left => new Rect(rect.x, rect.y, boxSize, boxSize),
                Anchor.Middle => new Rect(rect.x + (rect.width - boxSize) / 2f, rect.y, boxSize, boxSize),
                Anchor.Right => new Rect(rect.xMax - boxSize, rect.y, boxSize, boxSize),
                _ => rect
            };

            Widgets.DrawLightHighlight(alignedRect);
            if (drawColor.IndistinguishableFrom(color))
            {
                Widgets.DrawBox(alignedRect);
            }

            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(alignedRect, Helpers.TryTranslate(tooltip, tKeyTip));
            }

            Rect inner = new(alignedRect.x + border, alignedRect.y + border, colorSize, colorSize);
            Widgets.DrawBoxSolid(inner, drawColor);
        }
    }
}
