using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class EmbedMenu : SettingContainer
    {
        public string menu;

        private List<SettingContainer> settings;

        protected override bool Init()
        {
            addDefaultSpacing = false;
            settings = DefDatabase<SettingsMenuDef>.GetNamed(menu).settings;
            return InitializeContainers(modId, settings);
        }

        protected override float CalculateHeight(float width)
        {
            return CalculateHeightSettingsList(width, settings);
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            DrawSettingsList(inRect, settings);
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