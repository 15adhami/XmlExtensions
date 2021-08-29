﻿using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public abstract class SettingContainer : PatchOperation
    {
        public int errHeight = -1;

        public virtual void DrawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            try
            {
                this.drawSetting(listingStandard, selectedMod);
            }
            catch
            {
                GUI.color = Color.red;
                listingStandard.Label("Error drawing setting");
                errHeight = 22;
                GUI.color = Color.white;
            }
            
        }

        public virtual void drawSetting(Listing_Standard listingStandard, string selectedMod) { }

        public virtual bool setDefaultValue(string modId) { return true; }

        public int GetHeight(float width, string selectedMod) { return (errHeight < 0 ? this.getHeight(width, selectedMod) : errHeight); }

        public virtual int getHeight(float width, string selectedMod) { return 0; }

        public virtual void init() { }
    }

    public abstract class KeyedSettingContainer : SettingContainer
    {
        public string key = null;
        public string label = null;
        public string defaultValue = null;

        public override bool setDefaultValue(string modId)
        {
            if (key == null)
            {
                PatchManager.errors.Add("Error in " + this.GetType().ToString() + ": <key> is null");
                return false;
            }
            if (!XmlMod.settingsPerMod[modId].keys.Contains(key))
            {
                XmlMod.settingsPerMod[modId].keys.Add(key);
            }
            if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(key))
            {
                if (defaultValue != null)
                {
                    XmlMod.settingsPerMod[modId].defValues.Add(key, defaultValue);
                    if (!XmlMod.allSettings.dataDict.ContainsKey(modId + ";" + key))
                        XmlMod.allSettings.dataDict.Add(modId + ";" + key, defaultValue);
                }
                else
                {// TODO: Make a check after game boots up
                    //Log.Error("[XML Extensions] " + modId + "." + ((KeyedSettingContainer)(setting)).key + " has no default value defined.");
                }
            }
            return true;
        }

        public override void DrawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            try
            {
                this.drawSetting(listingStandard, selectedMod);
            }
            catch
            {
                GUI.color = Color.red;
                listingStandard.Label("Error drawing setting (maybe <defaultValue> needs to be defined?)");
                errHeight = 22;
                GUI.color = Color.white;
            }
        }
    }

    public class Range : KeyedSettingContainer
    {
        public int min;
        public int max;
        public string key2;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            if (key2 == null)
            {
                IntRange range = IntRange.FromString(XmlMod.allSettings.dataDict[selectedMod + ";" + this.key]);
                Color currColor = GUI.color;
                listingStandard.IntRange(ref range, min, max);
                GUI.color = currColor;
                XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = range.ToString();
            }
            else
            {
                IntRange range = IntRange.FromString(XmlMod.allSettings.dataDict[selectedMod + ";" + key]+"~"+ XmlMod.allSettings.dataDict[selectedMod + ";" + key2]);
                Color currColor = GUI.color;
                listingStandard.IntRange(ref range, min, max);
                GUI.color = currColor;
                XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = range.min.ToString();
                XmlMod.allSettings.dataDict[selectedMod + ";" + this.key2] = range.max.ToString();
            }
            
        }

        public override bool setDefaultValue(string modId)
        {
            if (key == null)
            {
                PatchManager.errors.Add("Error in XmlExtensions.Setting.Range: <key> is null");
                return false;
            }
            if (key2 == null)
            {
                if (!XmlMod.settingsPerMod[modId].keys.Contains(key))
                {
                    XmlMod.settingsPerMod[modId].keys.Add(key);
                }
                if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(key))
                {
                    if (defaultValue != null)
                    {
                        XmlMod.settingsPerMod[modId].defValues.Add(key, defaultValue);
                        if (!XmlMod.allSettings.dataDict.ContainsKey(modId + ";" + key))
                            XmlMod.allSettings.dataDict.Add(modId + ";" + key, defaultValue);
                    }
                    else
                    {// TODO: Make a check after game boots up
                     //Log.Error("[XML Extensions] " + modId + "." + ((KeyedSettingContainer)(setting)).key + " has no default value defined.");
                    }
                }
            }
            else
            {
                if (!XmlMod.settingsPerMod[modId].keys.Contains(key))
                {
                    XmlMod.settingsPerMod[modId].keys.Add(key);
                }
                if (!XmlMod.settingsPerMod[modId].keys.Contains(key2))
                {
                    XmlMod.settingsPerMod[modId].keys.Add(key2);
                }
                if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(key))
                {
                    if (defaultValue != null)
                    {
                        XmlMod.settingsPerMod[modId].defValues.Add(key, defaultValue.Split('~')[0]);
                        if (!XmlMod.allSettings.dataDict.ContainsKey(modId + ";" + key))
                            XmlMod.allSettings.dataDict.Add(modId + ";" + key, defaultValue.Split('~')[0]);
                    }
                    else
                    {// TODO: Make a check after game boots up
                     //Log.Error("[XML Extensions] " + modId + "." + ((KeyedSettingContainer)(setting)).key + " has no default value defined.");
                    }
                }
                if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(key2))
                {
                    if (defaultValue != null)
                    {
                        XmlMod.settingsPerMod[modId].defValues.Add(key2, defaultValue.Split('~')[1]);
                        if (!XmlMod.allSettings.dataDict.ContainsKey(modId + ";" + key2))
                            XmlMod.allSettings.dataDict.Add(modId + ";" + key2, defaultValue.Split('~')[1]);
                    }
                    else
                    {// TODO: Make a check after game boots up
                     //Log.Error("[XML Extensions] " + modId + "." + ((KeyedSettingContainer)(setting)).key + " has no default value defined.");
                    }
                }
            }
            return true;
        }

        public override int getHeight(float width, string selectedMod) { return (28 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }

    public class Slider : KeyedSettingContainer
    {
        public float min;
        public float max;
        public string tKey;
        public string tooltip = null;
        public string tKeyTip = null;
        public int decimals = 6;
        public bool hideLabel = false;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            listingStandard.verticalSpacing = 0;
            string currFloat = XmlMod.allSettings.dataDict[selectedMod + ";" + this.key];
            if (!hideLabel)
                listingStandard.Label(Helpers.substituteVariable(Helpers.tryTranslate(label, tKey), key, currFloat.ToString(), "{}"), 22, Helpers.tryTranslate(tooltip, tKeyTip));
            listingStandard.verticalSpacing = XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing;
            XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = (Math.Round(listingStandard.Slider(float.Parse(currFloat), min, max), decimals)).ToString();
        }

        public override int getHeight(float width, string selectedMod) { return (22 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing + (hideLabel? 0:22)); }
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

        public override int getHeight(float width, string selectedMod) { return (24 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
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

        public override int getHeight(float width, string selectedMod) { return (22 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }

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

        public override int getHeight(float width, string selectedMod) { return (lines*22 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }

    public class Text : SettingContainer
    {
        public string text;
        public GameFont font = GameFont.Small;
        public string anchor = "Left";
        public string tooltip = null;
        public string tKey = null;
        public string tKeyTip = null;
        public List<string> keys;

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
            if (keys != null)
            {
                foreach (string key in keys)
                {
                    str = Helpers.substituteVariable(str, key, XmlMod.allSettings.dataDict[selectedMod + ";" + key], "{}");
                }
            }            
            listingStandard.Label(str, -1, Helpers.tryTranslate(tooltip, tKeyTip));                       
            Verse.Text.Font = GameFont.Small;
            Verse.Text.Anchor = TextAnchor.UpperLeft;
        }

        public override int getHeight(float width, string selectedMod)
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

        public override int getHeight(float width, string selectedMod) { return (22 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
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
                {
                    foreach (string key in keys)
                        XmlMod.allSettings.dataDict[selectedMod + ";" + key] = XmlMod.settingsPerMod[selectedMod].defValues[key];
                }
                    
            }
            else
            {
                if (keys == null) { keys = XmlMod.settingsPerMod[selectedMod].keys; }
                if (listingStandard.ButtonText(Helpers.tryTranslate(label, tKey), null))
                {
                    Find.WindowStack.Add(new Dialog_MessageBox(Helpers.tryTranslate("Are you sure you want to reset every setting of the current mod?", "XmlExtensions_ConfirmationResetMod"), "Yes".Translate(), delegate ()
                    {
                        foreach(string key in keys)
                        {
                            if (XmlMod.allSettings.dataDict.ContainsKey(selectedMod + ";" + key))
                                XmlMod.allSettings.dataDict[selectedMod + ";" + key] = XmlMod.settingsPerMod[selectedMod].defValues[key];
                        }
                            
                    }, "No".Translate(), null, null, false, null, null));
                }
            }
        }

        public override int getHeight(float width, string selectedMod) { return (30 + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing); }
    }

    public class Gap : SettingContainer
    {
        public int spacing = 24;
        public override void drawSetting(Listing_Standard listingStandard, string selectedMod) { listingStandard.Gap(this.spacing); }

        // TODO: Add the height of the line itself?
        public override int getHeight(float width, string selectedMod) { return spacing; }
    }

    public class SplitColumn : SettingContainer
    {
        public float split = 0.50f;
        public List<SettingContainer> leftCol = new List<SettingContainer>();
        public List<SettingContainer> rightCol = new List<SettingContainer>();

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            Rect baseRect = listingStandard.GetRect(Math.Max(columnHeight(leftCol, listingStandard.ColumnWidth*split, selectedMod), columnHeight(rightCol, listingStandard.ColumnWidth*(1-split), selectedMod)));
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

        private int columnHeight(List<SettingContainer> settings, float width, string selectedMod)
        {
            int h = 0;
            foreach (SettingContainer setting in settings)
            {
                h += setting.GetHeight(width, selectedMod);
            }
            return h;
        }

        public override bool setDefaultValue(string modId)
        {
            int c = 0;
            if(leftCol != null)
            {
                c = 0;
                foreach(SettingContainer setting in leftCol)
                {
                    if(!setting.setDefaultValue(modId))
                    {
                        c++;
                        PatchManager.errors.Add("Error in XmlExtensions.Setting.SplitColumn: failed to initialize a setting in <leftCol> at position: " + c.ToString());
                        return false;
                    }
                }
            }
            if (rightCol != null)
            {
                c = 0;
                foreach (SettingContainer setting in rightCol)
                {
                    if (!setting.setDefaultValue(modId))
                    {
                        c++;
                        PatchManager.errors.Add("Error in XmlExtensions.Setting.SplitColumn: failed to initialize a setting in <rightCol> at position: " + c.ToString());
                        return false;
                    }
                }
            }
            return true;
        }

        public override int getHeight(float width, string selectedMod) { return Math.Max(columnHeight(leftCol, width * split, selectedMod), columnHeight(rightCol, width * (1 - split), selectedMod)); }
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

        public override int getHeight(float width, string selectedMod) { return spacing; }
    }

    public class SetColor : SettingContainer
    {
        protected Color color = Color.white;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            GUI.color = color;
        }

        public override int getHeight(float width, string selectedMod) { return 0; }
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

        public override int getHeight(float width, string selectedMod) { return (buttons.Count * ((spacing < 0 ? XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing : spacing) + 22)); }
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
                Rect baseRect = listingStandard.GetRect(calcHeight(settings, listingStandard.ColumnWidth, selectedMod));
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

        private int calcHeight(List<SettingContainer> settings, float width, string selectedMod)
        {
            int h = 0;
            if (settings != null)
            {
                foreach (SettingContainer setting in settings)
                {
                    h += setting.GetHeight(width, selectedMod);
                }
            }
            return h;
        }

        public override bool setDefaultValue(string modId)
        {
            if (caseTrue != null)
            {
                int c = 0;
                foreach (SettingContainer setting in caseTrue)
                {
                    c++;
                    if(!setting.setDefaultValue(modId))
                    {
                        PatchManager.errors.Add("Error in XmlExtensions.Setting.ToggleableSettings: failed to initialize a setting in <caseTrue> at position: " + c.ToString());
                        return false;
                    }
                }
            }
            if (caseFalse != null)
            {
                int c = 0;
                foreach (SettingContainer setting in caseFalse)
                {
                    c++;
                    if (!setting.setDefaultValue(modId))
                    {
                        PatchManager.errors.Add("Error in XmlExtensions.Setting.ToggleableSettings: failed to initialize a setting in <caseFalse> at position: " + c.ToString());
                        return false;
                    }
                }
            }
            return true;
        }

        public override int getHeight(float width, string selectedMod) { return (bool.Parse(XmlMod.allSettings.dataDict[XmlMod.selectedMod + ";" + key]) ? calcHeight(caseTrue, width, selectedMod) : calcHeight(caseFalse, width, selectedMod)); }
    }

    public class ScrollView : SettingContainer
    {
        public float height = 72;
        public List<SettingContainer> settings;

        private Vector2 scrollPos = Vector2.zero;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            Rect baseRect = listingStandard.GetRect(height);            
            Rect scrollRect = new Rect(0, 0, baseRect.width - 20f, calcHeight(settings, baseRect.width, selectedMod));
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

        private int calcHeight(List<SettingContainer> settings, float width, string selectedMod)
        {
            int h = 0;
            if (settings != null)
            {
                foreach (SettingContainer setting in settings)
                {
                    h += setting.GetHeight(width, selectedMod);
                }
            }
            return h;
        }

        public override bool setDefaultValue(string modId)
        {
            if (settings != null)
            {
                int c = 0;
                foreach (SettingContainer setting in settings)
                {
                    c++;
                    if (!setting.setDefaultValue(modId))
                    {
                        PatchManager.errors.Add("Error in XmlExtensions.Setting.ScrollView: failed to initialize a setting at position: " + c.ToString());
                        return false;
                    }
                }
            }
            return true;
        }

        public override int getHeight(float width, string selectedMod) { return ((int)height); }
    }

    public class DrawImage : SettingContainer
    {
        public string texPath;
        public string anchor = "Middle";
        public Vector2 dimensions = new Vector2(-1,-1);
        public float scale = -1;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            Texture2D img = ContentFinder<Texture2D>.Get(texPath);
            int height = img.height;
            int width = img.width;
            Rect drawRect = new Rect();
            if((dimensions.x < 0 || dimensions.y < 0) && scale < 0)
            {
                if (width > listingStandard.ColumnWidth)
                {
                    height = (int)(height/(width/ listingStandard.ColumnWidth));
                    width = (int)listingStandard.ColumnWidth;
                }
                Rect tempRect = listingStandard.GetRect(height);
                if (anchor == "Middle")
                {
                    drawRect = tempRect.LeftPartPixels((tempRect.width + width) / 2);
                    drawRect = drawRect.RightPartPixels(width);
                }
                else if (anchor == "Right")
                    drawRect = tempRect.RightPartPixels(width);
                else
                    drawRect = tempRect.LeftPartPixels(width);
            }
            else if(scale < 0)
            {
                if (dimensions.x > listingStandard.ColumnWidth)
                {
                    width = (int)listingStandard.ColumnWidth;
                }
                else
                {
                    width = (int)dimensions.x;
                }
                height = (int)dimensions.y;
                Rect tempRect = listingStandard.GetRect(height);
                if (anchor == "Middle")
                {
                    drawRect = tempRect.LeftPartPixels((tempRect.width + width) / 2);
                    drawRect = drawRect.RightPartPixels(width);
                }
                else if (anchor == "Right")
                    drawRect = tempRect.RightPartPixels(width);
                else
                    drawRect = tempRect.LeftPartPixels(width);
            }
            else
            {
                width = (int)(width * scale);
                height = (int)(height * scale);
                if (width > listingStandard.ColumnWidth)
                {
                    height = (int)(height / (width / listingStandard.ColumnWidth));
                    width = (int)listingStandard.ColumnWidth;
                }
                Rect tempRect = listingStandard.GetRect(height);
                if (anchor == "Middle")
                {
                    drawRect = tempRect.LeftPartPixels((tempRect.width + width) / 2);
                    drawRect = drawRect.RightPartPixels(width);
                }
                else if (anchor == "Right")
                    drawRect = tempRect.RightPartPixels(width);
                else
                    drawRect = tempRect.LeftPartPixels(width);
            }
            
            GUI.DrawTexture(drawRect, img);
        }

        public override int getHeight(float width2, string selectedMod)
        {
            Texture2D img = ContentFinder<Texture2D>.Get(texPath);
            int height = img.height;
            int width = img.width;
            if ((dimensions.x < 0 || dimensions.y < 0) && scale < 0)
            {
                if (width > width2)
                {
                    height = (int)(height / (width / width2));
                }
            }
            else if (scale < 0)
            {
                height = (int)dimensions.y;
            }
            else
            {
                width = (int)(width * scale);
                height = (int)(height * scale);
                if (width > width2)
                {
                    height = (int)(height / (width / width2));
                }
            }
            return height;
        }
    }

    public class SwitchSetting : SettingContainer
    {
        public string value;
        public List<SettingContainer> settings;
    }

    public class SwitchSettings : SettingContainer
    {
        public string key;
        public List<SwitchSetting> cases = null;
        private Dictionary<string, List<SettingContainer>> valSettingDict;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            try
            {
                List<SettingContainer> settings = valSettingDict[XmlMod.allSettings.dataDict[selectedMod + ";" + key]];
                foreach (SettingContainer setting in settings)
                {
                    setting.drawSetting(listingStandard, selectedMod);
                }
            }
            catch
            {

            }
            
        }

        public override int getHeight(float width, string selectedMod)
        {
            base.getHeight(width, selectedMod);
            int h = 0;
            try
            {
                List<SettingContainer> settings = valSettingDict[XmlMod.allSettings.dataDict[selectedMod + ";" + key]];
                foreach (SettingContainer setting in settings)
                {
                    h += setting.GetHeight(width, selectedMod);
                }
            }
            catch
            {

            }
            return h;
        }

        public override void init()
        {
            base.init();
            if (cases != null)
            {
                valSettingDict = new Dictionary<string, List<SettingContainer>>();
                foreach (SwitchSetting setting in cases)
                {
                    valSettingDict.Add(setting.value, setting.settings);
                }
            }            
        }

        public override bool setDefaultValue(string modId)
        {
            if (cases != null)
            {
                int i = 0;
                foreach (SwitchSetting switchSetting in cases)
                {
                    i++;
                    int c = 0;
                    foreach (SettingContainer setting in switchSetting.settings)
                    {
                        c++;
                        if (!setting.setDefaultValue(modId))
                        {
                            PatchManager.errors.Add("Error in XmlExtensions.Setting.SwitchSettings: failed to initialize a setting in case: " + i.ToString() + ", at position: " + c.ToString());
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }

    public class Tab : SettingContainer
    {
        public string label;
        public string tKey;
        public List<SettingContainer> settings;

        public override int getHeight(float width, string selectedMod)
        {
            int h = 0;
            foreach(SettingContainer setting in settings)
            {
                h += setting.GetHeight(width, selectedMod);
            }
            return h;
        }
    }

    public class TabView : SettingContainer
    {
        protected List<Tab> tabs;
        private List<TabRecord> tabRecords;
        private int selectedTab = 0;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            Rect rectTab = listingStandard.GetRect(tabs[selectedTab].getHeight(listingStandard.ColumnWidth, selectedMod) + 45);
            rectTab.yMin += 45f;
            TabDrawer.DrawTabs<TabRecord>(rectTab, tabRecords, 200f);
            Listing_Standard tempListing = new Listing_Standard();
            tempListing.Begin(rectTab);
            
            for (int i = 0; i < tabs.Count; i++)
            {
                if (selectedTab == i)
                {
                    foreach(SettingContainer setting in tabs[i].settings)
                    {
                        setting.drawSetting(tempListing, selectedMod);
                    }
                }
            }
            tempListing.End();
        }

        public override int getHeight(float width, string selectedMod)
        {
            return tabs[selectedTab].getHeight(width, selectedMod) + 45;
        }

        public override void init()
        {
            base.init();
            tabRecords = new List<TabRecord>();
            for (int i = 0; i < tabs.Count; i++)
            {
                int t = i;
                TabRecord temp = new TabRecord(tabs[t].label, delegate()
                {
                    this.selectedTab = t;
                }, () => this.selectedTab == t);
                tabRecords.Add(temp);
            }
        }

        public override bool setDefaultValue(string modId)
        {
            int t = 0;
            foreach(Tab tab in tabs)
            {
                t++;
                int c = 0;
                foreach(SettingContainer setting in tab.settings)
                {
                    c++;
                    if (!setting.setDefaultValue(modId))
                    {
                        PatchManager.errors.Add("Error in XmlExtensions.Setting.TabView: failed to initialize a setting in tab: " + t.ToString() + ", at position: " + c.ToString());
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
