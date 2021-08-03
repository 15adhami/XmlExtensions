using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class UseSetting : PatchOperation
    {
        protected string modId;
        protected string key;
        protected string brackets = "{}";
        protected string defaultValue;
        protected XmlContainer apply;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlMod.loadedMod = this.modId;
            XmlMod.addXmlMod(this.modId, this.key);
            string value;
            bool didContain = XmlMod.allSettings.dataDict.TryGetValue(this.modId + "." + this.key, out value);
            XmlContainer newContainer;
            if (!didContain)
            {
                value = defaultValue;
                XmlMod.addSetting(this.modId, this.key, defaultValue);
            }
            if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(key))
            {
                XmlMod.settingsPerMod[modId].defValues.Add(key, defaultValue);
            }
            if (!XmlMod.settingsPerMod[modId].keys.Contains(key))
            {
                XmlMod.settingsPerMod[modId].keys.Add(key);
            }
            newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.key, value, this.brackets);
            Helpers.runPatchesInXmlContainer(newContainer, xml);
            return true;
        }
    }

    public class CreateSettings : PatchOperation
    {
        protected string modId;
        protected string label;
        protected int defaultSpacing = 2;
        protected List<SettingContainer> settings;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlMod.loadedMod = this.modId;
            XmlMod.addXmlMod(this.modId, this.label);
            if (XmlMod.settingsPerMod[modId].defaultSpacing == 2)
            {
                XmlMod.settingsPerMod[modId].defaultSpacing = defaultSpacing;
            }            
            foreach (SettingContainer setting in this.settings)
            {
                XmlMod.tryAddSettings(setting, this.modId);
                if (setting.GetType().IsSubclassOf(typeof(KeyedSettingContainer)))
                {
                    if (!XmlMod.settingsPerMod[modId].keys.Contains(((KeyedSettingContainer)(setting)).key))
                    {
                        XmlMod.settingsPerMod[modId].keys.Add(((KeyedSettingContainer)(setting)).key);
                    }
                    if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(((KeyedSettingContainer)(setting)).key))
                    {
                        XmlMod.settingsPerMod[modId].defValues.Add(((KeyedSettingContainer)(setting)).key, ((KeyedSettingContainer)(setting)).defaultValue);
                    }
                } 
            }
            
            return true;
        }
    }

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

        public override int getHeight() { return (50 + 2*(this.spacing >= 0 ? this.spacing : XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing)); }
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

        public override int getHeight() { return (44 + 2*(this.spacing >= 0 ? this.spacing : XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing)); }
    }

    public class IntEntry : KeyedSettingContainer
    {
        public int multiplier;
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

    public class Gap : SettingContainer
    {
        public override void drawSetting(Listing_Standard listingStandard, string selectedMod) { listingStandard.Gap(this.spacing); }

        public override int getHeight() { return (this.spacing >= 0 ? this.spacing : XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }


}
