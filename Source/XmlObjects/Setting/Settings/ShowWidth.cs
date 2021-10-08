using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class ShowWidth : SettingContainer
    {
        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 22 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            Widgets.Label(inRect, "width: " + ((int)inRect.width).ToString() + "px");
        }
    }
}
