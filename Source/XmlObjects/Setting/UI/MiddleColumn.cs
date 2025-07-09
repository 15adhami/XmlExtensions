using System;
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

        protected override bool Init(string selectedMod)
        {
            return InitializeSettingsList(selectedMod, settings);
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            if (pixels > 0)
            {
                return CalculateHeightSettingsList(pixels, selectedMod, settings);
            }
            else
            {
                return CalculateHeightSettingsList(width * split, selectedMod, settings);
            }
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            if (pixels > 0)
            {
                Rect middleRect = inRect.MiddlePartPixels(pixels, inRect.height);
                DrawSettingsList(middleRect, selectedMod, settings);
            }
            else
            {
                Rect middleRect = inRect.MiddlePartPixels(inRect.width * split, inRect.height);
                DrawSettingsList(middleRect, selectedMod, settings);
            }
        }

        internal override bool PreOpen(string selectedMod)
        {
            return PreOpenSettingsList(selectedMod, settings);
        }
    }
}