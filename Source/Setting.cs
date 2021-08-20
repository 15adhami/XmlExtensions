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

        public virtual int getHeight(float width) { return 0; }
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
            IntRange range = IntRange.FromString(XmlMod.allSettings.dataDict[selectedMod + ";" + this.key]);
            Color currColor = GUI.color;
            listingStandard.IntRange(ref range, min, max);
            GUI.color = currColor;
            XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = range.ToString();
        }

        public override int getHeight(float width) { return (28 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }

    public class Slider : KeyedSettingContainer
    {
        public float min;
        public float max;
        public string tKey;
        public string tooltip = null;
        public string tKeyTip = null;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            listingStandard.verticalSpacing = 0;
            string currFloat = XmlMod.allSettings.dataDict[selectedMod + ";" + this.key];
            listingStandard.Label(Helpers.substituteVariable(Helpers.tryTranslate(label, tKey), key, currFloat.ToString(), "{}"), 22, Helpers.tryTranslate(tooltip, tKeyTip));
            listingStandard.verticalSpacing = XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing;
            XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = listingStandard.Slider(float.Parse(currFloat), min, max).ToString();
        }

        public override int getHeight(float width) { return (44 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }

    public class IntEntry : KeyedSettingContainer
    {
        public int multiplier = 1;
        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            int f = int.Parse(XmlMod.allSettings.dataDict[selectedMod + ";" + this.key]);
            string b = f.ToString();
            listingStandard.IntEntry(ref f, ref b, multiplier);
            XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = f.ToString();
        }

        public override int getHeight(float width) { return (24 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }

    /*
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
    */

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

        public override int getHeight(float width) { return (22 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }

    public class Textbox : KeyedSettingContainer
    {
        public string tKey;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            string currStr = XmlMod.allSettings.dataDict[selectedMod + ";" + this.key];
            XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = listingStandard.TextEntryLabeled(Helpers.tryTranslate(label, tKey), currStr);
        }

        public override int getHeight(float width) { return (22 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }

    public class Text : SettingContainer
    {
        public string text;
        public GameFont font = GameFont.Small;
        public string anchor = "Left";
        public string tooltip = null;
        public string tKey = null;
        public string tKeyTip = null;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {//M: 29 S: 22 T:18
            Verse.Text.Font = font;
            TextAnchor t = TextAnchor.UpperLeft;
            if(anchor == "Middle")
            {
                t = TextAnchor.MiddleCenter;
            }
            else if (anchor == "Right")
            {
                t = TextAnchor.UpperRight;
            }
            Verse.Text.Anchor = t;
            int h = 18;
            if (font == GameFont.Small)
            {
                h = 22;
            }
            else if (font == GameFont.Medium)
            {
                h = 29;
            }
            h += 1;
            string str = Helpers.tryTranslate(text, tKey);
            listingStandard.Label(str, -1, Helpers.tryTranslate(tooltip, tKeyTip));                       
            Verse.Text.Font = GameFont.Small;
            Verse.Text.Anchor = TextAnchor.UpperLeft;
        }

        public override int getHeight(float width)
        {
            Verse.Text.Font = font;
            TextAnchor t = TextAnchor.UpperLeft;
            if (anchor == "Middle")
            {
                t = TextAnchor.MiddleCenter;
            }
            else if (anchor == "Right")
            {
                t = TextAnchor.UpperRight;
            }
            Verse.Text.Anchor = t;
            int h = 0;
            string str = Helpers.tryTranslate(text, tKey);
            h = (int)Verse.Text.CalcHeight(str, width);
            Verse.Text.Font = GameFont.Small;
            Verse.Text.Anchor = TextAnchor.UpperLeft;
            return (h + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing);
        }
    }

    public class Checkbox : KeyedSettingContainer
    {
        public string tooltip;
        public string tKey;
        public string tKeyTip;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            bool currBool = bool.Parse(XmlMod.allSettings.dataDict[selectedMod + ";" + this.key]);
            listingStandard.CheckboxLabeled(Helpers.tryTranslate(label, tKey), ref currBool, Helpers.tryTranslate(tooltip, tKeyTip));
            XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = currBool.ToString();
        }

        public override int getHeight(float width) { return (22 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }

    public class ResetSettings : SettingContainer
    {
        protected string label = "Reset settings";
        protected List<string> keys = null;
        protected bool confirm = true;
        public string tKey;
        public string tKeyTip;
        public string tooltip;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            if (!confirm)
            {
                if (keys == null) { keys = XmlMod.settingsPerMod[selectedMod].keys; }
                if (listingStandard.ButtonText(Helpers.tryTranslate(label, tKey), Helpers.tryTranslate(tooltip, tKeyTip)))
                    foreach (string key in keys)
                        XmlMod.allSettings.dataDict[selectedMod + ";" + key] = XmlMod.settingsPerMod[selectedMod].defValues[key];
            }
            else
            {
                if (keys == null) { keys = XmlMod.settingsPerMod[selectedMod].keys; }
                if (listingStandard.ButtonText(Helpers.tryTranslate(label, tKey), null))
                {
                    Find.WindowStack.Add(new Dialog_MessageBox("XmlExtensions_Confirmation".Translate(), "Yes".Translate(), delegate ()
                    {
                        foreach(string key in keys)
                            XmlMod.allSettings.dataDict[selectedMod + ";" + key] = XmlMod.settingsPerMod[selectedMod].defValues[key];
                    }, "No".Translate(), null, null, false, null, null));
                }
            }
        }

        public override int getHeight(float width) { return (30 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }

    public class Gap : SettingContainer
    {
        public int spacing = 24;
        public override void drawSetting(Listing_Standard listingStandard, string selectedMod) { listingStandard.Gap(this.spacing); }

        // TODO: Add the height of the line itself?
        public override int getHeight(float width) { return spacing; }
    }

    public class SplitColumn : SettingContainer
    {
        public float split = 0.50f;
        public List<SettingContainer> leftCol;
        public List<SettingContainer> rightCol;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            Rect baseRect = listingStandard.GetRect(Math.Max(columnHeight(leftCol, listingStandard.ColumnWidth*split), columnHeight(rightCol, listingStandard.ColumnWidth*(1-split))));
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

        private int columnHeight(List<SettingContainer> settings, float width)
        {
            int h = 0;
            foreach (SettingContainer setting in settings)
            {
                h += setting.getHeight(width);
            }
            return h;
        }

        public override int getHeight(float width) { return Math.Max(columnHeight(leftCol, width * split), columnHeight(rightCol, width * (1 - split))); }
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

        public override int getHeight(float width) { return spacing; }
    }

    public class SetColor : SettingContainer
    {
        protected Color color = Color.white;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            GUI.color = color;
        }

        public override int getHeight(float width) { return 0; }
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
                b = listingStandard.RadioButton(Helpers.tryTranslate(option.node["label"].InnerText, tKey), XmlMod.allSettings.dataDict[selectedMod+";" +key] == option.node["value"].InnerText, 0, Helpers.tryTranslate(str, tKeyTip));
                if (b) { XmlMod.allSettings.dataDict[selectedMod + ";" + key] = option.node["value"].InnerText; }
            }
            listingStandard.verticalSpacing = XmlMod.settingsPerMod[selectedMod].defaultSpacing;
        }

        public override int getHeight(float width) { return (buttons.Count * ((spacing < 0 ? XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing : spacing) + 22)); }
    }

    public class ToggleableSettings : SettingContainer
    {
        public string key;
        public List<SettingContainer> caseTrue;
        public List<SettingContainer> caseFalse;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            List<SettingContainer> settings;
            if (bool.Parse(XmlMod.allSettings.dataDict[XmlMod.selectedMod + ";" + key]))
            {
                settings = caseTrue;
            }
            else
            {
                settings = caseFalse;
            }
            if (settings != null)
            {
                Rect baseRect = listingStandard.GetRect(calcHeight(settings, listingStandard.ColumnWidth));
                Listing_Standard listing = new Listing_Standard();
                listing.Begin(baseRect);
                listing.verticalSpacing = listingStandard.verticalSpacing;
                foreach (SettingContainer setting in settings)
                {
                    setting.drawSetting(listing, selectedMod);
                }
                listing.End();
            }
        }

        private int calcHeight(List<SettingContainer> settings, float width)
        {
            int h = 0;
            if (settings != null)
            {
                foreach (SettingContainer setting in settings)
                {
                    h += setting.getHeight(width);
                }
            }
            return h;
        }

        public override int getHeight(float width) { return (bool.Parse(XmlMod.allSettings.dataDict[XmlMod.selectedMod + ";" + key]) ? calcHeight(caseTrue, width) : calcHeight(caseFalse, width)); }
    }

    public class ScrollView : SettingContainer
    {
        public float height = 72;
        public List<SettingContainer> settings;

        private Vector2 scrollPos = Vector2.zero;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            Rect baseRect = listingStandard.GetRect(height);            
            Rect scrollRect = new Rect(0, 0, baseRect.width - 20f, calcHeight(settings, baseRect.width));
            Widgets.BeginScrollView(baseRect, ref scrollPos, scrollRect);
            Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect2);
            listing.verticalSpacing = listingStandard.verticalSpacing;
            foreach (SettingContainer setting in settings)
            {
                setting.drawSetting(listing, selectedMod);
            }
            listing.End();
            Widgets.EndScrollView();
        }

        private int calcHeight(List<SettingContainer> settings, float width)
        {
            int h = 0;
            if (settings != null)
            {
                foreach (SettingContainer setting in settings)
                {
                    h += setting.getHeight(width);
                }
            }
            return h;
        }

        public override int getHeight(float width) { return ((int)height); }
    }
}
