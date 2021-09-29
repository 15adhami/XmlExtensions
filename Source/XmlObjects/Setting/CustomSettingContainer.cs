using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public abstract class CustomSettingContainer : SettingContainer
    {
        public sealed override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            DoSettingContents(listingStandard.GetRect(CalculateHeight(listingStandard.ColumnWidth, selectedMod)), selectedMod);
        }

        public virtual void DoSettingContents(Rect rect, string selectedMod) { }

        public virtual float CalculateHeight(float width, string selectedMod) { return 0; }

        public sealed override int getHeight(float width, string selectedMod)
        {
            return (int)CalculateHeight(width, selectedMod);
        }
    }
}
