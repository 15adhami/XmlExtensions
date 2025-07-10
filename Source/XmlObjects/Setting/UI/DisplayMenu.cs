using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class DisplayMenu : SettingContainer
    {
        public string label;
        public string menu;
        public string tKey;

        protected override bool Init(string selectedMod)
        {
            if (label == null)
            {
                label = "Open";
                tKey = "XmlExtensions_Open";
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 30;
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