using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using XmlExtensions.Action;

namespace XmlExtensions
{
    // Window that appears when you press More Mod Settings
    public abstract class BaseSettingsWindow : Window
    {
        public static SettingsMenuDef activeMenu = null;

        public static Vector2 settingsPosition;
        public static ModContainer SelectedMod = null;

        // For KeyedActions
        protected static Dictionary<string, string> oldValuesCache;

        public int shouldClose = 0;

        public BaseSettingsWindow(SettingsMenuDef initialMenu = null, bool isXmlExtensions = false)
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

            // Close other mod dialogue windows
            ModSettingsWindow xmlModDialogue;
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
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (shouldClose > 0)
                Close();
            if (shouldClose == 2)
                GenCommandLine.Restart();
        }

        public static void SetActiveMenu(string defName)
        {
            if (activeMenu != null)
            {
                SettingsMenuDef tempMenu = activeMenu;
                activeMenu = null;
                tempMenu.PostClose();
            }
            if (defName != null)
            {
                activeMenu = DefDatabase<SettingsMenuDef>.GetNamed(defName);
                activeMenu.PreOpen();
            }
            else
            {
                activeMenu = null;
            }
        }

        public void SetSelectedMod(ModContainer mod)
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
            if (SelectedMod != null)
            {
                SelectedMod.WriteSettings();
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
            
            SelectedMod = mod;
        }

        protected void DrawModSettings(Rect rect)
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
    }
}