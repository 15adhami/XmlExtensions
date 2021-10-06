using Verse;

namespace XmlExtensions.Setting
{
    public class Gap : SettingContainer
    {
        public int spacing = 24;
        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod) { listingStandard.Gap(spacing); }

        protected override int CalcHeight(float width, string selectedMod) { return spacing; }
    }
}
