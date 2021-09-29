using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class SetColor : SettingContainer
    {
        protected Color color = Color.white;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            GUI.color = color;
        }

        public override int getHeight(float width, string selectedMod) { return 0; }
    }
}
