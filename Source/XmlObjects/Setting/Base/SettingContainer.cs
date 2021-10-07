using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public abstract class SettingContainer
    {
        protected int errHeight = -1;
        public string tag;

        private float cachedHeight = -1f;

        // Public methods

        /// <summary>
        /// This method will be run exactly one time after the game finishes booting, it is used to initialize the setting
        /// </summary>
        /// <returns>Returns <c>false</c> if there was an error, <c>true</c> otherwise</returns>
        public bool Initialize()
        {
            try
            {
                return Init();
            }
            catch (Exception e)
            {
                ThrowError(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Sets the defaultValue of this setting
        /// </summary>
        /// <param name="modId">The modId of the mod that is using this setting</param>
        /// <returns>Returns <c>false</c> if there was an error, <c>true</c> otherwise</returns>
        public bool DefaultValue(string modId)
        {
            try
            {
                return SetDefaultValue(modId);
            }
            catch (Exception e)
            {
                ThrowError(e.Message);
                return false;
            }
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
                cachedHeight = errHeight < 0 ? CalcHeight(width, selectedMod) : errHeight;
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
            catch
            {
                GUI.color = Color.red;
                Widgets.Label(inRect, "Error drawing setting: " + GetType().ToString().Split('.')[GetType().ToString().Split('.').Length - 1]);
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
            return PreClose(selectedMod);
        }

        // Methods to override

        /// <summary>
        /// This method will be run exactly one time after the game finishes booting. You may run any initialization or pre-computations here.
        /// </summary>
        /// <returns>Return <c>false</c> if there was an error, <c>true</c> otherwise.</returns>
        protected virtual bool Init()
        {
            return true;
        }

        /// <summary>
        /// Sets the defaultValue of this setting. You may skip this if your setting doesn't contain other settings, or doesn't require a special method
        /// </summary>
        /// <param name="modId">The modId of the mod that is using this setting</param>
        /// <returns>Returns <c>false</c> if there was an error, <c>true</c> otherwise</returns>
        protected virtual bool SetDefaultValue(string modId)
        {
            return true;
        }

        /// <summary>
        /// Returns the height of the setting for the current frame
        /// </sumary>
        /// <param name="width">The width of the column the setting is contaiend in</param>
        /// <param name="selectedMod">The modId of the active mod in the settings menu</param>
        /// <returns>The height of the setting, in pixels</returns>
        protected virtual float CalcHeight(float width, string selectedMod)
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

        protected bool DefaultValueSettingsList(string modId, List<SettingContainer> settings, string name)
        {
            if (settings != null)
            {
                int c = 0;
                foreach (SettingContainer setting in settings)
                {
                    c++;
                    if (!setting.DefaultValue(modId))
                    {
                        ThrowError(name, c);
                        return false;
                    }
                }
            }
            return true;
        }

        protected bool DefaultValueSettingsList(string modId, List<SettingContainer> settings)
        {
            if (settings != null)
            {
                int c = 0;
                foreach (SettingContainer setting in settings)
                {
                    c++;
                    if (!setting.DefaultValue(modId))
                    {
                        ThrowError(c);
                        return false;
                    }
                }
            }
            return true;
        }

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

        protected bool DoPreCloseSettingsList(string selectedMod, List<SettingContainer> settings)
        {
            if (settings != null)
            {
                int c = 0;
                foreach (SettingContainer setting in settings)
                {
                    c++;
                    if (!setting.DoPreClose(selectedMod))
                    {
                        ThrowError("Failed to run PreClose() for a setting at position=" + c.ToString());
                        return false;
                    }
                }
            }
            return true;
        }

        protected bool DoPreCloseSettingsList(string selectedMod, List<SettingContainer> settings, string name)
        {
            if (settings != null)
            {
                int c = 0;
                foreach (SettingContainer setting in settings)
                {
                    c++;
                    if (!setting.DoPreClose(selectedMod))
                    {
                        ThrowError("Failed to run PreClose() for a setting in <" + name + "> at position=" + c.ToString());
                        return false;
                    }
                }
            }
            return true;
        }

        protected bool InitializeSettingsList(List<SettingContainer> settings)
        {
            if (settings != null)
            {
                int c = 0;
                foreach (SettingContainer setting in settings)
                {
                    c++;
                    if (!setting.Initialize())
                    {
                        ThrowError(c);
                        return false;
                    }
                }
            }
            return true;
        }

        protected bool InitializeSettingsList(List<SettingContainer> settings, string name)
        {
            if (settings != null)
            {
                int c = 0;
                foreach (SettingContainer setting in settings)
                {
                    c++;
                    if (!setting.Initialize())
                    {
                        ThrowError(name, c);
                        return false;
                    }
                }
            }
            return true;
        }

        // Error handling

        protected void ThrowError(string msg = "Failed to initialize")
        {
            PatchManager.errors.Add(GetType().ToString() + ": " + msg);
        }

        protected void ThrowError(int errNum)
        {
            PatchManager.errors.Add(GetType().ToString() + ": Failed to initialize a setting at position=" + errNum.ToString());
        }

        protected void ThrowError(string name, int errNum)
        {
            PatchManager.errors.Add(GetType().ToString() + ": Failed to initialize a setting in <" + name + "> at position=" + errNum.ToString());
        }

       
    }
}