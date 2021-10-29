using UnityEngine;

namespace XmlExtensions.Setting
{
    internal class Gap : SettingContainer
    {
        public int spacing = 24;

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return spacing;
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
        }
    }
}