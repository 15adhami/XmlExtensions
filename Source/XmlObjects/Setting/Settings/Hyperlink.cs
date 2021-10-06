using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class Hyperlink : SettingContainer
    {

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            Rect rect = listingStandard.GetRect(24);
            Dialog_InfoCard.Hyperlink hyperlink = new Dialog_InfoCard.Hyperlink(DefDatabase<ThingDef>.GetNamed("Plant_Rice"));
            Widgets.HyperlinkWithIcon(rect, hyperlink);
        }

        protected override int CalcHeight(float width, string selectedMod) { return 24; }
    }
}
