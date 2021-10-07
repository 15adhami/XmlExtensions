using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class ScrollView : SettingContainer
    {
        public float height = 72;
        public List<SettingContainer> settings;

        private Vector2 scrollPos = Vector2.zero;

        protected override bool Init()
        {
            return InitializeSettingsList(settings);
        }

        protected override bool SetDefaultValue(string modId)
        {
            return DefaultValueSettingsList(modId, settings);
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return height;
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {          
            Rect scrollRect = new Rect(0, 0, inRect.width - 20f, GetHeightSettingsList(inRect.width - 20f, selectedMod, settings));
            Widgets.BeginScrollView(inRect, ref scrollPos, scrollRect);
            Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
            DrawSettingsList(rect2, selectedMod, settings);
            Widgets.EndScrollView();
        }

        protected override bool PreClose(string selectedMod)
        {
            return DoPreCloseSettingsList(selectedMod, settings);
        }
    }
}
