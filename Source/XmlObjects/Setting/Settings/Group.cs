using System.Collections.Generic;
using UnityEngine;

namespace XmlExtensions.Setting
{
    internal class Group : SettingContainer
    {
        public List<SettingContainer> settings;

        protected override bool Init(string selectedMod)
        {
            return InitializeSettingsList(selectedMod, settings);
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return CalculateHeightSettingsList(width, selectedMod, settings);
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            DrawSettingsList(inRect, selectedMod, settings);
        }

        internal override bool PreOpen(string selectedMod)
        {
            return PreOpenSettingsList(selectedMod, settings);
        }
    }
}