using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class ScrollArea : SettingContainer
    {
        public float height = 72;
        public float innerWidth = -1;
        public List<SettingContainer> settings;

        private Vector2 horPos = Vector2.zero;
        private Vector2 vertPos = Vector2.zero;

        protected override bool Init(string selectedMod)
        {
            if (innerWidth < 0)
            {
                innerWidth = 1000;
            }
            return InitializeSettingsList(selectedMod, settings);
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return height;
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            float h = GetHeightSettingsList(innerWidth, selectedMod, settings);
            Rect vertRect = new Rect(0, 0, inRect.width, height);
            Rect horRect = new Rect(0, 0, inRect.width - 20f, height);
            Rect scrollRect = new Rect(0, 0, innerWidth, h);
            
            Widgets.ScrollHorizontal(horRect, ref horPos, scrollRect);
            Widgets.BeginScrollView(vertRect, ref vertPos, scrollRect);
            Rect rect2 = new Rect(0f, 0f, innerWidth, 99999f);
            DrawSettingsList(rect2, selectedMod, settings);
            Widgets.EndScrollView();
        }
    }
}
