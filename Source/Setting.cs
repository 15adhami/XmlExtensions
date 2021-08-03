using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace XmlExtensions.Setting
{
    public abstract class SettingContainer : PatchOperation
    {
        public int spacing = -1;

        public virtual void drawSetting(Listing_Standard listingStandard, string selectedMod) { }

        public virtual int getHeight() { return 0; }
    }

    public abstract class KeyedSettingContainer : SettingContainer
    {
        public string key = "";
        public string label = "";
        public string defaultValue = null;
    }

    public class Range : KeyedSettingContainer
    {
        public int min;
        public int max;
        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            IntRange range = IntRange.FromString(XmlMod.allSettings.dataDict[selectedMod + "." + this.key]);
            listingStandard.Label(this.label);
            listingStandard.IntRange(ref range, min, max);
            XmlMod.allSettings.dataDict[selectedMod + "." + this.key] = range.ToString();
        }

        public override int getHeight() { return (50 + 2 * (this.spacing >= 0 ? this.spacing : XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing)); }
    }

    public class Slider : KeyedSettingContainer
    {
        public float min;
        public float max;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            string currFloat = XmlMod.allSettings.dataDict[selectedMod + "." + this.key];
            listingStandard.Label(this.label + ": " + currFloat.ToString());
            XmlMod.allSettings.dataDict[selectedMod + "." + this.key] = listingStandard.Slider(float.Parse(currFloat), min, max).ToString();
        }

        public override int getHeight() { return (44 + 2 * (this.spacing >= 0 ? this.spacing : XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing)); }
    }

    public class IntEntry : KeyedSettingContainer
    {
        public int multiplier = 1;
        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            int f = int.Parse(XmlMod.allSettings.dataDict[selectedMod + "." + this.key]);
            string b = f.ToString();
            listingStandard.IntEntry(ref f, ref b, multiplier);
            XmlMod.allSettings.dataDict[selectedMod + "." + this.key] = f.ToString();
        }

        public override int getHeight() { return (24 + (this.spacing >= 0 ? this.spacing : XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing)); }
    }

    public class IntAdjuster : KeyedSettingContainer
    {
        public int countChange;
        public int min = 0;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            int f = int.Parse(XmlMod.allSettings.dataDict[selectedMod + "." + this.key]);
            listingStandard.IntAdjuster(ref f, countChange, min);
            XmlMod.allSettings.dataDict[selectedMod + "." + this.key] = f.ToString();
        }

        public override int getHeight() { return (24 + (this.spacing >= 0 ? this.spacing : XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing)); }
    }

    public class Numeric : KeyedSettingContainer
    {
        public float min;
        public float max;
        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            float f = float.Parse(XmlMod.allSettings.dataDict[selectedMod + "." + this.key]);
            string buf = f.ToString();
            listingStandard.TextFieldNumericLabeled<float>(this.label, ref f, ref buf, min, max);
            XmlMod.allSettings.dataDict[selectedMod + "." + this.key] = f.ToString();
        }

        public override int getHeight() { return (22 + (this.spacing >= 0 ? this.spacing : XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing)); }
    }

    public class Textbox : KeyedSettingContainer
    {
        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            string currStr = XmlMod.allSettings.dataDict[selectedMod + "." + this.key];
            XmlMod.allSettings.dataDict[selectedMod + "." + this.key] = listingStandard.TextEntryLabeled(this.label, currStr);
        }

        public override int getHeight() { return (22 + (this.spacing >= 0 ? this.spacing : XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing)); }
    }

    public class Text : SettingContainer
    {
        public string text;
        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            listingStandard.Label(text);
        }

        public override int getHeight() { return (22 + (this.spacing >= 0 ? this.spacing : XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing)); }
    }

    public class Checkbox : KeyedSettingContainer
    {
        public string tooltip;
        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            bool currBool = bool.Parse(XmlMod.allSettings.dataDict[selectedMod + "." + this.key]);
            listingStandard.CheckboxLabeled(this.label, ref currBool, tooltip);
            XmlMod.allSettings.dataDict[selectedMod + "." + this.key] = currBool.ToString();
        }

        public override int getHeight() { return (22 + (this.spacing >= 0 ? this.spacing : XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing)); }
    }

    public class ResetSettings : SettingContainer
    {
        protected string label = "Reset settings";
        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            bool def = listingStandard.ButtonText(label);
            if (def)
            {
                XmlMod.settingsPerMod[selectedMod].resetSettings();
            }
        }

        public override int getHeight() { return (30 + (this.spacing >= 0 ? this.spacing : XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing)); }
    }

    public class Gap : SettingContainer
    {
        public override void drawSetting(Listing_Standard listingStandard, string selectedMod) { listingStandard.Gap(this.spacing); }

        public override int getHeight() { return (this.spacing >= 0 ? this.spacing : XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }
}
