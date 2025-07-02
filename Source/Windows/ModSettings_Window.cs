using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using XmlExtensions.Action;

namespace XmlExtensions
{
    // Window that appears when you press More Mod Settings
    public class ModSettings_Window : Window
    {
        public static SettingsMenuDef activeMenu = null;

        public static Vector2 settingsPosition;
        public static Vector2 modListPosition;
        public static ModContainer SelectedMod = null;


        private bool pinned = false;
        private string searchText = "";
        private static Dictionary<string, string> oldValuesCache;
        private bool focusSearchBox = false;
        private readonly float ListWidth = 256f;

        private const float TopAreaHeight = 40f;

        private const float TopButtonHeight = 35f;

        private const float TopButtonWidth = 150f;

        public override Vector2 InitialSize => new Vector2(900f, 700f);

        private Texture2D pinTex;

        public ModSettings_Window(SettingsMenuDef initialMenu = null, bool isXmlExtensions = false)
        {
            soundAmbient = null;
            soundAppear = null;
            oldValuesCache = new();
            doCloseButton = true;
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
            doCloseX = true;
            closeOnAccept = false;

            // Close other mod dialgue windows
            ModSettings_Window xmlModDialogue;
            bool foundXmlModWindow = Find.WindowStack.TryGetWindow(out xmlModDialogue);
            if (foundXmlModWindow)
            {
                xmlModDialogue.soundClose = null;
                xmlModDialogue.Close(false);
            }
            Dialog_ModSettings modDialogue;
            bool foundModWindow = Find.WindowStack.TryGetWindow(out modDialogue);
            if (foundModWindow)
            {
                modDialogue.soundClose = null;
                modDialogue.Close(false);
            }

            // Set initial menu
            if (initialMenu != null)
            {
                SetSelectedMod(initialMenu);
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect rightRect = inRect.RightPartPixels(864f);
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0f, 0f, rightRect.width - 150f - 17f, 35f), SelectedMod != null ? SelectedMod.ToString() : "XML Extensions");
            Text.Font = GameFont.Small;
            //Rect rect = new Rect(0f, 40f, inRect.width, inRect.height - 40f - Window.CloseButSize.y);
            Rect rect = new Rect(0f, 0f, rightRect.width, rightRect.height - 40f);
            GUI.BeginGroup(rect);
            if (SelectedMod != null)
            {
                Rect scrollRect = new Rect(0, 0, rect.width - 20f, activeMenu.CalculateHeight(rect.width - 20f, SelectedMod.modId));
                Widgets.BeginScrollView(rect.BottomPartPixels(rect.height - 40), ref settingsPosition, scrollRect);
                Rect rect2 = new Rect(0f, 0f, scrollRect.width, 999999f);
                activeMenu.DrawSettings(rect2);
                if (activeMenu.onFrameActions != null)
                {
                    ErrorManager.ClearErrors();
                    foreach (ActionContainer action in activeMenu.onFrameActions)
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
                DrawXmlExtensionsSettings(rect.BottomPartPixels(rect.height - 40f));
            }
            GUI.EndGroup();
        }
        public void SetSelectedMod(SettingsMenuDef menu)
        {
            // Run KeyedActions
            if (SelectedMod != null && XmlMod.keyedActionListDict.ContainsKey(SelectedMod.modId))
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
            if (SelectedMod != null)
            {
                SelectedMod.WriteSettings();
            }
            oldValuesCache.Clear();
            if (menu != null)
            {
                // Cache values for next mod
                if (XmlMod.keyedActionListDict.ContainsKey(menu.modId))
                {
                    foreach (string key in XmlMod.keyedActionListDict[menu.modId].Keys)
                    {
                        oldValuesCache.Add(key, SettingsManager.GetSetting(menu.modId, key));
                    }
                }
                SetActiveMenu(menu.defName);
                SelectedMod = new ModContainer(menu.modId);
            }
            else
            {
                SetActiveMenu(null);
            }
        }

        public override void PreClose()
        {
            SetSelectedMod(null);
            LoadedModManager.GetMod(typeof(XmlMod)).WriteSettings();
            base.PreClose();
        }

        public static void SetActiveMenu(string defName)
        {
            if (activeMenu != null)
            {
                SettingsMenuDef tempMenu = activeMenu;
                activeMenu = null;
                tempMenu.RunPostCloseActions();
            }
            if (defName != null)
            {
                activeMenu = DefDatabase<SettingsMenuDef>.GetNamed(defName);
                activeMenu.RunPreOpenActions();
            }
        }

        private void DrawXmlExtensionsSettings(Rect rect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            bool b = XmlMod.allSettings.showSettingsButton;
            listingStandard.CheckboxLabeled("XmlExtensions_ShowSettingsButton".Translate(), ref b, "XmlExtensions_ShowSettingsButtonTip".Translate());
            if (b != XmlMod.allSettings.showSettingsButton)
            {
                XmlMod.allSettings.showSettingsButton = b;
            }
            XmlMod.allSettings.showSettingsButton = b;
            b = XmlMod.allSettings.mainButton;
            listingStandard.CheckboxLabeled("XmlExtensions_AddMainButton".Translate(), ref XmlMod.allSettings.mainButton, "XmlExtensions_AddMainButtonTip".Translate());
            if (b != XmlMod.allSettings.mainButton)
            {
                DefDatabase<MainButtonDef>.GetNamed("XmlExtensions_MainButton_ModSettings").buttonVisible = XmlMod.allSettings.mainButton;
            }
            Rect buttonRect = listingStandard.GetRect(30f);
            Listing_Standard listingStandard2 = new();
            listingStandard2.Begin(buttonRect);
            if (listingStandard2.ButtonText("XmlExtensions_ViewUnusedSettings".Translate()))
            {
                Find.WindowStack.Add(new UnusedSettings_Window());
            }
            listingStandard2.End();
            listingStandard.End();
        }
    }
}