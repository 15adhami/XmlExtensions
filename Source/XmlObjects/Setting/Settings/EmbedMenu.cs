using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class EmbedMenu : SettingContainer
    {
        public string menu;

        private List<SettingContainer> settings;

        protected override bool Init(string selectedMod)
        {
            settings = DefDatabase<SettingsMenuDef>.GetNamed(menu).settings;
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return CalculateHeightSettingsList(width, selectedMod, settings);
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            DrawSettingsList(inRect, selectedMod, settings);
        }
    }
}