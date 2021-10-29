using UnityEngine;

namespace XmlExtensions.Setting
{
    internal class SetColor : SettingContainer
    {
        protected Color color = Color.white;

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            GUI.color = color;
        }
    }
}