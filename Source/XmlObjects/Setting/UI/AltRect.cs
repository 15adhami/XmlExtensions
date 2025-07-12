using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class AltRect : SettingContainer
    {
        protected override float CalculateHeight(float width)
        {
            return 22;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            Widgets.DrawAltRect(inRect);
        }
    }
}