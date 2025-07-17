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

        protected override bool Init()
        {
            return InitializeContainers(menuDef, settings);
        }

        protected override float CalculateHeight(float width)
        {
            return height + 16;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            Rect scrollRect = new Rect(0, 0, innerWidth, CalculateHeightSettingsList(innerWidth, settings));
            Widgets.BeginScrollView(inRect, ref vertPos, scrollRect);
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