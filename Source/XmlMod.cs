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
        public static string selectedExtraMod;
        public static bool viewingSettings = false;

        static XmlMod()
        {
            allSettings = new XmlModBaseSettings();
            settingsPosition = new Vector2();
            modListPosition = new Vector2();
            selectedMod = null;
            loadedXmlMods = new List<string>();
            selectedExtraMod = null;
        }

        public XmlMod(ModContentPack content) : base(content)
        {
            allSettings = GetSettings<XmlModBaseSettings>();
        }


        public static void DrawSettingsWindow(Rect inRect)
        {
            Rect rectSettings = inRect.RightPart(0.7f);
            Rect rectMods = inRect.LeftPart(0.285f);
            drawXmlModSettings(rectSettings);
            drawXmlModList(rectMods);
        }


        private static void drawXmlModSettings(Rect rect)
        {
            if (selectedMod != null)
            {
                Rect scrollRect = new Rect(0, 0, rect.width - 20f, settingsPerMod[selectedMod].calculateHeight(rect.width - 20f, selectedMod));
                Widgets.BeginScrollView(rect, ref settingsPosition, scrollRect);
                Listing_Standard listingStandard = new Listing_Standard();
                Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
                listingStandard.Begin(rect2);
                listingStandard.verticalSpacing = settingsPerMod[selectedMod].defaultSpacing;
                //listingStandard.Label(settingsPerMod[selectedMod].label);
                foreach (SettingContainer setting in settingsPerMod[selectedMod].settings)
                {
                    setting.DrawSetting(listingStandard, selectedMod);
                }
                GUI.color = Color.white;
                listingStandard.End();
                Widgets.EndScrollView();
            }
            else
            {
                drawXmlExtensionsSettings(rect);
            }
        }
        
        private static void drawXmlExtensionsSettings(Rect rect)
        {
            if (viewingSettings)
            {
                drawXmlSettingsList(rect);
            }
            else
            {
                Listing_Standard listingStandard = new Listing_Standard();
                listingStandard.Begin(rect);
                listingStandard.CheckboxLabeled(Helpers.tryTranslate("Enable stack trace for XML patch errors", "XmlExtensions_EnableStackTrace"), ref allSettings.trace);
                if (listingStandard.ButtonText(Helpers.tryTranslate("View unused settings", "XmlExtensions_ViewUnusedSettings")))
                    viewingSettings = true;
                listingStandard.End();
            }
        }


        private static void drawXmlSettingsList(Rect rect)
        {
            int keyCount = 0;
            int unique = 0;
            string tId = "";
            List<KeyValuePair<string, string>> kvpList = XmlMod.allSettings.dataDict.ToList<KeyValuePair<string, string>>();
            List<string> keyList = new List<string>();
            kvpList.Sort(delegate (KeyValuePair<string, string> pair1, KeyValuePair<string, string> pair2) { return pair1.Key.CompareTo(pair2.Key); });
            foreach (KeyValuePair<string, string> pair in kvpList)
            {
                if (pair.Key.Contains(";"))
                {
                    if (!loadedXmlMods.Contains(pair.Key.Split(';')[0]) || !settingsPerMod[pair.Key.Split(';')[0]].keys.Contains(pair.Key.Split(';')[1]))
                    {
                        if (selectedExtraMod == pair.Key.Split(';')[0])
                            keyCount++;
                        if (tId != pair.Key.Split(';')[0])
                        {
                            tId = pair.Key.Split(';')[0];
                            unique++;
                        }
                        keyList.Add(pair.Key);
                    }
                }
                else
                {
                    XmlMod.allSettings.dataDict.Remove(pair.Key);
                }

            }
            keyList.Sort();
            Listing_Standard listingStandard = new Listing_Standard();
            if (selectedExtraMod == null)
            {
                Rect scrollRect = new Rect(0, 0, rect.width - 20f, unique * 32 + 32 + 12 + 24 + 32);
                Widgets.BeginScrollView(rect, ref settingsPosition, scrollRect);
                Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
                listingStandard.Begin(rect2);
                listingStandard.Label(Helpers.tryTranslate("Settings currently not being used by loaded mods:", "XmlExtensions_UnloadedSettings"));
                listingStandard.GapLine(4);
                listingStandard.Gap(2);
                if (keyList.Count == 0)
                {
                    listingStandard.Label(Helpers.tryTranslate("No extra settings at the moment.", "XmlExtensions_NoExtraSettings"));
                }
                string temp = "";
                foreach (string key in keyList)
                {
                    string tempId = key.Split(';')[0];
                    if (temp != tempId)
                    {
                        temp = tempId;
                        if (listingStandard.ButtonText(temp))
                        {
                            selectedExtraMod = temp;
                        }
                    }

                }
                listingStandard.GapLine(4);
                listingStandard.Gap(2);
                if (listingStandard.ButtonText(Helpers.tryTranslate("Delete all extra settings", "XmlExtensions_DeleteAllExtraSettings"), null))
                {
                    Find.WindowStack.Add(new Dialog_MessageBox(Helpers.tryTranslate("Are you sure you want to reset every setting of every unloaded mod?", "XmlExtensions_ConfirmResetAll"), "Yes".Translate(), delegate ()
                    {
                        foreach (string key in keyList)
                        {
                            XmlMod.allSettings.dataDict.Remove(key);
                        }
                        selectedExtraMod = null;
                    }, "No".Translate(), null, null, false, null, null));
                }
                if (listingStandard.ButtonText(Helpers.tryTranslate("Back", "XmlExtensions_Back")))
                    viewingSettings = false;
                listingStandard.End();
                Widgets.EndScrollView();
            }
            else
            {
                Rect scrollRect = new Rect(0, 0, rect.width - 20f, keyCount * 24 + 12 + 32 + 32 + 24);
                Widgets.BeginScrollView(rect, ref settingsPosition, scrollRect);
                Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
                listingStandard.Begin(rect2);
                listingStandard.Label(Helpers.tryTranslate("Currently selected mod's unused settings:", "XmlExtensions_SelectedModUnusedSettings"));
                listingStandard.GapLine(4);
                listingStandard.Gap(2);
                foreach (string key in keyList)
                {
                    string tempId = key.Split(';')[0];
                    if (selectedExtraMod == tempId)
                    {
                        bool del = false;
                        listingStandard.CheckboxLabeled(key.Split(';')[1] + ": " + XmlMod.allSettings.dataDict[key], ref del, "Delete");
                        if (del)
                        {
                            XmlMod.allSettings.dataDict.Remove(key);
                        }
                    }

                }
                listingStandard.GapLine(4);
                listingStandard.Gap(2);
                if (listingStandard.ButtonText(Helpers.tryTranslate("Delete extra settings", "XmlExtensions_DeleteExtraSettings"), null))
                {
                    Find.WindowStack.Add(new Dialog_MessageBox(Helpers.tryTranslate("Are you sure you want to reset every setting of the current mod?", "XmlExtensions_ConfirmationResetMod"), "Yes".Translate(), delegate ()
                    {
                        foreach (string key in keyList)
                        {
                            if (selectedExtraMod == key.Split(';')[0])
                                XmlMod.allSettings.dataDict.Remove(key);
                        }
                        selectedExtraMod = null;
                    }, "No".Translate(), null, null, false, null, null));
                }
                if (listingStandard.ButtonText("Back"))
                    selectedExtraMod = null;
                listingStandard.End();
                Widgets.EndScrollView();
            }
        }

        public static int tempInt = 600;
    


        private static void drawXmlModList(Rect rect)
        {
            Rect scrollRect = new Rect(0, 0, rect.width - 20f, Math.Max(loadedXmlMods.Count * (30 + 2) + 38, rect.height+1));
            Widgets.BeginScrollView(rect, ref modListPosition, scrollRect);
            Listing_Standard listingStandard = new Listing_Standard();
            Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
            listingStandard.Begin(rect2);
            foreach (string modId in loadedXmlMods)
            {
                bool t = false;
                // TODO: Translate label
                t = listingStandard.ButtonText(Helpers.tryTranslate(settingsPerMod[modId].label, settingsPerMod[modId].tKey));
                if (t)
                {
                    selectedMod = modId;
                    selectedExtraMod = null;
                    viewingSettings = false;
                }
            }
            listingStandard.GapLine(4);
            listingStandard.Gap(2);
            bool t1 = false;
            t1 = listingStandard.ButtonText(Helpers.tryTranslate("XML Extensions", "XmlExtensions_Label"));
            if (t1)
            {
                selectedMod = null;
                selectedExtraMod = null;
                viewingSettings = false;
            }
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

        public static void addXmlMod(string modId)
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
                settingsPerMod.Add(modId, t);
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
