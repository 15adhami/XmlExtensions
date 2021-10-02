using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace XmlExtensions
{
    public class XmlMod : Mod
    {
        public static XmlModBaseSettings allSettings;
        public static Vector2 settingsPosition;
        public static Vector2 modListPosition;
        public static Vector2 keyPosition;
        public static string loadedMod;
        public static List<string> loadedXmlMods;
        public static Dictionary<string, XmlModSettings> settingsPerMod;
        public static string selectedMod;
        public static string selectedExtraMod;
        public static bool viewingSettings = false;
        public static Dictionary<string, SettingsMenuDef> menus;
        public static string activeMenu = null;
        public static Dictionary<string, List<string>> unusedSettings;
        public static List<string> unusedMods;

        static XmlMod()
        {
            var harmony = new Harmony("com.github.15adhami.xmlextensions");
            harmony.PatchAll();
            unusedMods = new List<string>();
            unusedSettings = new Dictionary<string, List<string>>();
            menus = new Dictionary<string, SettingsMenuDef>();
            allSettings = new XmlModBaseSettings();
            settingsPosition = new Vector2();
            modListPosition = new Vector2();
            selectedMod = null;
            loadedXmlMods = new List<string>();
            selectedExtraMod = null;
            keyPosition = new Vector2();
        }

        public XmlMod(ModContentPack content) : base(content)
        {
            allSettings = GetSettings<XmlModBaseSettings>();
        }

        public static void DrawSettingsWindow(Rect inRect)
        {
            Rect rectSettings = inRect.RightPartPixels(inRect.width-256-6);
            Rect rectMods = inRect.LeftPartPixels(256); //.285f
            drawXmlModSettings(rectSettings);
            drawXmlModList(rectMods);
        }

        private static void drawXmlModSettings(Rect rect)
        {
            if (selectedMod != null)
            {
                Rect scrollRect = new Rect(0, 0, rect.width - 20f, menus[activeMenu].CalculateHeight(rect.width - 20f, selectedMod));
                Widgets.BeginScrollView(rect, ref settingsPosition, scrollRect);
                Listing_Standard listingStandard = new Listing_Standard();
                Rect rect2 = new Rect(0f, 0f, scrollRect.width, 999999f);
                listingStandard.Begin(rect2);
                menus[activeMenu].DrawSettings(listingStandard);                
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
            Rect modListRect = rect.LeftPartPixels(300f);
            Rect keyListRect = rect.RightPartPixels(rect.width - 300f - 8f);
            Listing_Standard modListListing = new Listing_Standard();
            modListListing.Begin(modListRect);
            modListListing.Label(Helpers.tryTranslate("Currently inactive mods:", "XmlExtensions_InactiveMods"));
            modListListing.GapLine(4);
            modListListing.Gap(2);
            Rect modRect = modListListing.GetRect(rect.height - 30f);
            Rect scrollRect = new Rect(0, 0, modRect.width - 20f, Math.Max(unusedMods.Count > 0 ? unusedMods.Count * 32 : 30, rect.height - 29f));
            Widgets.BeginScrollView(modRect, ref settingsPosition, scrollRect);
            Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
            Listing_Standard modListing = new Listing_Standard();
            modListing.Begin(rect2);
            if (unusedMods.Count == 0)
            {
                modListing.Label(Helpers.tryTranslate("No extra settings at the moment", "XmlExtensions_NoExtraSettings"));
            }
            foreach (string mod in unusedMods)
            {
                if (modListing.ButtonText(mod)) { selectedExtraMod = mod; }
            }
            modListing.End();
            Widgets.EndScrollView();
            modListListing.End();

            // Draw right column
            if (selectedExtraMod != null)
            {
                Listing_Standard keyListListing = new Listing_Standard();
                keyListListing.Begin(keyListRect);
                keyListListing.Label(Helpers.tryTranslate("Currently selected mod's unused settings:", "XmlExtensions_SelectedModUnusedKeys"));
                keyListListing.GapLine(4);
                keyListListing.Gap(2);

                Rect keyRect = keyListListing.GetRect(rect.height - 30f - 36f);
                Rect scrollRect2 = new Rect(0, 0, keyRect.width - 20f, Math.Max(unusedSettings[selectedExtraMod].Count * 24f, rect.height - 29f - 36f));
                Widgets.BeginScrollView(keyRect, ref keyPosition, scrollRect2);
                rect2 = new Rect(0f, 0f, scrollRect2.width, 99999f);
                Listing_Standard keyListing = new Listing_Standard();
                keyListing.Begin(rect2);
                string tempStr = null;
                foreach (string key in unusedSettings[selectedExtraMod])
                {
                    bool del = false;
                    keyListing.CheckboxLabeled(key + ": " + SettingsManager.GetSetting(selectedExtraMod, key), ref del, Helpers.tryTranslate("Delete", "XmlExtensions_Delete"));
                    if (del)
                    {
                        DeleteSetting(selectedExtraMod, key);
                        tempStr = key;
                    }
                }
                if (tempStr != null)
                {
                    unusedSettings[selectedExtraMod].Remove(tempStr);
                    if (unusedSettings[selectedExtraMod].Count == 0)
                    {
                        unusedMods.Remove(selectedExtraMod);
                        unusedSettings.Remove(selectedExtraMod);
                        selectedExtraMod = null;
                    }
                }
                keyListing.End();
                Widgets.EndScrollView();
                // Make sure the last key wasn't just deleted (otherwise missing key in dict exception)
                if (selectedExtraMod != null)
                {
                    keyListListing.GapLine(4);
                    Rect tempRect = keyListListing.GetRect(32f);
                    Rect firstRect = tempRect.LeftPart(0.495f);
                    Listing_Standard buttonLeftListing = new Listing_Standard();
                    buttonLeftListing.Begin(firstRect);
                    if (buttonLeftListing.ButtonText(Helpers.tryTranslate("Delete {0} keys", "XmlExtensions_DeleteKeys").Replace("{0}", unusedSettings[selectedExtraMod].Count.ToString())))
                    {
                        Find.WindowStack.Add(new Dialog_MessageBox(Helpers.tryTranslate("Are you sure you want to delete every unused key of the current mod?", "XmlExtensions_ConfirmationResetMod"), "Yes".Translate(), delegate ()
                        {
                            foreach (string key in unusedSettings[selectedExtraMod])
                            {
                                DeleteSetting(selectedExtraMod, key);
                            }
                            unusedMods.Remove(selectedExtraMod);
                            unusedSettings.Remove(selectedExtraMod);
                            selectedExtraMod = null;
                        }, "No".Translate(), null, null, false, null, null));
                    }
                    buttonLeftListing.End();

                    Rect secondRect = tempRect.RightPart(0.495f);
                    Listing_Standard buttonRightListing = new Listing_Standard();
                    buttonRightListing.Begin(secondRect);
                    GUI.color = Color.red;
                    int count = 0;
                    foreach (List<string> list in unusedSettings.Values)
                    {
                        count += list.Count;
                    }
                    if (buttonRightListing.ButtonText(Helpers.tryTranslate("Delete all {0} unused keys", "XmlExtensions_DeleteAllUnusedKeys").Replace("{0}", count.ToString()), null))
                    {
                        Find.WindowStack.Add(new Dialog_MessageBox(Helpers.tryTranslate("Are you sure you want to delete all unused keys from every mod?", "XmlExtensions_ConfirmResetAll"), "Yes".Translate(), delegate ()
                        {
                            foreach (string mod in unusedMods)
                            {
                                foreach (string key in unusedSettings[mod])
                                {
                                    DeleteSetting(mod, key);
                                }
                            }
                            unusedSettings.Clear();
                            unusedMods.Clear();
                            selectedExtraMod = null;
                        }, "No".Translate(), null, null, false, null, null));
                    }
                    GUI.color = Color.white;
                    buttonRightListing.End();
                }                
                keyListListing.End();
            }
            
        }

        private static void drawXmlModList(Rect rect)
        {
            int count = 0;
            foreach(string modId in loadedXmlMods)
            {
                if (settingsPerMod[modId].label != null)
                    count++;
            }
            Rect scrollRect = new Rect(0, 0, rect.width - 20f, Math.Max((loadedXmlMods.Count-count) * (30 + 2) + 38, rect.height+1));
            Widgets.BeginScrollView(rect, ref modListPosition, scrollRect);
            Listing_Standard listingStandard = new Listing_Standard();
            Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
            listingStandard.Begin(rect2);
            foreach (string modId in loadedXmlMods)
            {
                if (settingsPerMod[modId].label != null)
                {
                    bool t = false;
                    t = listingStandard.ButtonText(Helpers.tryTranslate(settingsPerMod[modId].label, settingsPerMod[modId].tKey));
                    if (t)
                    {
                        selectedMod = modId;
                        activeMenu = settingsPerMod[modId].homeMenu;
                        selectedExtraMod = null;
                        viewingSettings = false;
                    }
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
            settingsPerMod[modId].label = label;
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

        public static void DeleteSetting(string modId, string key)
        {
            allSettings.dataDict.Remove(modId + ';' + key);
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
