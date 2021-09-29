using System;
using Verse;

namespace XmlExtensions.Setting
{
    public class Slider : KeyedSettingContainer
    {
        public float min;
        public float max;
        public string tKey;
        public string tooltip = null;
        public string tKeyTip = null;
        public int decimals = 6;
        public bool hideLabel = false;
        private int buffer = 0;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {            
            listingStandard.verticalSpacing = 0;
            string currFloat = XmlMod.allSettings.dataDict[selectedMod + ";" + this.key];
            if (!hideLabel)
            {
                listingStandard.Label(Helpers.substituteVariable(Helpers.tryTranslate(label, tKey), key, currFloat.ToString(), "{}"), 22, Helpers.tryTranslate(tooltip, tKeyTip));
            }                
            listingStandard.Gap((float)Math.Ceiling((double)buffer/2));
            listingStandard.verticalSpacing = XmlMod.menus[XmlMod.activeMenu].defaultSpacing;
            float tempFloat = listingStandard.Slider(float.Parse(currFloat), min, max);
            XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = (Math.Round(tempFloat, decimals)).ToString();
            listingStandard.Gap((float)Math.Floor((double)buffer / 2));
        }

        public override void Init()
        {
            base.Init();
            if (label == null)
            {
                hideLabel = true;
            }                
        }

        public override int getHeight(float width, string selectedMod) { return (22 + buffer + XmlMod.menus[XmlMod.activeMenu].defaultSpacing + (hideLabel? 0:22)); }
    }
}
