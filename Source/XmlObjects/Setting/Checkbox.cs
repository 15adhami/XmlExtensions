using Verse;

namespace XmlExtensions.Setting
{
    public class Checkbox : KeyedSettingContainer
    {
        public string tooltip;
        public string tKey;
        public string tKeyTip;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            bool currBool = bool.Parse(XmlMod.allSettings.dataDict[selectedMod + ";" + this.key]);
            listingStandard.CheckboxLabeled(Helpers.tryTranslate(label, tKey), ref currBool, Helpers.tryTranslate(tooltip, tKeyTip));
            XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = currBool.ToString();
        }

        public override int getHeight(float width, string selectedMod) { return (22 + XmlMod.menus[XmlMod.activeMenu].defaultSpacing); }
    }
}
