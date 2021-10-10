using System;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class HeightTool : SettingContainer
    {
        private int h = 22;

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return Math.Max(22, h);
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            Widgets.DrawBox(inRect);
            Rect numericRect = inRect.LeftPartPixels(120).TopPartPixels(22);
            string buffer = h.ToString();
            Widgets.TextFieldNumericLabeled(numericRect, "Height: ", ref h, ref buffer, 0, 99999);
        }
    }
}