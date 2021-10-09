﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public abstract class SettingContainer
    {
        public string tag;

        private List<List<SettingContainer>> cachedLists = new List<List<SettingContainer>>();
        private Dictionary<List<SettingContainer>, string> listNameDict = new Dictionary<List<SettingContainer>, string>();
        private float cachedHeight = -1f;
        private int errHeight = -1;

        // Public methods

        /// <summary>
        /// This method will be run exactly one time after the game finishes booting and after running <c>DefaultValue()</c>, it is used to initialize the setting
        /// </summary>
        /// <returns>Returns <c>false</c> if there was an error, <c>true</c> otherwise</returns>
        public bool Initialize(string selectedMod)
        {
            try
            {
                if (!SetDefaultValue(selectedMod))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                ThrowError("Failed to set the default value:\n" + e.Message);
                return false;
            }
            try
            {
                return Init(selectedMod);
            }
            catch (Exception e)
            {
                ThrowError("Failed to initialize:\n" + e.Message);
                return false;
            }
        }

        public bool DoPostOpen(string selectedMod)
        {
            try
            {
                // Run this setting's PreClose() method
                if (!PostOpen(selectedMod))
                {
                    return false;
                }

                // Run all of the cached setting's PreClose() method
                foreach (List<SettingContainer> list in cachedLists)
                {
                    if (!DoPostOpenSettingsList(selectedMod, list, listNameDict[list]))
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                ThrowError("Failed to run PostOpen():\n" + e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns the height of the setting for the current frame
        /// </sumary>
        /// <param name="width">The width of the column the setting is contaiend in</param>
        /// <param name="selectedMod">The modId of the active mod in the settings menu</param>
        /// <returns>The height of the setting, in pixels</returns>
        public float GetHeight(float width, string selectedMod)
        {
            if (cachedHeight < 0)
            {
                cachedHeight = errHeight < 0 ? CalculateHeight(width, selectedMod) : errHeight;
            }
            return cachedHeight;
        }

        /// <summary>
        /// Draw the setting in the given <c>Rect</c>
        /// </summary>
        /// <param name="inRect">The <c>Rect</c> that the setting will be drawn in</param>
        /// <param name="selectedMod">The modId of the active mod in the settings menu</param>
        public virtual void DrawSetting(Rect inRect, string selectedMod)
        {
            try
            {
                DrawSettingContents(inRect, selectedMod);
            }
            catch (Exception e)
            {
                GUI.color = Color.red;
                Widgets.Label(inRect, "Error drawing setting: " + GetType().ToString().Split('.')[GetType().ToString().Split('.').Length - 1]);
                Verse.Log.Error(e.Message);
                errHeight = 22;
                GUI.color = Color.white;
            }
            cachedHeight = -1f;
        }

        /// <summary>
        /// Run the setting's <c>PreClose()</c> method
        /// </summary>
        /// <param name="selectedMod">The modId of the active mod in the settings menu</param>
        /// <returns>Returns <c>false</c> if there was an error, <c>true</c> otherwise</returns>
        public bool DoPreClose(string selectedMod)
        {
            try
            {
                // Run this setting's PreClose() method
                if (!PreClose(selectedMod))
                {
                    return false;                                                                                                                                                                                                           
                }

                // Run all of the cached setting's PreClose() method
                foreach (List<SettingContainer> list in cachedLists)
                {
                    if (!DoPreCloseSettingsList(selectedMod, list, listNameDict[list]))
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                ThrowError("Failed to run PreClose():\n" + e.Message);
                return false;
            }
            return true;
        }

        // Methods to override

        /// <summary>
        /// Sets the defaultValue of this setting, it is run immediately after the game finishes loading<br/>
        /// You may skip this if your setting doesn't contain other settings, or doesn't require a special method
        /// </summary>
        /// <param name="selectedMod"></param>
        /// <returns>Returns <c>false</c> if there was an error, <c>true</c> otherwise</returns>
        protected virtual bool SetDefaultValue(string selectedMod)
        {
            return true;
        }

        /// <summary>
        /// This method will be run exactly one time after the game finishes booting and after running <c>SetDefaultValue()</c><br/>
        /// You may run any initialization or pre-computation code here
        /// </summary>
        /// <returns>Return <c>false</c> if there was an error, <c>true</c> otherwise.</returns>
        protected virtual bool Init(string selectedMod)
        {
            return true;
        }

        /// <summary>
        /// This method will be run as soon as a mod's setting menu is opened<br/>
        /// It will be run everytime a menu is opened
        /// </summary>
        /// <param name="selectedMod">The modId of the mod whose menu was just selected</param>
        /// <returns>Return <c>false</c> if there was an error, <c>true</c> otherwise</returns>
        protected virtual bool PostOpen(string selectedMod)
        {
            return true;
        }

        /// <summary>
        /// Returns the height of the setting for the current frame
        /// </sumary>
        /// <param name="width">The width of the column the setting is contaiend in</param>
        /// <param name="selectedMod">The modId of the active mod in the settings menu</param>
        /// <returns>The height of the setting, in pixels</returns>
        protected virtual float CalculateHeight(float width, string selectedMod)
        {
            return 0;
        }

        /// <summary>
        /// Draw the setting in the given <c>Rect</c>
        /// </summary>
        /// <param name="inRect">The <c>Rect</c> that the setting will be drawn in</param>
        /// <param name="selectedMod">The modId of the active mod in the settings menu</param>
        protected abstract void DrawSettingContents(Rect inRect, string selectedMod);

        /// <summary>
        /// This method will be run when the menu containing this setting is closed
        /// </summary>
        /// <param name="selectedMod">The modId of the active mod in the settings menu</param>
        /// <returns>Returns <c>false</c> if there was an error, <c>true</c> otherwise</returns>
        protected virtual bool PreClose(string selectedMod)
        {
            return true;
        }

        // Helpers

        /// <summary>
        /// Gets the defaultSpacing of the currently open <c>SettingsMenuDef</c>.
        /// </summary>
        /// <returns>The spacing of the current <c>SettingsMenuDef</c>.</returns>
        protected int GetDefaultSpacing()
        {
            return XmlMod.menus[XmlMod.activeMenu].defaultSpacing;
        }

        /// <summary>
        /// Sets the currently displayed menu to the one given
        /// </summary>
        /// <param name="defName">the defName of the SettingsMenuDef you want to display</param>
        protected void SetActiveMenu(string defName)
        {
            XmlMod.activeMenu = defName;
        }

        /// <summary>
        /// Draws a list of SettingsContainer, error handling is done automatically
        /// </summary>
        /// <param name="rect">The <c>Rect</c> to draw in, should be the same height as the height of the list of settings</param>
        /// <param name="selectedMod">The <c>modId</c> of the selected mod in the menu</param>
        /// <param name="settings">The list of settings to draw</param>
        protected void DrawSettingsList(Rect rect, string selectedMod, List<SettingContainer> settings)
        {
            if (settings != null)
            {
                Listing_Standard listing = new Listing_Standard();
                listing.Begin(rect);
                foreach (SettingContainer setting in settings)
                {
                    setting.DrawSetting(listing.GetRect(setting.GetHeight(rect.width, selectedMod)), selectedMod);
                }
                listing.End();
            }
        }

        /// <summary>
        /// Calculates the total height of every setting in the list
        /// </summary>
        /// <param name="width">The width of the column the settings will be placed in</param>
        /// <param name="selectedMod">The modId of the active mod in the settings menu</param>
        /// <param name="settings">The list of settings</param>
        /// <returns>The total height of the settings</returns>
        protected float GetHeightSettingsList(float width, string selectedMod, List<SettingContainer> settings)
        {
            float h = 0;
            if (settings != null)
            {
                foreach (SettingContainer setting in settings)
                {
                    h += setting.GetHeight(width, selectedMod);
                }
            }
            return h;
        }

        /// <summary>
        /// Applies the <c>PreClose()</c> method on every setting in the list, error handling done automatically and prints the name of the list
        /// </summary>
        /// <param name="selectedMod">The modId of the active mod in the settings menu</param>
        /// <param name="settings">The list of settings</param>
        /// <param name="name">The name of the list (for error reporting purposes)</param>
        /// <returns>Returns <c>false</c> if there was an error, <c>true</c> otherwise</returns>
        protected bool DoPreCloseSettingsList(string selectedMod, List<SettingContainer> settings, string name = null)
        {
            if (settings != null)
            {
                int c = 0;
                foreach (SettingContainer setting in settings)
                {
                    c++;
                    if (!setting.DoPreClose(selectedMod))
                    { 
                        if (name != null)
                        {
                            ThrowError("Failed to run PreClose() for a setting in <" + name + "> at position=" + c.ToString());
                        }
                        else
                        {
                            ThrowError("Failed to run PreClose() for a setting at position=" + c.ToString());
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Applies the <c>Init()</c> method on every setting in the list, error handling done automatically<br/>If the name of the list is provided, it will be used for error reporting
        /// </summary>
        /// <param name="selectedMod">The modId of the active mod in the settings menu</param>
        /// <param name="settings">The list of settings</param>
        /// <param name="name">The name of the list (for error reporting purposes)</param>
        /// <returns>Returns <c>false</c> if there was an error, <c>true</c> otherwise</returns>
        protected bool InitializeSettingsList(string selectedMod, List<SettingContainer> settings, string name = null)
        {
            if (settings != null)
            {
                cachedLists.Add(settings);
                listNameDict.Add(settings, name);
                int c = 0;
                foreach (SettingContainer setting in settings)
                {
                    c++;
                    if (!setting.Initialize(selectedMod))
                    {
                        if (name != null)
                        {
                            ThrowError("Failed to initialize a setting in <" + name + "> at position=" + c.ToString());
                        }
                        else
                        {
                            ThrowError("Failed to initialize a setting at position = " + c.ToString());
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Runs the PostOpen() method on every setting in the list<br/>
        /// Error handling is done automatically
        /// </summary>
        /// <param name="selectedMod">The modId of the selected mod</param>
        /// <param name="settings">The list of settings</param>
        /// <param name="name">(optional) the name of the list of settings; used in error reporting</param>
        /// <returns>Returns <c>false</c> if there was an error</returns>
        protected bool DoPostOpenSettingsList(string selectedMod, List<SettingContainer> settings, string name = null)
        {
            if (settings != null)
            {
                int c = 0;
                foreach (SettingContainer setting in settings)
                {
                    c++;
                    if (!setting.DoPostOpen(selectedMod))
                    {
                        if (name != null)
                        {
                            ThrowError("Failed to run PostOpen() in a setting in <" + name + "> at position=" + c.ToString());
                        }
                        else
                        {
                            ThrowError("Failed to run PostOpen() in a setting at position = " + c.ToString());
                        }
                        return false;
                    }
                }
            }
            return true;
        }


        // Error handling

        protected void ThrowError(string msg = "Error")
        {
            PatchManager.errors.Add(GetType().ToString() + ": " + msg);
        }       
    }
}