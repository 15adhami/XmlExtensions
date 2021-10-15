﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using XmlExtensions.Action;

namespace XmlExtensions
{
    // Window that appears when you press More Mod Settings
    public class XmlExtensions_SettingsMenu : Window
    {
        public static string selectedExtraMod;
        public static string activeMenu = null; // defName

        public static Vector2 settingsPosition;
        public static Vector2 modListPosition;
        public static Vector2 keyPosition;
        public static bool viewingSettings = false;
        public static ModContainer SelectedMod = null;

        private List<ModContainer> loadedMods;
        private List<ModContainer> cachedFilteredList;

        private static ModContainer prevMod = null;
        private string searchText = "";
        private static Dictionary<string, string> oldValuesCache;
        private bool focusSearchBox = false;

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(900f + 256f + 6f, 700f);
            }
        }

        public XmlExtensions_SettingsMenu()
        {
            loadedMods = new List<ModContainer>();
            cachedFilteredList = new List<ModContainer>();
            oldValuesCache = new Dictionary<string, string>();
            doCloseButton = true;
            forcePause = true;
            absorbInputAroundWindow = true;
            doCloseX = true;
            closeOnAccept = false;
        }

        public override void PreOpen()
        {
            base.PreOpen();
            focusSearchBox = true;
            foreach (string id in XmlMod.loadedXmlMods)
            {
                loadedMods.Add(new ModContainer(id));
            }
            if (XmlMod.allSettings.vanillaMods)
            {
                foreach (Mod item in from mod in LoadedModManager.ModHandles
                                     where !mod.SettingsCategory().NullOrEmpty()
                                     select mod)
                {
                    loadedMods.Add(new ModContainer(item));
                }
            }
            loadedMods.Sort();
            CacheFilter();
            if (prevMod != null)
            {
                foreach (ModContainer mod in loadedMods)
                {
                    if (mod.ToString() == prevMod.ToString())
                    {
                        SetSelectedMod(mod);
                    }
                }
            }
            else
            {
                SelectedMod = null;
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect rectSettings = inRect.RightPartPixels(inRect.width - 256 - 6).TopPartPixels(inRect.height - 40);
            Rect headerRect = rectSettings.TopPartPixels(40f);
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(headerRect);
            Text.Font = GameFont.Medium;
            listing.verticalSpacing = 0;
            listing.Label(Helpers.TryTranslate("Mod settings for ", "XmlExtensions_ModSettingsFor") + (SelectedMod != null ? SelectedMod.ToString() : "XML Extensions"));
            Text.Font = GameFont.Small;
            listing.GapLine(9f);
            listing.End();
            GUI.BeginGroup(rectSettings);
            Verse.Text.Font = Verse.GameFont.Small;
            drawXmlModSettings(rectSettings);
            Verse.Text.Font = Verse.GameFont.Small;
            GUI.EndGroup();

            string temp = searchText;
            Rect rectMods = inRect.LeftPartPixels(256f).TopPartPixels(inRect.height - 40); //.285f
            GUI.SetNextControlName("searchbox");
            searchText = Widgets.TextField(rectMods.TopPartPixels(22).LeftPartPixels(rectMods.width - 22), temp);
            GUI.color *= new Color(0.33f, 0.33f, 0.33f);
            if (searchText == null || searchText == "")
            {
                GUI.color = new Color(0.5f, 0.5f, 0.5f);
                GUI.DrawTexture(rectMods.TopPartPixels(22).RightPartPixels(22), TexButton.Search);
            }
            else
            {
                if (Widgets.ButtonImage(rectMods.TopPartPixels(22).RightPartPixels(22).ContractedBy(2f), TexButton.CloseXSmall))
                {
                    searchText = "";
                }
            }
            if (searchText != temp)
            {
                CacheFilter();
            }
            GUI.color = Color.white;
            if (focusSearchBox)
            {
                GUI.FocusControl("searchbox");
                focusSearchBox = false;
            }
            drawXmlModList(rectMods.BottomPartPixels(rectMods.height - 24));
        }

        private void drawXmlModList(Rect rect)
        {
            int count = 0;
            foreach (string modId in XmlMod.loadedXmlMods)
            {
                if (XmlMod.settingsPerMod[modId].label != null)
                    count++;
            }
            Rect scrollRect = new Rect(0, 0, rect.width - 20f, Math.Max(cachedFilteredList.Count * (30 + 2) + 38, rect.height + 1));
            Widgets.BeginScrollView(rect, ref modListPosition, scrollRect);
            Listing_Standard listingStandard = new Listing_Standard();
            Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
            listingStandard.Begin(rect2);

            // Draw the button list
            foreach (ModContainer mod in cachedFilteredList)
            {
                if (mod == SelectedMod)
                {
                    GUI.color = new Color(0.7f, 0.7f, 0.7f);
                }
                if (listingStandard.ButtonText(mod.ToString()))
                {
                    SetSelectedMod(mod);
                }
                GUI.color = Color.white;
            }

            listingStandard.GapLine(4);
            listingStandard.Gap(2);
            if (SelectedMod == null)
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
            }
            if (listingStandard.ButtonText(Helpers.TryTranslate("XML Extensions", "XmlExtensions_Label")))
            {
                SetSelectedMod(null);
            }
            GUI.color = Color.white;
            listingStandard.End();
            Widgets.EndScrollView();
        }

        public static void SetSelectedMod(ModContainer mod)
        {
            // Run KeyedActions
            if (SelectedMod != null && SelectedMod.IsXmlMod() && XmlMod.keyedActionListDict.ContainsKey(SelectedMod.modId))
            {
                foreach (string key in XmlMod.keyedActionListDict[SelectedMod.modId].Keys)
                {
                    foreach (KeyedAction action in XmlMod.keyedActionListDict[SelectedMod.modId][key])
                    {
                        if (!action.DoKeyedAction(oldValuesCache[key], SettingsManager.GetSetting(SelectedMod.modId, key)))
                        {
                            ErrorManager.PrintErrors();
                        }
                    }
                }
            }
            oldValuesCache.Clear();
            if (mod != null && mod.IsXmlMod())
            {
                // Cache values for next mod
                if (XmlMod.keyedActionListDict.ContainsKey(mod.modId))
                {
                    foreach (string key in XmlMod.keyedActionListDict[mod.modId].Keys)
                    {
                        oldValuesCache.Add(key, SettingsManager.GetSetting(mod.modId, key));
                    }
                }
                SetActiveMenu(XmlMod.settingsPerMod[mod.modId].homeMenu);
            }
            else
            {
                SetActiveMenu(null);
            }
            if (SelectedMod != null)
            {
                SelectedMod.WriteSettings();
            }
            SelectedMod = mod;
            selectedExtraMod = null;
            viewingSettings = false;
        }

        private void drawXmlModSettings(Rect rect)
        {
            rect.x = 0;
            rect.y = 0;
            if (SelectedMod != null)
            {
                if (SelectedMod.IsXmlMod())
                {
                    Rect scrollRect = new Rect(0, 0, rect.width - 20f, XmlMod.menus[activeMenu].CalculateHeight(rect.width - 20f, SelectedMod.modId));
                    Widgets.BeginScrollView(rect.BottomPartPixels(rect.height - 40), ref settingsPosition, scrollRect);
                    Rect rect2 = new Rect(0f, 0f, scrollRect.width, 999999f);
                    XmlMod.menus[activeMenu].DrawSettings(rect2);
                    if (XmlMod.menus[activeMenu].onFrameActions != null)
                    {
                        ErrorManager.ClearErrors();
                        foreach (MenuAction action in XmlMod.menus[activeMenu].onFrameActions)
                        {
                            if (!action.DoAction())
                            {
                                ErrorManager.PrintErrors();
                            }
                        }
                    }
                    GUI.color = Color.white;
                    Widgets.EndScrollView();
                }
                else
                {
                    SelectedMod.mod.DoSettingsWindowContents(rect.BottomPartPixels(rect.height - 40));
                }
            }
            else
            {
                drawXmlExtensionsSettings(rect.BottomPartPixels(rect.height - 40f));
            }
        }

        private void drawXmlExtensionsSettings(Rect rect)
        {
            if (viewingSettings)
            {
                drawXmlSettingsList(rect);
            }
            else
            {
                Listing_Standard listingStandard = new Listing_Standard();
                listingStandard.Begin(rect);
                listingStandard.CheckboxLabeled(Helpers.TryTranslate("Enable stack trace for XML patch errors", "XmlExtensions_EnableStackTrace"), ref XmlMod.allSettings.trace, Helpers.TryTranslate("Improves error reporting when enabled", "XmlExtensions_StackTraceTip"));
                bool b = XmlMod.allSettings.vanillaMods;
                listingStandard.CheckboxLabeled(Helpers.TryTranslate("Include standard Mod Settings", "XmlExtensions_IncludeStandardMods"), ref b, Helpers.TryTranslate("Include settings from mods that do not use XML Extensions (does not support settings created via HugsLib)", "XmlExtensions_IncludeStandardModsTip"));
                if (b != XmlMod.allSettings.vanillaMods)
                {
                    loadedMods.Clear();
                    foreach (string id in XmlMod.loadedXmlMods)
                    {
                        loadedMods.Add(new ModContainer(id));
                    }
                    if (b)
                    {
                        foreach (Mod item in from mod in LoadedModManager.ModHandles
                                     where !mod.SettingsCategory().NullOrEmpty()
                                     select mod)
                        {
                            loadedMods.Add(new ModContainer(item));
                        }
                    }
                    loadedMods.Sort();
                    CacheFilter();
                }
                XmlMod.allSettings.vanillaMods = b;
                if (listingStandard.ButtonText(Helpers.TryTranslate("View unused settings", "XmlExtensions_ViewUnusedSettings")))
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
            modListListing.Label(Helpers.TryTranslate("Currently inactive mods:", "XmlExtensions_InactiveMods"));
            modListListing.GapLine(4);
            modListListing.Gap(2);
            Rect modRect = modListListing.GetRect(rect.height - 30f);
            Rect scrollRect = new Rect(0, 0, modRect.width - 20f, Math.Max(XmlMod.unusedMods.Count > 0 ? XmlMod.unusedMods.Count * 32 : 30, rect.height - 29f));
            Widgets.BeginScrollView(modRect, ref settingsPosition, scrollRect);
            Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
            Listing_Standard modListing = new Listing_Standard();
            modListing.Begin(rect2);
            if (XmlMod.unusedMods.Count == 0)
            {
                modListing.Label(Helpers.TryTranslate("No extra settings at the moment", "XmlExtensions_NoExtraSettings"));
            }
            foreach (string mod in XmlMod.unusedMods)
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
                keyListListing.Label(Helpers.TryTranslate("Currently selected mod's unused settings:", "XmlExtensions_SelectedModUnusedKeys"));
                keyListListing.GapLine(4);
                keyListListing.Gap(2);

                Rect keyRect = keyListListing.GetRect(rect.height - 30f - 36f);
                Rect scrollRect2 = new Rect(0, 0, keyRect.width - 20f, Math.Max(XmlMod.unusedSettings[selectedExtraMod].Count * 24f, rect.height - 29f - 36f));
                Widgets.BeginScrollView(keyRect, ref keyPosition, scrollRect2);
                rect2 = new Rect(0f, 0f, scrollRect2.width, 99999f);
                Listing_Standard keyListing = new Listing_Standard();
                keyListing.Begin(rect2);
                string tempStr = null;
                foreach (string key in XmlMod.unusedSettings[selectedExtraMod])
                {
                    bool del = false;
                    keyListing.CheckboxLabeled(key + ": " + SettingsManager.GetSetting(selectedExtraMod, key), ref del, Helpers.TryTranslate("Delete", "XmlExtensions_Delete"));
                    if (del)
                    {
                        SettingsManager.DeleteSetting(selectedExtraMod, key);
                        tempStr = key;
                    }
                }
                if (tempStr != null)
                {
                    XmlMod.unusedSettings[selectedExtraMod].Remove(tempStr);
                    if (XmlMod.unusedSettings[selectedExtraMod].Count == 0)
                    {
                        XmlMod.unusedMods.Remove(selectedExtraMod);
                        XmlMod.unusedSettings.Remove(selectedExtraMod);
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
                    if (buttonLeftListing.ButtonText(Helpers.TryTranslate("Delete {0} keys", "XmlExtensions_DeleteKeys").Replace("{0}", XmlMod.unusedSettings[selectedExtraMod].Count.ToString())))
                    {
                        Find.WindowStack.Add(new Dialog_MessageBox(Helpers.TryTranslate("Are you sure you want to delete every unused key of the current mod?", "XmlExtensions_ConfirmationResetMod"), "Yes".Translate(), delegate ()
                        {
                            foreach (string key in XmlMod.unusedSettings[selectedExtraMod])
                            {
                                SettingsManager.DeleteSetting(selectedExtraMod, key);
                            }
                            XmlMod.unusedMods.Remove(selectedExtraMod);
                            XmlMod.unusedSettings.Remove(selectedExtraMod);
                            selectedExtraMod = null;
                        }, "No".Translate(), null, null, false, null, null));
                    }
                    buttonLeftListing.End();

                    Rect secondRect = tempRect.RightPart(0.495f);
                    Listing_Standard buttonRightListing = new Listing_Standard();
                    buttonRightListing.Begin(secondRect);
                    GUI.color = Color.red;
                    int count = 0;
                    foreach (List<string> list in XmlMod.unusedSettings.Values)
                    {
                        count += list.Count;
                    }
                    if (buttonRightListing.ButtonText(Helpers.TryTranslate("Delete all {0} unused keys", "XmlExtensions_DeleteAllUnusedKeys").Replace("{0}", count.ToString()), null))
                    {
                        Find.WindowStack.Add(new Dialog_MessageBox(Helpers.TryTranslate("Are you sure you want to delete all unused keys from every mod?", "XmlExtensions_ConfirmResetAll"), "Yes".Translate(), delegate ()
                        {
                            foreach (string mod in XmlMod.unusedMods)
                            {
                                foreach (string key in XmlMod.unusedSettings[mod])
                                {
                                    SettingsManager.DeleteSetting(mod, key);
                                }
                            }
                            XmlMod.unusedSettings.Clear();
                            XmlMod.unusedMods.Clear();
                            selectedExtraMod = null;
                        }, "No".Translate(), null, null, false, null, null));
                    }
                    GUI.color = Color.white;
                    buttonRightListing.End();
                }
                keyListListing.End();
            }
        }

        public override void PreClose()
        {
            prevMod = SelectedMod;
            SetSelectedMod(null);
            LoadedModManager.GetMod(typeof(XmlMod)).WriteSettings();
            base.PreClose();
        }

        private void CacheFilter()
        {
            cachedFilteredList.Clear();
            foreach (ModContainer mod in loadedMods)
            {
                if (searchText == null || searchText == "" || mod.ToString().ToLower().Contains(searchText.ToLower()))
                {
                    cachedFilteredList.Add(mod);
                }
            }
        }

        public static void SetActiveMenu(string defName)
        {
            if (activeMenu != null)
            {
                XmlMod.menus[activeMenu].RunPostCloseActions();
            }
            if (defName != null)
            {
                XmlMod.menus[defName].RunPreOpenActions();
            }
            activeMenu = defName;
        }
    }
}