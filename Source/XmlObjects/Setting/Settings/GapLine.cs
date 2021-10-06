using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class GapLine : SettingContainer
    {
        public int spacing = 24;
        protected int thickness = 1;

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            Rect gapRect = listingStandard.GetRect(spacing);
            float y = gapRect.y + spacing / 2f - thickness / 2f;
            Color color = GUI.color;
            GUI.color = color * new Color(1f, 1f, 1f, 0.4f);
            GUI.DrawTexture(new Rect(gapRect.x, y, listingStandard.ColumnWidth, thickness), BaseContent.WhiteTex);
            GUI.color = color;
        }

        protected override int CalcHeight(float width, string selectedMod)
        {
            return spacing;
        }
    }
}