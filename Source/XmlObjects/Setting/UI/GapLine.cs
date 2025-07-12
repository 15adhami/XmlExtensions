using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class GapLine : SettingContainer
    {
        public int spacing = 24;
        protected int thickness = 1;

        protected override float CalculateHeight(float width)
        {
            addDefaultSpacing = false;
            return spacing;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            float y = inRect.y + spacing / 2f - thickness / 2f;
            Color color = GUI.color;
            GUI.color = color * new Color(1f, 1f, 1f, 0.4f);
            GUI.DrawTexture(new Rect(inRect.x, y, inRect.width, thickness), BaseContent.WhiteTex);
            GUI.color = color;
        }
    }
}