﻿using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class DisplayMenu : SettingContainer
    {
        public string label;
        public string menu;
        public string tKey;

        protected override bool Init()
        {
            if (label == null)
            {
                label = "Open";
                tKey = "XmlExtensions_Open";
            }
            return true;
        }

        protected override float CalcHeight(float width, string selectedMod)
        {
            return 30 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            if (Widgets.ButtonText(inRect, Helpers.TryTranslate(label, tKey)))
            {
                SetActiveMenu(menu);
            }
        }        
    }
}
