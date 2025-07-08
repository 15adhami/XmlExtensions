using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class AltRect : SettingContainer
    {
        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 22;
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            Widgets.DrawAltRect(inRect);
        }
    }
}