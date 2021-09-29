using Verse;

namespace XmlExtensions.Setting
{
    public class Textbox : KeyedSettingContainer
    {
        public string tKey;
        public int lines = 1;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            string currStr = XmlMod.allSettings.dataDict[selectedMod + ";" + this.key];
            if (label != null)
            {
                XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = listingStandard.TextEntryLabeled(Helpers.tryTranslate(label, tKey), currStr, lines);
            }
            else
            {
                XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = listingStandard.TextEntry(currStr, lines);
            }
            
        }

        public override int getHeight(float width, string selectedMod) { return (lines*22 + XmlMod.menus[XmlMod.activeMenu].defaultSpacing); }
    }
}
