using System.Collections.Generic;
using Verse;

namespace XmlExtensions.Setting
{
    public class RadioButtons : KeyedSettingContainer
    {
        public List<XmlContainer> buttons;
        protected int spacing = -1;

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            listingStandard.verticalSpacing = (spacing < 0 ? XmlMod.menus[XmlMod.activeMenu].defaultSpacing : spacing);
            foreach (XmlContainer option in buttons)
            {                
                bool b = false;
                string str;
                try
                {
                    str = option.node["tooltip"].InnerText;
                }
                catch
                {
                    str = null;
                }
                string tKey;
                try
                {
                    tKey = option.node["tKey"].InnerText;
                }
                catch
                {
                    tKey = null;
                }
                string tKeyTip;
                try
                {
                    tKeyTip = option.node["tKeyTip"].InnerText;
                }
                catch
                {
                    tKeyTip = null;
                }
                b = listingStandard.RadioButton(Helpers.TryTranslate(option.node["label"].InnerText, tKey), XmlMod.allSettings.dataDict[selectedMod+";" +key] == option.node["value"].InnerText, 0, Helpers.TryTranslate(str, tKeyTip));
                if (b) { XmlMod.allSettings.dataDict[selectedMod + ";" + key] = option.node["value"].InnerText; }
            }
            listingStandard.verticalSpacing = XmlMod.menus[XmlMod.activeMenu].defaultSpacing;
        }

        protected override int CalcHeight(float width, string selectedMod) { return (buttons.Count * ((spacing < 0 ? XmlMod.menus[XmlMod.activeMenu].defaultSpacing : spacing) + 22)); }
    }
}
