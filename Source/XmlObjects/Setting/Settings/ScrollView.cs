using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class ScrollView : SettingContainer
    {
        public float height = 72;
        public List<SettingContainer> settings;

        private Vector2 scrollPos = Vector2.zero;

        protected override bool Init(string selectedMod)
        {
            return InitializeSettingsList(selectedMod, settings);
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return height;
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            Rect scrollRect = new Rect(0, 0, inRect.width - 16f, CalculateHeightSettingsList(inRect.width - 16f, selectedMod, settings));
            Widgets.BeginScrollView(inRect, ref scrollPos, scrollRect);
            DrawSettingsList(scrollRect, selectedMod, settings);
            Widgets.EndScrollView();
        }
    }
}