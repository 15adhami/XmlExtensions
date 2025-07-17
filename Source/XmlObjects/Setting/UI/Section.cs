using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class Section : SettingContainer
    {
        public float height = -1f;
        public List<SettingContainer> settings;
        public float padding = 4f;
        public bool drawBorder = true;

        protected override bool Init()
        {
            addDefaultSpacing = false;
            return InitializeContainers(menuDef, settings);
        }

        protected override float CalculateHeight(float width)
        {
            if (settings.Count == 0)
                return 0;
            if (height < 0)
            {
                float h = 0;
                if (settings != null)
                {
                    foreach (SettingContainer setting in settings)
                    {
                        h += setting.GetHeight(width - padding * 2f);
                    }
                }
                return h + padding * 2;
            }
            else
            {
                return height + padding * 2;
            }
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            if (CalculateHeight(inRect.width) > 0)
            {
                Color curColor = GUI.color;
                GUI.color = Widgets.MenuSectionBGFillColor * curColor;
                GUI.DrawTexture(inRect, BaseContent.WhiteTex);
                if (drawBorder)
                {
                    GUI.color = new ColorInt(135, 135, 135).ToColor * curColor;
                    Widgets.DrawBox(inRect, 1, null);
                }
                GUI.color = curColor;
                Rect rect2 = new Rect(inRect.x + padding, inRect.y + padding, inRect.width - padding * 2f, inRect.height - padding * 2f);
                DrawSettingsList(rect2, settings);
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