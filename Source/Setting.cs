using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public abstract class SettingContainer : PatchOperation
    {
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
            listingStandard.IntRange(ref range, min, max);
            XmlMod.allSettings.dataDict[selectedMod + "." + this.key] = range.ToString();
        }

        public override int getHeight() { return (28 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }

    public class Slider : KeyedSettingContainer
    {
        public float min;
        public float max;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            listingStandard.verticalSpacing = 0;
            string currFloat = XmlMod.allSettings.dataDict[selectedMod + "." + this.key];
            listingStandard.Label(Helpers.substituteVariable(label, key, currFloat.ToString(), "{}"));
            listingStandard.verticalSpacing = XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing;
            XmlMod.allSettings.dataDict[selectedMod + "." + this.key] = listingStandard.Slider(float.Parse(currFloat), min, max).ToString();
        }

        public override int getHeight() { return (44 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
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

        public override int getHeight() { return (24 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
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

        public override int getHeight() { return (24 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
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

        public override int getHeight() { return (22 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }

    public class Textbox : KeyedSettingContainer
    {
        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            string currStr = XmlMod.allSettings.dataDict[selectedMod + "." + this.key];
            XmlMod.allSettings.dataDict[selectedMod + "." + this.key] = listingStandard.TextEntryLabeled(this.label, currStr);
        }

        public override int getHeight() { return (22 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }

    public class Text : SettingContainer
    {
        public string text;
        public GameFont font = GameFont.Small;
        public string tooltip = null;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {//M: 29 S: 22 T:18
            Verse.Text.Font = font;
            int h = 18;
            if (font == GameFont.Small)
            {
                h = 22;
            }
            else if (font == GameFont.Medium)
            {
                h = 29;
            }
            listingStandard.Label(text, h, tooltip);
            Verse.Text.Font = GameFont.Small;
        }

        public override int getHeight()
        {
            int h = 18;
            if (font == GameFont.Small)
            {
                h = 22;
            }
            else if (font == GameFont.Medium)
            {
                h = 29;
            }
            return (h + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing);
        }
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

        public override int getHeight() { return (22 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
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

        public override int getHeight() { return (30 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }

    public class Gap : SettingContainer
    {
        public int spacing = 24;
        public override void drawSetting(Listing_Standard listingStandard, string selectedMod) { listingStandard.Gap(this.spacing); }

        // TODO: Add the height of the line itself?
        public override int getHeight() { return spacing; }
    }

    public class SplitColumn : SettingContainer
    {
        public float split = 0.50f;
        public List<SettingContainer> leftCol;
        public List<SettingContainer> rightCol;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            Rect baseRect = listingStandard.GetRect(Math.Max(columnHeight(leftCol), columnHeight(rightCol)));
            //Rect baseRect = listingStandard.GetRect(100);
            Rect leftRect = baseRect.LeftPart(split - 0.005f);
            Rect rightRect = baseRect.RightPart(1 - split - 0.005f);
            Listing_Standard lListing = new Listing_Standard();
            lListing.Begin(leftRect);
            lListing.verticalSpacing = listingStandard.verticalSpacing;
            foreach (SettingContainer setting in leftCol)
            {
                setting.drawSetting(lListing, selectedMod);
            }
            lListing.End();
            Listing_Standard rListing = new Listing_Standard();
            rListing.Begin(rightRect);
            rListing.verticalSpacing = listingStandard.verticalSpacing;
            foreach (SettingContainer setting in rightCol)
            {
                setting.drawSetting(rListing, selectedMod);
            }
            rListing.End();
        }

        private int columnHeight(List<SettingContainer> settings)
        {
            int h = 0;
            foreach (SettingContainer setting in settings)
            {
                h += setting.getHeight();
            }
            return h;
        }

        // TODO: Add the height of the line itself?
        public override int getHeight() { return Math.Max(columnHeight(leftCol), columnHeight(rightCol)); }
    }

    public class GapLine : SettingContainer
    {
        public int spacing = 24;
        protected int thickness = 1;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod) 
        {
            Rect gapRect = listingStandard.GetRect(spacing);
            float y = gapRect.y + spacing / 2f - thickness / 2;
            Color color = GUI.color;
            GUI.color = color * new Color(1f, 1f, 1f, 0.4f);
            GUI.DrawTexture(new Rect(gapRect.x, y, listingStandard.ColumnWidth, thickness), BaseContent.WhiteTex);
            GUI.color = color;
        }

        public override int getHeight() { return spacing; }
    }

    public class SetColor : SettingContainer
    {
        protected Color color = Color.white;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            GUI.color = color;
        }

        public override int getHeight() { return 0; }
    }

    public class RadioButtons : KeyedSettingContainer
    {
        public List<XmlContainer> buttons;
        protected int spacing = -1;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            listingStandard.verticalSpacing = (spacing < 0 ? XmlMod.settingsPerMod[selectedMod].defaultSpacing : spacing);
            foreach (XmlContainer option in buttons)
            {
                bool b = listingStandard.RadioButton_NewTemp(option.node["label"].InnerText, XmlMod.allSettings.dataDict[selectedMod+"."+key] == option.node["value"].InnerText);
                if (b) { XmlMod.allSettings.dataDict[selectedMod + "." + key] = option.node["value"].InnerText; }
            }
            listingStandard.verticalSpacing = XmlMod.settingsPerMod[selectedMod].defaultSpacing;
        }

        public override int getHeight() { return (buttons.Count * ((spacing < 0 ? XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing : spacing) + 22)); }
    }
}
