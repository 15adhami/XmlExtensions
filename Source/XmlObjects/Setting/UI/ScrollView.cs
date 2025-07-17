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
            return InitializeContainers(menuDef, settings);
        }

        protected override float CalculateHeight(float width)
        {
            return height;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            Rect scrollRect = new Rect(0, 0, inRect.width - 16f, CalculateHeightSettingsList(inRect.width - 16f, settings));
            Widgets.BeginScrollView(inRect, ref scrollPos, scrollRect);
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