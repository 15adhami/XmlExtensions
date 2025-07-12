using UnityEngine;

namespace XmlExtensions.Setting
{
    internal class Gap : SettingContainer
    {
        public int spacing = 24;

        protected override float CalculateHeight(float width)
        {
            addDefaultSpacing = false;
            return spacing;
        }

        protected override void DrawSettingContents(Rect inRect) { }
    }
}