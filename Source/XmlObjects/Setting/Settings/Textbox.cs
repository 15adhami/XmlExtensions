using Verse;

namespace XmlExtensions.Setting
{
    public class Textbox : KeyedSettingContainer
    {
        public string tKey;
        public int lines = 1;

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            string currStr = XmlMod.allSettings.dataDict[selectedMod + ";" + key];
            if (label != null)
            {
                XmlMod.allSettings.dataDict[selectedMod + ";" + key] = listingStandard.TextEntryLabeled(Helpers.TryTranslate(label, tKey), currStr, lines);
            }
            else
            {
                XmlMod.allSettings.dataDict[selectedMod + ";" + key] = listingStandard.TextEntry(currStr, lines);
            }
            
        }

        protected override int CalcHeight(float width, string selectedMod) { return (lines*22 + XmlMod.menus[XmlMod.activeMenu].defaultSpacing); }
    }
}
