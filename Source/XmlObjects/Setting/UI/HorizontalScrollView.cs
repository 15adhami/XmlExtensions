using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class HorizontalScrollView : SettingContainer
    {
        public float innerWidth = 1000;
        public List<SettingContainer> settings;

        private Vector2 horPos = Vector2.zero;

        protected override bool Init()
        {
            return InitializeContainers(menuDef, settings);
        }

        protected override float CalculateHeight(float width)
        {
            return CalculateHeightSettingsList(innerWidth, settings) + 16f;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            Rect scrollRect = new Rect(0, 0, innerWidth, inRect.height - 16f);
            Widgets.BeginScrollView(inRect, ref horPos, scrollRect);
            Widgets.ScrollHorizontal(inRect, ref horPos, scrollRect);
            DrawSettingsList(scrollRect, settings);
            Widgets.EndScrollView();
        }

        internal override bool PreOpen()
        {
            return PreOpenContainers(settings);
        }

        internal override bool PostClose()
        {
            return PostCloseContainers(settings);
        }
    }
}