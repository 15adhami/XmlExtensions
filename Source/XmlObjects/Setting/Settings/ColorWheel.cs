using System;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class ColorWheel : KeyedSettingContainer
    {
        public float size = 128;

        private bool currentlyDragging = false;

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return size + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            Color cColor = GUI.color;
            Rect rect = inRect.TopPartPixels(size).MiddlePartPixels(size, size);
            Color color = ParseHelper.FromString<Color>(SettingsManager.GetSetting(selectedMod, key));
            Widgets.HSVColorWheel(rect, ref color, ref currentlyDragging);
            GUI.color = cColor;
            SettingsManager.SetSetting(selectedMod, key, color.ToString());
        }
    }
}
