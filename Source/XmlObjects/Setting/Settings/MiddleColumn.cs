using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class MiddleColumn : SettingContainer
    {
        public float split = 0.50f;
        public List<SettingContainer> settings;

        protected override bool Init()
        {
            return InitializeSettingsList(settings);
        }

        protected override bool SetDefaultValue(string modId)
        {
            return SetDefaultValueSettingsList(modId, settings);
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return GetHeightSettingsList(width * split, selectedMod, settings);
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            Rect middleRect = inRect.LeftPart(split/2f+0.5f).RightPart(split/(split / 2f + 0.5f));
            DrawSettingsList(middleRect, selectedMod, settings);
        }

        protected override bool PreClose(string selectedMod)
        {
            return DoPreCloseSettingsList(selectedMod, settings);
        }
    }
}
