using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using XmlExtensions.Action;
using XmlExtensions.Setting;

namespace XmlExtensions
{
    /// <summary>
    /// A Def that defines a settings menu
    /// </summary>
    public class SettingsMenuDef : Def
    {
        /// <summary>
        /// Translation key for the label
        /// </summary>
        public string tKey;

        /// <summary>
        /// The spacing between each setting, in pixels
        /// </summary>
        public int defaultSpacing = 0;

        /// <summary>
        /// The list of settings to be displayed
        /// </summary>
        public List<SettingContainer> settings;

        /// <summary>
        /// The list of KeyedActions to run
        /// </summary>
        public List<KeyedAction> keyedActions;

        /// <summary>
        /// The list of ActionContainers that will be run when the menu is opened
        /// </summary>
        public List<ActionContainer> preOpenActions;

        /// <summary>
        /// The list of ActionContainers that will be run every frame
        /// </summary>
        public List<ActionContainer> onFrameActions;

        /// <summary>
        /// The list of ActionContainers that will be run when the menu is closed
        /// </summary>
        public List<ActionContainer> postCloseActions;

        /// <summary>
        /// The modId of the mod that defined this menu
        /// </summary>
        public string modId;

        /// <summary>
        /// Whether or not the menu is a submenu
        /// </summary>
        public bool submenu = false;

        internal bool Init()
        {
            if (label == null)
            {
                submenu = true;
            }
            if (modId == null)
            {
                ErrorManager.AddError("XmlExtensions.SettingsMenuDef: <modId>=null");
                return false;
            }
            try
            {
                if (submenu)
                    SettingsManager.AddMod(modId);
                else
                    SettingsManager.AddMod(modId, label);
                if (tKey != null)
                    XmlMod.settingsPerMod[modId].tKey = tKey;
                if (!submenu)
                {
                    XmlMod.settingsPerMod[modId].homeMenu = defName;
                }
                int c = 0;
                foreach (SettingContainer setting in settings)
                {
                    try
                    {
                        c++;
                        if (!setting.Initialize(modId))
                        {
                            ErrorManager.AddError("XmlExtensions.SettingsMenuDef(" + defName + "): Error in initializing a setting at position=" + c.ToString());
                            return false;
                        }
                    }
                    catch
                    {
                        ErrorManager.AddError("XmlExtensions.SettingsMenuDef(" + defName + "): Error in initializing a setting at position=" + c.ToString());
                        return false;
                    }
                }
                if (keyedActions != null)
                {
                    foreach (KeyedAction action in keyedActions)
                    {
                        action.modId = modId;
                        XmlMod.AddKeyedAction(modId, action.key, action);
                    }
                }
                if (preOpenActions != null)
                {
                    foreach (ActionContainer action in preOpenActions)
                    {
                        action.modId = modId;
                    }
                }
                if (postCloseActions != null)
                {
                    foreach (ActionContainer action in postCloseActions)
                    {
                        action.modId = modId;
                    }
                }
                if (onFrameActions != null)
                {
                    foreach (ActionContainer action in onFrameActions)
                    {
                        action.modId = modId;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorManager.AddError("XmlExtensions.SettingsMenuDef(" + defName + "): " + e.Message);
                return false;
            }
            return true;
        }

        internal float CalculateHeight(float width, string selectedMod)
        {
            float h = 0;
            foreach (SettingContainer setting in settings)
            {
                h += setting.GetHeight(width, selectedMod);
            }
            return h;
        }

        internal void DrawSettings(Rect rect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            listingStandard.verticalSpacing = defaultSpacing;
            float width = listingStandard.ColumnWidth;
            foreach (SettingContainer setting in settings)
            {
                setting.DrawSetting(listingStandard.GetRect(setting.GetHeight(width, modId)), modId);
            }
            listingStandard.End();
        }

        internal void RunPostCloseActions()
        {
            if (postCloseActions != null)
            {
                ErrorManager.ClearErrors();
                foreach (ActionContainer action in postCloseActions)
                {
                    if (!action.DoAction())
                    {
                        ErrorManager.PrintErrors();
                    }
                }
            }
        }

        internal void RunPreOpenActions()
        {
            if (preOpenActions != null)
            {
                ErrorManager.ClearErrors();
                foreach (ActionContainer action in preOpenActions)
                {
                    if (!action.DoAction())
                    {
                        ErrorManager.PrintErrors();
                    }
                }
            }
        }

        internal void PreOpenSettings()
        {
            if (settings != null)
            {
                ErrorManager.ClearErrors();
                int c = 0;
                foreach (SettingContainer setting in settings)
                {
                    try
                    {
                        c++;
                        if (!setting.PreOpen(modId))
                        {
                            ErrorManager.AddError("XmlExtensions.SettingsMenuDef(" + defName + "): Error in preopening a setting at position=" + c.ToString());
                            ErrorManager.PrintErrors();
                        }
                    }
                    catch
                    {
                        ErrorManager.AddError("XmlExtensions.SettingsMenuDef(" + defName + "): Error in preopening a setting at position=" + c.ToString());
                        ErrorManager.PrintErrors();
                    }
                }
            }
        }
    }
}