using Verse;

namespace XmlExtensions.Setting
{
    public class Numeric : KeyedSettingContainer
    {
        public float min;
        public float max;
        public string tKey;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            float f = float.Parse(XmlMod.allSettings.dataDict[selectedMod + ";" + this.key]);
            string buf = f.ToString();
            listingStandard.TextFieldNumericLabeled<float>(Helpers.tryTranslate(label, tKey), ref f, ref buf, min, max);
            XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = f.ToString();
        }

        public override int getHeight(float width, string selectedMod) { return (22 + XmlMod.menus[XmlMod.activeMenu].defaultSpacing); }
    }
}
