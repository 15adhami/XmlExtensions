using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class ScrollArea : SettingContainer
    {
        public float height = 144;
        public float innerWidth = 1000;
        public List<SettingContainer> settings;

        private Vector2 horPos = Vector2.zero;
        private Vector2 vertPos = Vector2.zero;

        protected override bool Init(string selectedMod)
        {
            return InitializeSettingsList(selectedMod, settings);
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return height + 16;
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            Rect scrollRect = new Rect(0, 0, innerWidth, CalculateHeightSettingsList(innerWidth, selectedMod, settings));
            Widgets.BeginScrollView(inRect, ref vertPos, scrollRect);
            Widgets.ScrollHorizontal(inRect, ref horPos, scrollRect);
            DrawSettingsList(scrollRect, selectedMod, settings);
            Widgets.EndScrollView();
        }
    }
}