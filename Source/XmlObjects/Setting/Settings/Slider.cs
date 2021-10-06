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

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {            
            listingStandard.verticalSpacing = 0;
            string currFloat = XmlMod.allSettings.dataDict[selectedMod + ";" + key];
            if (!hideLabel)
            {
                listingStandard.Label(Helpers.SubstituteVariable(Helpers.TryTranslate(label, tKey), key, currFloat.ToString(), "{}"), 22, Helpers.TryTranslate(tooltip, tKeyTip));
            }                
            listingStandard.Gap((float)Math.Ceiling((double)buffer/2));
            listingStandard.verticalSpacing = XmlMod.menus[XmlMod.activeMenu].defaultSpacing;
            float tempFloat = listingStandard.Slider(float.Parse(currFloat), min, max);
            XmlMod.allSettings.dataDict[selectedMod + ";" + key] = (Math.Round(tempFloat, decimals)).ToString();
            listingStandard.Gap((float)Math.Floor((double)buffer / 2));
        }

        protected override bool Init()
        {
            base.Init();
            if (label == null)
            {
                hideLabel = true;
            }
            return true;
        }

        protected override int CalcHeight(float width, string selectedMod) { return (22 + buffer + XmlMod.menus[XmlMod.activeMenu].defaultSpacing + (hideLabel? 0:22)); }
    }
}
