using RimWorld;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace XmlExtensions.Setting
{
    public abstract class SettingContainer : PatchOperation
    {
        public int errHeight = -1;
        public string tag;

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
                }
                if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(key2))
                {
                    if (defaultValue != null)
                    {
                        XmlMod.settingsPerMod[modId].defValues.Add(key2, defaultValue.Split('~')[1]);
                        if (!XmlMod.allSettings.dataDict.ContainsKey(modId + ";" + key2))
                            XmlMod.allSettings.dataDict.Add(modId + ";" + key2, defaultValue.Split('~')[1]);
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
            listingStandard.verticalSpacing = XmlMod.settingsPerMod[selectedMod].defaultSpacing;
            float tempFloat = listingStandard.Slider(float.Parse(currFloat), min, max);
            XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = (Math.Round(tempFloat, decimals)).ToString();
            listingStandard.Gap((float)Math.Floor((double)buffer / 2));
        }

        public override void init()
        {
            base.init();
            if (label == null)
            {
                hideLabel = true;
            }                
        }

        public override int getHeight(float width, string selectedMod) { return (22 + buffer + XmlMod.settingsPerMod[XmlMod.selectedMod].defaultSpacing + (hideLabel? 0:22)); }
    }

    public class IntEntry : KeyedSettingContainer
    {
        public int multiplier = 1;
        public string min;
        public string max;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            int f = int.Parse(XmlMod.allSettings.dataDict[selectedMod + ";" + this.key]);
            
            if (min != null && f < int.Parse(min))
                f = int.Parse(min);
            if (max != null && f > int.Parse(max))
                f = int.Parse(max);
            string editBuffer = f.ToString();
            int value = f;
            Rect rect = listingStandard.GetRect(24f);
            if (listingStandard.BoundingRectCached == null || rect.Overlaps(listingStandard.BoundingRectCached.Value))
            {
                int num = Mathf.Min(40, (int)rect.width / 5);
                if (Widgets.ButtonText(new Rect(rect.xMin, rect.yMin, (float)num, rect.height), (-10 * multiplier).ToStringCached(), true, true, true))
                {
                    value -= 10 * multiplier * GenUI.CurrentAdjustmentMultiplier();
                    editBuffer = value.ToStringCached();
                    SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
                }
                if (Widgets.ButtonText(new Rect(rect.xMin + (float)num, rect.yMin, (float)num, rect.height), (-1 * multiplier).ToStringCached(), true, true, true))
                {
                    value -= multiplier * GenUI.CurrentAdjustmentMultiplier();
                    editBuffer = value.ToStringCached();
                    SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
                }
                if (Widgets.ButtonText(new Rect(rect.xMax - (float)num, rect.yMin, (float)num, rect.height), "+" + (10 * multiplier).ToStringCached(), true, true, true))
                {
                    value += 10 * multiplier * GenUI.CurrentAdjustmentMultiplier();
                    editBuffer = value.ToStringCached();
                    SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
                }
                if (Widgets.ButtonText(new Rect(rect.xMax - (float)(num * 2), rect.yMin, (float)num, rect.height), "+" + multiplier.ToStringCached(), true, true, true))
                {
                    value += multiplier * GenUI.CurrentAdjustmentMultiplier();
                    editBuffer = value.ToStringCached();
                    SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
                }
                Widgets.TextFieldNumeric<int>(new Rect(rect.xMin + (float)(num * 2), rect.yMin, rect.width - (float)(num * 4), rect.height), ref value, ref editBuffer, -9999999f, 1E+09f);
            }
            listingStandard.Gap(listingStandard.verticalSpacing);

            XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = value.ToString();
            
            
            
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
                t = TextAnchor.UpperCenter;
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
                t = TextAnchor.UpperCenter;
            }
            else if (anchor == "Right")
            {
                t = TextAnchor.UpperRight;
            }
            Verse.Text.Anchor = t;
            int h = 0;
            string str = Helpers.tryTranslate(text, tKey);
            h = (int)Math.Ceiling(Verse.Text.CalcHeight(str, width));
            Verse.Text.Font = GameFont.Small;
            Verse.Text.Anchor = TextAnchor.UpperLeft;
            return h + XmlMod.settingsPerMod[selectedMod].defaultSpacing;
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

        public override int getHeight(float width, string selectedMod) { return (22 + XmlMod.settingsPerMod[selectedMod].defaultSpacing); }
    }

    public class ResetSettings : SettingContainer
    {
        protected string label = "Reset settings";
        protected List<string> keys = null;
        protected bool confirm = true;
        public string message;
        public string tKeyMessage;
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
                if (tKeyMessage == null && message == null)
                    tKeyMessage = "XmlExtensions_ConfirmationResetMod";
                if(message == null)
                    message = "Are you sure you want to reset every setting of the current mod?";
                if (keys == null) { keys = XmlMod.settingsPerMod[selectedMod].keys; }
                if (listingStandard.ButtonText(Helpers.tryTranslate(label, tKey), null))
                {
                    Find.WindowStack.Add(new Dialog_MessageBox(Helpers.tryTranslate(message, tKeyMessage), "Yes".Translate(), delegate ()
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

        public override int getHeight(float width, string selectedMod) { return spacing; }
    }

    public class SplitColumn : SettingContainer
    {
        public float split = 0.50f;
        public List<SettingContainer> leftCol = new List<SettingContainer>();
        public List<SettingContainer> rightCol = new List<SettingContainer>();
        public bool drawLine = false;
        private Spacing gapSize = Spacing.Small;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            Rect baseRect = listingStandard.GetRect(Math.Max(columnHeight(leftCol, listingStandard.ColumnWidth*split - ((int)gapSize), selectedMod), columnHeight(rightCol, listingStandard.ColumnWidth*(1-split) - ((int)gapSize), selectedMod)));
            Rect leftRect = baseRect.LeftPartPixels(baseRect.width*split - ((int)gapSize));
            Rect rightRect = baseRect.RightPartPixels(baseRect.width * (1 - split) - ((int)gapSize));
            Listing_Standard lListing = new Listing_Standard();
            lListing.Begin(leftRect);
            lListing.verticalSpacing = listingStandard.verticalSpacing;
            foreach (SettingContainer setting in leftCol)
            {
                setting.DrawSetting(lListing, selectedMod);
            }
            lListing.End();
            Listing_Standard rListing = new Listing_Standard();
            if(drawLine)
            {
                Color color = GUI.color;
                GUI.color = color * new Color(1f, 1f, 1f, 0.4f);
                GUI.DrawTexture(new Rect(baseRect.center.x, rightRect.yMin, 1f, rightRect.height), BaseContent.WhiteTex);
                GUI.color = color;
            }                
            rListing.Begin(rightRect);
            rListing.verticalSpacing = listingStandard.verticalSpacing;
            foreach (SettingContainer setting in rightCol)
            {
                setting.DrawSetting(rListing, selectedMod);
            }
            rListing.End();
        }

        private enum Spacing : int
        {
            None = 0,
            Tiny = 2,
            Small = 3,
            Medium = 5,
            Large = 9,
            Huge = 15
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

        public override void init()
        {
            if(leftCol != null)
            {
                foreach (SettingContainer setting in leftCol)
                {
                    setting.init();
                }
            }
            if (rightCol != null)
            {
                foreach (SettingContainer setting in rightCol)
                {
                    setting.init();
                }
            }
        }

        public override int getHeight(float width, string selectedMod) { return Math.Max(columnHeight(leftCol, width * split - ((int)gapSize), selectedMod), columnHeight(rightCol, width * (1 - split) - ((int)gapSize), selectedMod)); }
    }

    public class MiddleColumn : SettingContainer
    {
        public float split = 0.50f;
        public List<SettingContainer> settings = new List<SettingContainer>();

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            Rect baseRect = listingStandard.GetRect(columnHeight(settings, listingStandard.ColumnWidth * split, selectedMod)).LeftPart(split/2f+0.5f).RightPart(split/(split / 2f + 0.5f));
            Listing_Standard lListing = new Listing_Standard();
            lListing.Begin(baseRect);
            lListing.verticalSpacing = listingStandard.verticalSpacing;
            foreach (SettingContainer setting in settings)
            {
                setting.DrawSetting(lListing, selectedMod);
            }
            lListing.End();
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
            if (settings != null)
            {
                c = 0;
                foreach (SettingContainer setting in settings)
                {
                    if (!setting.setDefaultValue(modId))
                    {
                        c++;
                        PatchManager.errors.Add("XmlExtensions.Setting.MiddleColumn: Failed to initialize a setting at position: " + c.ToString());
                        return false;
                    }
                }
            }
            return true;
        }

        public override void init()
        {
            if (settings != null)
            {
                foreach (SettingContainer setting in settings)
                {
                    setting.init();
                }
            }
        }

        public override int getHeight(float width, string selectedMod) { return columnHeight(settings, width * split, selectedMod); }
    }

    public class GapLine : SettingContainer
    {
        public int spacing = 24;
        protected int thickness = 1;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod) 
        {
            Rect gapRect = listingStandard.GetRect(spacing);
            float y = gapRect.y + spacing / 2f - thickness / 2f;
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
        public List<SettingContainer> caseTrue = new List<SettingContainer>();
        public List<SettingContainer> caseFalse = new List<SettingContainer>();

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
                    setting.DrawSetting(listing, selectedMod);
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

        public override void init()
        {
            base.init();
            if (caseTrue != null)
            {
                foreach (SettingContainer setting in caseTrue)
                {
                    setting.init();
                }
            }
            if (caseFalse != null)
            {
                foreach (SettingContainer setting in caseFalse)
                {
                    setting.init();
                }
            }
        }

        public override int getHeight(float width, string selectedMod) { return (bool.Parse(XmlMod.allSettings.dataDict[XmlMod.selectedMod + ";" + key]) ? calcHeight(caseTrue, width, selectedMod) : calcHeight(caseFalse, width, selectedMod)); }
    }

    public class ScrollView : SettingContainer
    {
        public float height = 72;
        public List<SettingContainer> settings = new List<SettingContainer>();

        private Vector2 scrollPos = Vector2.zero;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            Rect baseRect = listingStandard.GetRect(height);            
            Rect scrollRect = new Rect(0, 0, baseRect.width - 20f, calcHeight(settings, baseRect.width - 20f, selectedMod));
            Widgets.BeginScrollView(baseRect, ref scrollPos, scrollRect);
            Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect2);
            listing.verticalSpacing = listingStandard.verticalSpacing;
            foreach (SettingContainer setting in settings)
            {
                setting.DrawSetting(listing, selectedMod);
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

        public override void init()
        {
            if (settings != null)
            {
                foreach (SettingContainer setting in settings)
                {
                    setting.init();
                }
            }            
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
                float width2 = 0;
                if (dimensions.x > listingStandard.ColumnWidth)
                {
                    width2 = listingStandard.ColumnWidth;
                }
                else
                {
                    width2 = dimensions.x;
                }
                height = (int)dimensions.y;
                Rect tempRect = listingStandard.GetRect(height);
                if (anchor == "Middle")
                {
                    drawRect = tempRect.LeftPartPixels((tempRect.width + width2) / 2);
                    drawRect = drawRect.RightPartPixels(width2);
                }
                else if (anchor == "Right")
                    drawRect = tempRect.RightPartPixels(width2);
                else
                    drawRect = tempRect.LeftPartPixels(width2);
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
        public List<SwitchSetting> cases = new List<SwitchSetting>();
        private Dictionary<string, List<SettingContainer>> valSettingDict;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            try
            {
                List<SettingContainer> settings = valSettingDict[XmlMod.allSettings.dataDict[selectedMod + ";" + key]];
                foreach (SettingContainer setting in settings)
                {
                    setting.DrawSetting(listingStandard, selectedMod);
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
                foreach (SwitchSetting switchSetting in cases)
                {
                    if(switchSetting.settings != null)
                    {
                        foreach (SettingContainer setting in switchSetting.settings)
                        {
                            setting.init();
                        }
                    }                    
                }
            }            
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
        public List<SettingContainer> settings = new List<SettingContainer>();

        public override int getHeight(float width, string selectedMod)
        {
            int h = 0;
            if(settings != null)
            {
                foreach (SettingContainer setting in settings)
                {
                    h += setting.GetHeight(width, selectedMod);
                }                
            }
            return h;
        }
    }

    public class TabView : SettingContainer
    {
        protected List<Tab> tabs = new List<Tab>();
        private List<TabRecord> tabRecords;
        private int selectedTab = 0;
        private float tabHeight = 32;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            
            Rect rectTab = listingStandard.GetRect(tabs[selectedTab].getHeight(listingStandard.ColumnWidth, selectedMod) + tabHeight+4);
            rectTab.yMin += tabHeight;
            TabDrawer.DrawTabs<TabRecord>(rectTab, tabRecords, 200f);            
            Listing_Standard tempListing = new Listing_Standard();
            tempListing.Begin(rectTab);
            
            for (int i = 0; i < tabs.Count; i++)
            {
                if (selectedTab == i)
                {
                    foreach(SettingContainer setting in tabs[i].settings)
                    {
                        setting.DrawSetting(tempListing, selectedMod);
                    }
                }
            }
            tempListing.End();
        }

        public override int getHeight(float width, string selectedMod)
        {
            return tabs[selectedTab].getHeight(width, selectedMod) + (int)tabHeight+4;
        }

        public override void init()
        {
            base.init();
            if(tabs != null)
            {
                foreach (Tab tab in tabs)
                {
                    if (tab.settings != null)
                    {
                        foreach (SettingContainer setting in tab.settings)
                        {
                            setting.init();
                        }
                    }                    
                }
            }
            
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

    public class Section : SettingContainer
    {
        public float height = -1f;
        public List<SettingContainer> settings = new List<SettingContainer>();
        public float padding = 4f;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            Rect rect = listingStandard.GetRect(getHeight(listingStandard.ColumnWidth, selectedMod));
            Widgets.DrawMenuSection(rect);
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.verticalSpacing = listingStandard.verticalSpacing;
            Rect rect2 = new Rect(rect.x + padding, rect.y + padding, rect.width - padding * 2f, rect.height - padding * 2f);
            listing_Standard.Begin(rect2);
            foreach (SettingContainer setting in settings)
            {
                setting.DrawSetting(listing_Standard, selectedMod);
            }
            listing_Standard.End();
        }

        public override int getHeight(float width, string selectedMod)
        {
            if(height<0)
            {
                int h = 0;
                if (settings != null)
                {
                    foreach (SettingContainer setting in settings)
                    {
                        h += setting.GetHeight(width - padding * 2f, selectedMod);
                    }
                }
                return h + (int)padding * 2;
            }
            else
            {
                return (int)height + (int)padding * 2;
            }
        }

        public override bool setDefaultValue(string modId)
        {
            int t = 0;
            foreach (SettingContainer setting in settings)
            {
                t++;
                if (!setting.setDefaultValue(modId))
                {
                    PatchManager.errors.Add("XmlExtensions.Setting.Section: Failed to initialize a setting at position=" + t.ToString());
                    return false;
                }
            }
            return true;
        }

        public override void init()
        {
            base.init();
            if (settings != null)
            {
                foreach (SettingContainer setting in settings)
                {
                    setting.init();
                }
            }
        }
    }

    public class Group : SettingContainer
    {
        public List<SettingContainer> settings = new List<SettingContainer>();

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            Rect rect = listingStandard.GetRect(getHeight(listingStandard.ColumnWidth, selectedMod));
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.verticalSpacing = listingStandard.verticalSpacing;
            listing_Standard.Begin(rect);
            foreach (SettingContainer setting in settings)
            {
                setting.DrawSetting(listing_Standard, selectedMod);
            }
            listing_Standard.End();
        }

        public override int getHeight(float width, string selectedMod)
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
            int t = 0;
            foreach (SettingContainer setting in settings)
            {
                t++;
                if (!setting.setDefaultValue(modId))
                {
                    PatchManager.errors.Add("XmlExtensions.Setting.Group: Failed to initialize a setting at position=" + t.ToString());
                    return false;
                }
            }
            return true;
        }

        public override void init()
        {
            base.init();
            if (settings != null)
            {
                foreach (SettingContainer setting in settings)
                {
                    setting.init();
                }
            }
        }
    }

}
