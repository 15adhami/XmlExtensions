using Verse;

namespace XmlExtensions.Setting
{
    public class DisplayMenu : SettingContainer
    {
        protected string label = "Open";
        public string menu;
        public string tKey;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            if (listingStandard.ButtonText(Helpers.tryTranslate(label, tKey), null))
            {
                XmlMod.activeMenu = menu;
            }
        }

        public override int getHeight(float width, string selectedMod) { return (30 + XmlMod.menus[XmlMod.activeMenu].defaultSpacing); }
    }
}
