using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class MiddleColumn : SettingContainer
    {
        public float split = 0.50f;
        public float pixels = -1f;
        public List<SettingContainer> settings;

        protected override bool Init()
        {
            addDefaultSpacing = false;
            return InitializeContainers(settings);
        }

        protected override float CalculateHeight(float width)
        {
            if (pixels > 0) { return CalculateHeightSettingsList(pixels, settings); }
            else { return CalculateHeightSettingsList(width * split, settings); }
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            if (pixels > 0)
            {
                Rect middleRect = inRect.MiddlePartPixels(pixels, inRect.height);
                DrawSettingsList(middleRect, settings);
            }
            else
            {
                Rect middleRect = inRect.MiddlePartPixels(inRect.width * split, inRect.height);
                DrawSettingsList(middleRect, settings);
            }
        }
    }
}