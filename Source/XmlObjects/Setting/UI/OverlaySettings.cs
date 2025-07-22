using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class OverlaySettings : SettingContainer
    {
        public Anchor anchor = Anchor.Top;
        public List<SettingContainer> settings;

        private List<float> cachedHeights;
        private float tallest;

        public enum Anchor
        {
            Top,
            Middle,
            Bottom
        }

        protected override bool Init()
        {
            addDefaultSpacing = false;
            return InitializeContainers(menuDef, settings);
        }

        protected override float CalculateHeight(float width)
        {
            cachedHeights = new List<float>();
            tallest = 0f;

            foreach (SettingContainer setting in settings)
            {
                float h = setting.GetHeight(width);
                cachedHeights.Add(h);
                if (h > tallest)
                    tallest = h;
            }

            return tallest;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            for (int i = 0; i < settings.Count; i++)
            {
                SettingContainer setting = settings[i];
                float h = cachedHeights[i];
                float yOffset = anchor switch
                {
                    Anchor.Top => 0f,
                    Anchor.Middle => (tallest - h) / 2f,
                    Anchor.Bottom => (tallest - h),
                    _ => 0f
                };

                Rect settingRect = new Rect(inRect.x, inRect.y + yOffset, inRect.width, h);
                setting.DrawSetting(settingRect);
            }
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
