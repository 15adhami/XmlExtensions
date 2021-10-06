using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class SetColor : SettingContainer
    {
        protected Color color = Color.white;

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            GUI.color = color;
        }

        protected override int CalcHeight(float width, string selectedMod) { return 0; }
    }
}
