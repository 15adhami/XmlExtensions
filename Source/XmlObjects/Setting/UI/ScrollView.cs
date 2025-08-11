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

        protected override bool Init()
        {
            searchType = SearchType.SearchAllAndHighlight;
            return InitializeContainers(settings);
        }

        protected override float CalculateHeight(float width)
        {
            return height;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            Rect scrollRect = new Rect(0, 0, inRect.width - GenUI.ScrollBarWidth, CalculateHeightSettingsList(inRect.width - GenUI.ScrollBarWidth, settings));
            Widgets.BeginScrollView(inRect, ref scrollPos, scrollRect);
            DrawSettingsList(scrollRect, settings);
            Widgets.EndScrollView();
        }
    }
}