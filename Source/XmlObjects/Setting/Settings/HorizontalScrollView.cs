using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class HorizontalScrollView : SettingContainer
    {
        public float innerWidth = 1000;
        public List<SettingContainer> settings;

        private Vector2 horPos = Vector2.zero;

        protected override bool Init(string selectedMod)
        {
            return InitializeSettingsList(selectedMod, settings);
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return CalculateHeightSettingsList(innerWidth, selectedMod, settings) + 16f;
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            Rect scrollRect = new Rect(0, 0, innerWidth, inRect.height - 16f);
            Widgets.BeginScrollView(inRect, ref horPos, scrollRect);
            Widgets.ScrollHorizontal(inRect, ref horPos, scrollRect);
            DrawSettingsList(scrollRect, selectedMod, settings);
            Widgets.EndScrollView();
        }
    }
}