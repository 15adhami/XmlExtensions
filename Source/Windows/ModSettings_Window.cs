using RimWorld;
using System.Collections.Generic;
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
        public static ModContainer SelectedMod = null;

        // For KeyedActions
        private static Dictionary<string, string> oldValuesCache;

        public override Vector2 InitialSize => new Vector2(900f, 700f);

        public ModSettings_Window(SettingsMenuDef initialMenu = null, bool isXmlExtensions = false)
        {
            settingsPosition = new();
            soundAmbient = null;
            soundAppear = null;
            oldValuesCache = new();
            doCloseButton = true;
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
            doCloseX = true;
            closeOnAccept = false;

            SelectedMod = null;
            activeMenu = null;

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
            Rect rect = new Rect(0f, 0f, rightRect.width, rightRect.height - Window.CloseButSize.y);
            GUI.BeginGroup(rect);

            if (SelectedMod != null && activeMenu != null)
            {
                Rect scrollRect = new Rect(0, 0, rect.width - 20f, activeMenu.CalculateHeight(rect.width - 20f));
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
                SelectedMod = null;
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
                activeMenu.PreOpen();
            }
            else
            {
                activeMenu = null;
            }
        }
    }
}