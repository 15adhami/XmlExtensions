using Verse;

namespace XmlExtensions.Setting
{
    public class DisplayMenu : SettingContainer
    {
        protected string label;
        public string menu;
        public string tKey;

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            if (listingStandard.ButtonText(Helpers.TryTranslate(label, tKey), null))
            {
                XmlMod.activeMenu = menu;
            }
        }

        protected override bool Init()
        {
            base.Init();
            if (label == null)
            {
                label = "Open";
                tKey = "XmlExtensions_Open";
            }
            return true;
        }

        protected override int CalcHeight(float width, string selectedMod) { return (30 + XmlMod.menus[XmlMod.activeMenu].defaultSpacing); }
    }
}
