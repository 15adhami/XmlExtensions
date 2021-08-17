using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using XmlExtensions.Setting;

namespace XmlExtensions
{
    public class XmlMod : Mod
    {
        public static XmlModBaseSettings allSettings;
        public static Vector2 settingsPosition;
        public static Vector2 modListPosition;
        public static string loadedMod;
        public static List<string> loadedXmlMods;
        public static Dictionary<string, XmlModSettings> settingsPerMod;
        public static string selectedMod;
        static XmlMod()
        {
            allSettings = new XmlModBaseSettings();
            settingsPosition = new Vector2();
            modListPosition = new Vector2();
            selectedMod = null;
            loadedXmlMods = new List<string>();
        }

        public XmlMod(ModContentPack content) : base(content)
        {
            allSettings = GetSettings<XmlModBaseSettings>();
        }

        public override string SettingsCategory()
        {
            return "More Mod Settings";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Rect rectSettings = inRect.RightPart(0.7f);
            Rect rectMods = inRect.LeftPart(0.285f);
            drawXmlModSettings(rectSettings);
            drawXmlModList(rectMods);
            base.DoSettingsWindowContents(inRect);
        }

        private void drawXmlModSettings(Rect rect)
        {
            if (selectedMod != null)
            {
                Rect scrollRect = new Rect(0, 0, rect.width - 16f, settingsPerMod[selectedMod].calculateHeight());
                Widgets.BeginScrollView(rect, ref settingsPosition, scrollRect);
                Listing_Standard listingStandard = new Listing_Standard();
                Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
                listingStandard.Begin(rect2);
                listingStandard.verticalSpacing = settingsPerMod[selectedMod].defaultSpacing;
                //listingStandard.Label(settingsPerMod[selectedMod].label);
                foreach (SettingContainer setting in settingsPerMod[selectedMod].settings)
                {
                    setting.drawSetting(listingStandard, selectedMod);
                }
                GUI.color = Color.white;
                listingStandard.End();
                Widgets.EndScrollView();
            }
            else
            {              
                List<KeyValuePair<string, string>> kvpList = XmlMod.allSettings.dataDict.ToList<KeyValuePair<string, string>>();
                List<string> keyList = new List<string>();
                foreach (KeyValuePair<string, string> pair in kvpList)
                {
                    if(pair.Key.Contains(";"))
                    {
                        if (!loadedXmlMods.Contains(pair.Key.Split(';')[0]) || !settingsPerMod[pair.Key.Split(';')[0]].keys.Contains(pair.Key.Split(';')[1]))
                        {
                            keyList.Add(pair.Key);
                        }
                    }
                    else
                    {
                        XmlMod.allSettings.dataDict.Remove(pair.Key);
                    }
                    
                }
                keyList.Sort();                
                Rect scrollRect = new Rect(0, 0, rect.width - 16f, keyList.Count * 24 + 32 + 22);
                Listing_Standard listingStandard = new Listing_Standard();
                Widgets.BeginScrollView(rect, ref settingsPosition, scrollRect);
                Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
                listingStandard.Begin(rect2);
                listingStandard.Label("Settings currently not being used by loaded mods:");
                if (keyList.Count == 0)
                {
                    listingStandard.Label("No extra settings at the moment.");
                }
                foreach (string key in keyList)
                {
                    bool del = false;
                    listingStandard.CheckboxLabeled(key.Split(';')[0]+"."+ key.Split(';')[1], ref del, "Delete");
                    if (del)
                    {
                        XmlMod.allSettings.dataDict.Remove(key);
                    }                                  
                }
                if (listingStandard.ButtonText("Delete all extra settings", null))
                {
                    Find.WindowStack.Add(new Dialog_MessageBox("XmlExtensions_Confirmation".Translate(), "Yes".Translate(), delegate ()
                    {
                        foreach (string key in keyList)
                        {
                            XmlMod.allSettings.dataDict.Remove(key);
                        }
                    }, "No".Translate(), null, null, false, null, null));
                }
                listingStandard.End();
                Widgets.EndScrollView();
            }
        }


        public static int tempInt = 600;
        private void drawXmlModList(Rect rect)
        {
            Rect scrollRect = new Rect(0, 0, rect.width - 16f, Math.Max(loadedXmlMods.Count * (30 + 2) + 38, 585));
            Widgets.BeginScrollView(rect, ref modListPosition, scrollRect);
            Listing_Standard listingStandard = new Listing_Standard();
            Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
            listingStandard.Begin(rect2);
            foreach (string modId in loadedXmlMods)
            {
                bool t = false;
                t = listingStandard.ButtonText(settingsPerMod[modId].label);
                if (t) { selectedMod = modId; }
            }
            listingStandard.GapLine(4);
            listingStandard.Gap(2);
            bool t1 = false;
            t1 = listingStandard.ButtonText("XML Extensions");
            if (t1) { selectedMod = null; }
            /*
            float f = (float)(tempInt);
            string buf = f.ToString();
            listingStandard.TextFieldNumericLabeled<float>("height", ref f, ref buf, 0, 99999);
            tempInt = (int)f;
            */
            listingStandard.End();
            Widgets.EndScrollView();
        }


        public static void addSetting(string modId, string key, string value)
        {
            if (allSettings == null)
            {
                allSettings = new XmlModBaseSettings();
            }
            allSettings.dataDict.Add(modId + ";" + key, value);
            if(settingsPerMod[modId].defValues == null)
            {
                settingsPerMod[modId].defValues = new Dictionary<string, string>();
            }
            if (!settingsPerMod[modId].defValues.ContainsKey(key))
            {
                settingsPerMod[modId].defValues.Add(key, value);
            }
        }

        public static bool tryGetSetting(string modId, string key, out string value)
        {
            string temp = "";
            bool b;
            b = allSettings.dataDict.TryGetValue(modId + ";" + key, out temp);
            value = temp;
            return b;
        }

        public static void addXmlMod(string modId, string label)
        {
            if (loadedXmlMods == null)
            {
                loadedXmlMods = new List<string>();
            }
            if (!loadedXmlMods.Contains(modId))
            {
                loadedXmlMods.Add(modId);
            }
            if (settingsPerMod == null)
            {
                settingsPerMod = new Dictionary<string, XmlModSettings>();
            }
            if (!settingsPerMod.Keys.Contains(modId))
            {
                XmlModSettings t = new XmlModSettings(modId);
                t.label = label;
                settingsPerMod.Add(modId, t);
            }
            else
            {
                settingsPerMod[modId].label = label;
            }
            if (settingsPerMod[modId].defValues == null)
            {
                settingsPerMod[modId].defValues = new Dictionary<string, string>();
            }
        }

        public static void tryAddSettings(SettingContainer container, string modId)
        {
            if (settingsPerMod[modId].settings == null)
            {
                settingsPerMod[modId].settings = new List<SettingContainer>();
            }
            settingsPerMod[modId].settings.Add(container);
        }

        public static string getSetting(string modId, string key)
        {
            return allSettings.dataDict.TryGetValue<string, string>(modId + ";" + key);
        }

        public static void setSetting(string modId, string key, string value)
        {
            string fullKey = modId + ";" + key;
            if (allSettings.dataDict.ContainsKey(fullKey))
            {
                allSettings.dataDict[fullKey] = value;
            }
            else
            {
                addSetting(modId, key, value);
            }
        }
    }
}
