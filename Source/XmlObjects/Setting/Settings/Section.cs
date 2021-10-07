using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class Section : SettingContainer
    {
        public float height = -1f;
        public List<SettingContainer> settings;
        public float padding = 4f;

        protected override bool Init()
        {
            return InitializeSettingsList(settings);
        }

        protected override bool SetDefaultValue(string modId)
        {
            return DefaultValueSettingsList(modId, settings);
        }
        protected override float CalcHeight(float width, string selectedMod)
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
                        h += setting.GetHeight(width - padding * 2f, selectedMod);
                    }
                }
                return h + padding * 2;
            }
            else
            {
                return height + padding * 2;
            }
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            if(settings.Count > 0)
            {
                Color curColor = GUI.color;
                GUI.color = Widgets.MenuSectionBGFillColor * curColor;
                GUI.DrawTexture(inRect, BaseContent.WhiteTex);
                GUI.color = new ColorInt(135, 135, 135).ToColor * curColor;
                Widgets.DrawBox(inRect, 1, null);
                GUI.color = curColor;
                Rect rect2 = new Rect(inRect.x + padding, inRect.y + padding, inRect.width - padding * 2f, inRect.height - padding * 2f);
                DrawSettingsList(rect2, selectedMod, settings);
            }            
        }

        protected override bool PreClose(string selectedMod)
        {
            return DoPreCloseSettingsList(selectedMod, settings);
        }
    }
}
