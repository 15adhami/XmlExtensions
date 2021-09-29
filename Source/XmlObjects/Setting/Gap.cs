using Verse;

namespace XmlExtensions.Setting
{
    public class Gap : SettingContainer
    {
        public int spacing = 24;
        public override void drawSetting(Listing_Standard listingStandard, string selectedMod) { listingStandard.Gap(this.spacing); }

        public override int getHeight(float width, string selectedMod) { return spacing; }
    }
}
