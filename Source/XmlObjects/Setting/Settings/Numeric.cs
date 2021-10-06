using Verse;

namespace XmlExtensions.Setting
{
    public class Numeric : KeyedSettingContainer
    {
        public float min;
        public float max;
        public string tKey;

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            float f = float.Parse(XmlMod.allSettings.dataDict[selectedMod + ";" + key]);
            string buf = f.ToString();
            listingStandard.TextFieldNumericLabeled<float>(Helpers.TryTranslate(label, tKey), ref f, ref buf, min, max);
            XmlMod.allSettings.dataDict[selectedMod + ";" + key] = f.ToString();
        }

        protected override int CalcHeight(float width, string selectedMod) { return (22 + XmlMod.menus[XmlMod.activeMenu].defaultSpacing); }
    }
}
