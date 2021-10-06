using Verse;

namespace XmlExtensions.Setting
{
    public class Checkbox : KeyedSettingContainer
    {
        public string tooltip;
        public string tKey;
        public string tKeyTip;

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            bool currBool = bool.Parse(XmlMod.allSettings.dataDict[selectedMod + ";" + key]);
            listingStandard.CheckboxLabeled(Helpers.TryTranslate(label, tKey), ref currBool, Helpers.TryTranslate(tooltip, tKeyTip));
            XmlMod.allSettings.dataDict[selectedMod + ";" + key] = currBool.ToString();
        }

        protected override int CalcHeight(float width, string selectedMod) { return (22 + XmlMod.menus[XmlMod.activeMenu].defaultSpacing); }
    }
}
