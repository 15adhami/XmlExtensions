using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class Group : SettingContainer
    {
        public List<SettingContainer> settings;

        protected override bool Init(string selectedMod)
        {
            return InitializeSettingsList(selectedMod, settings);
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return GetHeightSettingsList(width, selectedMod, settings);
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            DrawSettingsList(inRect, selectedMod, settings);
        }
    }
}
