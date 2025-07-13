using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    /// <summary>
    /// Inherit from this class in order to create a new setting.
    /// </summary>
    public abstract class SettingContainer : Container
    {// TODO: Add translateLeft parameters to SettingContainer

        /// <summary>
        /// Determines whether or not the default setting should be added after drawing the setting
        /// </summary>
        protected bool addDefaultSpacing = true;

        /// <summary>
        /// How many pixels to pad above the setting
        /// </summary>
        protected float padAbove = 0f;

        /// <summary>
        /// How many pixels to pad below the setting
        /// </summary>
        protected float padBelow = 0f;

        /// <summary>
        /// How many pixels to pad to the left of the setting
        /// </summary>
        protected float padLeft = 0f;

        /// <summary>
        /// How many pixels to pad to the right of the setting
        /// </summary>
        protected float padRight = 0f;

        protected float translateX = 0;

        protected float translateY = 0;

        /// <summary>
        /// Set to true to display the dimensions of the setting
        /// </summary>
        protected bool showDimensions = false;

        /// <summary>
        /// Used to help reference the setting via xpath
        /// </summary>
        protected string tag;

        private float cachedHeight = -1f;
        private int errHeight = -1;

        // Public methods

        /// <summary>
        /// This method will be run exactly one time after the game finishes booting and after running <c>DefaultValue()</c>, it is used to initialize the setting
        /// </summary>
        /// <returns>Returns <c>false</c> if there was an error, <c>true</c> otherwise</returns>
        public override bool Initialize(string modId)
        {
            if (!initialized)
            {
                try
                {
                    this.modId = modId;
                    if (!SetDefaultValue())
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Error("Failed to set default value:\n" + e.Message);
                    return false;
                }
                return base.Initialize(modId);
            }
            return true;
        }

        /// <summary>
        /// Returns the height of the setting for the current frame
        /// </summary>
        /// <param name="width">The width of the column the setting is contaiend in</param>
        /// <param name="selectedMod">The modId of the active mod in the settings menu</param>
        /// <returns>The height of the setting, in pixels</returns>
        public float GetHeight(float width)
        {
            try
            {
                if (errHeight > 0)
                {
                    return errHeight;
                }

                if (cachedHeight < 0)
                {
                    float effectiveWidth = width - (padLeft > 0 ? padLeft : 0f) - (padRight > 0 ? padRight : 0f);

                    cachedHeight = errHeight;
                    if (errHeight < 0)
                    {
                        cachedHeight = CalculateHeight(effectiveWidth);
                        cachedHeight += padAbove > 0 ? padAbove : 0f;
                        cachedHeight += padBelow > 0 ? padBelow : 0f;
                        cachedHeight += addDefaultSpacing ? GetDefaultSpacing() : 0f;
                    }
                }
                return cachedHeight;
            }
            catch
            {
                errHeight = 22;
                cachedHeight = errHeight;
                return errHeight;
            }
        }

        /// <summary>
        /// Draw the setting in the given <c>Rect</c>. The height is equal to <c>cachedHeight</c>.
        /// </summary>
        /// <param name="inRect">The <c>Rect</c> that the setting will be drawn in</param>
        public virtual void DrawSetting(Rect inRect)
        {
            try
            {
                if (errHeight > 0)
                {
                    GUI.color = Color.red;
                    Widgets.Label(inRect, "Error drawing setting: " + GetType().ToString().Split('.')[GetType().ToString().Split('.').Length - 1]);
                    errHeight = 22;
                    GUI.color = Color.white;
                }
                else
                {
                    float topPad = padAbove > 0 ? padAbove : 0f;
                    float bottomPad = padBelow > 0 ? padBelow : 0f;
                    float leftPad = padLeft > 0 ? padLeft : 0f;
                    float rightPad = padRight > 0 ? padRight : 0f;
                    float spacing = addDefaultSpacing ? GetDefaultSpacing() : 0f;

                    inRect.x += translateX;
                    inRect.y += translateY;

                    Rect drawRect = new Rect(
                        inRect.x + leftPad,
                        inRect.y + topPad,
                        inRect.width - leftPad - rightPad,
                        inRect.height - topPad - bottomPad - spacing
                    );

                    if (showDimensions)
                    {
                        Color originalColor = GUI.color;

                        // Outer full rectangle
                        GUI.color = Color.gray;
                        Widgets.DrawBox(inRect);

                        // Inner padded rectangle
                        GUI.color = Color.white;
                        Widgets.DrawBox(drawRect);

                        // Label dimensions inside drawRect
                        Widgets.Label(drawRect, $" {drawRect.width:0}x{drawRect.height:0}");

                        GUI.color = originalColor;
                    }
                    else
                    {
                        DrawSettingContents(drawRect);
                    } 
                }
            }
            catch
            {
                GUI.color = Color.red;
                Widgets.Label(inRect, "Error drawing setting: " + GetType().ToString().Split('.')[GetType().ToString().Split('.').Length - 1]);
                errHeight = 22;
                GUI.color = Color.white;
            }

            cachedHeight = -1f;
        }


        // Methods to override

        /// <summary>
        /// Sets the defaultValue of this setting, it is run immediately after the game finishes loading<br/>
        /// You may skip this if your setting doesn't contain other settings, or doesn't require a special method
        /// </summary>
        /// <param name="selectedMod"></param>
        /// <returns>Returns <c>false</c> if there was an error, <c>true</c> otherwise</returns>
        protected virtual bool SetDefaultValue()
        {
            return true;
        }

        /// <summary>
        /// Returns the height of the setting for the current frame
        /// </summary>
        /// <param name="width">The width of the column the setting is contaiend in</param>
        /// <returns>The height of the setting, in pixels</returns>
        protected virtual float CalculateHeight(float width)
        {
            return 0;
        }

        /// <summary>
        /// Draw the setting in the given <c>Rect</c>
        /// </summary>
        /// <param name="inRect">The <c>Rect</c> that the setting will be drawn in</param>
        protected abstract void DrawSettingContents(Rect inRect);

        // Helpers

        /// <summary>
        /// Gets the defaultSpacing of the currently open <c>SettingsMenuDef</c>.
        /// </summary>
        /// <returns>The spacing of the current <c>SettingsMenuDef</c>.</returns>
        protected int GetDefaultSpacing()
        {
            if (XmlExtensions_MenuModSettings.activeMenu != null)
            {
                return XmlExtensions_MenuModSettings.activeMenu.defaultSpacing;
            }
            else if (ModSettings_Window.activeMenu != null)
            {
                return ModSettings_Window.activeMenu.defaultSpacing;
            }
            return 0;
        }

        /// <summary>
        /// Sets the currently displayed menu to the one given
        /// </summary>
        /// <param name="defName">the defName of the SettingsMenuDef you want to display</param>
        protected void SetActiveMenu(string defName)
        {
            if (XmlExtensions_MenuModSettings.activeMenu != null)
            {
                XmlExtensions_MenuModSettings.SetActiveMenu(defName);
            }
            else if (ModSettings_Window.activeMenu != null)
            {
                ModSettings_Window.SetActiveMenu(defName);
            }
        }

        /// <summary>
        /// Draws a list of SettingsContainer, error handling is done automatically
        /// </summary>
        /// <param name="rect">The <c>Rect</c> to draw in, should be the same height as the height of the list of settings</param>
        /// <param name="settings">The list of settings to draw</param>
        protected void DrawSettingsList(Rect rect, List<SettingContainer> settings)
        {
            if (settings != null)
            {
                Listing_Standard listing = new Listing_Standard();
                listing.verticalSpacing = 0;
                listing.Begin(rect);
                foreach (SettingContainer setting in settings)
                {
                    setting.DrawSetting(listing.GetRect(setting.GetHeight(rect.width)));
                }
                listing.End();
            }
        }

        /// <summary>
        /// Calculates the total height of every setting in the list
        /// </summary>
        /// <param name="width">The width of the column the settings will be placed in</param>
        /// <param name="settings">The list of settings</param>
        /// <returns>The total height of the settings</returns>
        protected float CalculateHeightSettingsList(float width, List<SettingContainer> settings)
        {
            float h = 0;
            if (settings != null)
            {
                foreach (SettingContainer setting in settings)
                {
                    h += setting.GetHeight(width);
                }
            }
            return h;
        }
    }
}