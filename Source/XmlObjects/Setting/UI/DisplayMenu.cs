using System;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    [Obsolete]
    internal class DisplayMenu : SettingContainer
    {
        public string label;
        public string menu;
        public string tKey;

        protected override bool Init()
        {
            XmlMod.WarnUsingObselete(modId, "");
            if (label == null)
            {
                label = "Open";
                tKey = "XmlExtensions_Open";
            }
            return true;
        }

        protected override float CalculateHeight(float width)
        {
            return 30;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            if (Widgets.ButtonText(inRect, Helpers.TryTranslate(label, tKey)))
            {
                SetActiveMenu(menu);
            }
        }
    }
}