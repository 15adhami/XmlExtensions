using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    /// <summary>
    /// Inherit from this class in order to create a new setting.
    /// </summary>
    public abstract class SettingContainer : Container
    {
        // Public fields

        /// <summary>
        /// The key that this setting manages (if needed)
        /// </summary>
        public string key = null;

        public string label = null;
        public string tKey = null;
        public string tKeyTip = null;
        public string tooltip = null;

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

        //protected float translateY = 0;

        /// <summary>
        /// Set to true to display the dimensions of the setting
        /// </summary>
        protected bool showDimensions = false;

        /// <summary>
        /// Used to help reference the setting via xpath
        /// </summary>
        protected string tag;

        // Fields for search
        protected bool allowSearch = true;
        protected SearchType? searchType = null;
        protected string searchTag = null;

        protected enum SearchType
        {
            SearchAllAndHighlight,
            SearchDrawnAndHighlight,
            SearchAll,
            SearchDrawn,
        }

        protected internal Dictionary<IEnumerable<Container>, bool> containedFiltered = [];
        protected internal Dictionary<IEnumerable<Container>, bool> prevContainedFiltered = [];

        // For caching text fields
        protected internal string cachedText;

        // Private fields
        private bool needsDraw = false;
        private float cachedHeight = -1f;
        private int errHeight = -1;

        // Public methods

        /// <summary>
        /// This method will be run exactly one time after the game finishes booting and after running <c>DefaultValue()</c>, it is used to initialize the setting
        /// </summary>
        /// <returns>Returns <c>false</c> if there was an error, <c>true</c> otherwise</returns>
        internal override bool Initialize(SettingsMenuDef menuDef)
        {
            if (!initialized)
            {
                try
                {
                    this.menuDef ??= menuDef;
                    modId = menuDef.modId;
                    menuDef.settingFilterDict.Add(this, false);
                    menuDef.prevSettingFilterDict.Add(this, false);
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
                bool flag = base.Initialize(menuDef);
                searchType ??= initializedContainerLists.Count > 0 ? SearchType.SearchAll : SearchType.SearchAllAndHighlight;
                return flag;
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
        /// Returns the cached height of the setting
        /// </summary>
        public float GetHeight()
        {
            if (errHeight > 0)
            {
                return errHeight;
            }
            return cachedHeight;
        }

        /// <summary>
        /// Draw the setting in the given <c>Rect</c>. The height is equal to <c>cachedHeight</c>.
        /// </summary>
        /// <param name="inRect">The <c>Rect</c> that the setting will be drawn in</param>
        public virtual void DrawSetting(Rect inRect, bool isVisible = true) // TODO: Implement optimization based on visibility
        {
            if (isVisible || needsDraw)
            {
                try
                {
                    // Reset filter
                    foreach (var key in containedFiltered.Keys.ToList())
                        containedFiltered[key] = false;

                    if (errHeight > 0)
                    {
                        GUI.color = Color.red;
                        Widgets.Label(inRect, "Error drawing setting: " + GetType().ToString().Split('.')[GetType().ToString().Split('.').Length - 1]);
                        errHeight = 22;
                        GUI.color = Color.white;
                    }
                    else
                    {
                        // Filter drawn setting
                        if (allowSearch && !menuDef.searchText.NullOrEmpty())
                        {
                            Filter();
                        }

                        // Apply padding and translate
                        float topPad = padAbove > 0 ? padAbove : 0f;
                        float bottomPad = padBelow > 0 ? padBelow : 0f;
                        float leftPad = padLeft > 0 ? padLeft : 0f;
                        float rightPad = padRight > 0 ? padRight : 0f;
                        float spacing = addDefaultSpacing ? GetDefaultSpacing() : 0f;

                        inRect.x += translateX;
                        //inRect.y += translateY; TODO: implement translateY

                        Rect drawRect = new(
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
                            if (color != null)
                            {
                                Color originalColor = GUI.color;
                                GUI.color = (Color)color;
                                DrawSettingContents(drawRect);
                                GUI.color = originalColor;
                            }
                            else
                            {
                                DrawSettingContents(drawRect);
                            }
                        }

                        // Draw filter box
                        DoFilterBox(drawRect);
                    }
                }
                catch
                {
                    GUI.color = Color.red;
                    Widgets.Label(inRect, "Error drawing setting: " + GetType().ToString().Split('.')[GetType().ToString().Split('.').Length - 1]);
                    errHeight = 22;
                    GUI.color = Color.white;
                }
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
            if (tag != null)
            {
                menuDef.AddTag(tag, this);
            }
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
        /// Draw the setting in the given <c>Rect</c>.
        /// </summary>
        /// <param name="inRect">The <c>Rect</c> that the setting will be drawn in</param>
        protected abstract void DrawSettingContents(Rect inRect);

        /// <summary>
        /// Override to implement a custom DrawFilterBox method.
        /// </summary>
        /// <param name="inRect">The <c>Rect</c> that the filter box will be drawn in</param>
        protected virtual void DrawFilterBox(Rect inRect)
        {
            FilterBox(inRect);
        }

        // Helpers

        /// <summary>
        /// Gets the defaultSpacing of the currently open <c>SettingsMenuDef</c>.
        /// </summary>
        /// <returns>The spacing of the current <c>SettingsMenuDef</c>.</returns>
        protected int GetDefaultSpacing()
        {
            if (BaseSettingsWindow.activeMenu != null)
            {
                return BaseSettingsWindow.activeMenu.defaultSpacing;
            }
            return 0;
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
                    if (menuDef.settingFilterDict[setting])
                    {
                        menuDef.settingFilterDict[this] = true;
                        containedFiltered[settings] = true;
                    }
                }
                listing.End();
                initializedContainerLists[settings] = true;
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

        /// <summary>
        /// Draws an animated box in the given Rect.
        /// </summary>
        /// <param name="inRect">Rect to draw animated box.</param>
        protected void FilterBox(Rect inRect)
        {
            Color originalColor = GUI.color;
            Color c = menuDef.highlightColor;
            if (menuDef.animateHighlight)
            {
                float t = (float)menuDef.ticksOpen / 60f;
                float factor = 0.75f + 0.25f * Mathf.Cos(t * Mathf.PI * 0.2f);
                c.a = Mathf.Clamp01(c.a * factor);
            }
            GUI.color = c;
            Widgets.DrawBox(inRect.ContractedBy(1f));
            GUI.color = originalColor;
        }

        protected override bool InitializeContainers(IEnumerable<Container> containers, string name = null)
        {
            if (containers != null)
            {
                containedFiltered.Add(containers, false);
                prevContainedFiltered.Add(containers, false);
            }
            return base.InitializeContainers(containers, name);
        }

        // Internal helpers

        protected sealed override internal bool FilterSetting()
        {
            bool flag = false;
            if (!menuDef.searchText.NullOrEmpty() && allowSearch)
            {
                if (!menuDef.settingFilterDict[this])
                {
                    flag = Filter();
                }
                else
                {
                    flag = true;
                }
                if (searchType == SearchType.SearchAll || searchType == SearchType.SearchAllAndHighlight)
                {
                    foreach (IEnumerable<Container> containers in initializedContainerLists.Keys)
                    {
                        if (!initializedContainerLists[containers] && FilterSettings(containers))
                        {
                            flag = true;
                            containedFiltered[containers] = true;
                            menuDef.settingFilterDict[this] = true;
                        }
                    }
                }
                foreach (IEnumerable<Container> containers in initializedContainerLists.Keys.ToList())
                {
                    initializedContainerLists[containers] = false;
                }
                foreach (var key in containedFiltered.Keys)
                {
                    prevContainedFiltered[key] = containedFiltered[key];
                }
                /*
                foreach (var key in containedFiltered.Keys.ToList())
                {
                    containedFiltered[key] = false;
                }*/
            }

            return flag;
        }

        private bool FilterSettings(IEnumerable<Container> containers)
        {
            bool flag = false;
            if (containers != null)
            {
                foreach (Container container in containers)
                {
                    if (container.FilterSetting())
                    {
                        flag = true;
                    }
                }
            }
            return flag;
        }

        private void DoFilterBox(Rect inRect)
        {
            if ((searchType == SearchType.SearchDrawnAndHighlight || searchType == SearchType.SearchAllAndHighlight) && menuDef.prevSettingFilterDict[this] && allowSearch && !menuDef.searchText.NullOrEmpty())
            {
                DrawFilterBox(inRect);
            }
        }

        private bool Filter()
        {
            bool flag = false;
            if (label != null && menuDef.searchLabels && Helpers.TryTranslate(label, tKey).ToLower().Contains(menuDef.prevSearchText.ToLower()))
            {
                flag = true;
                menuDef.settingFilterDict[this] = true;
                menuDef.foundResults += 1;
            }
            else if (tooltip != null && menuDef.searchToolTips && Helpers.TryTranslate(tooltip, tKeyTip).ToLower().Contains(menuDef.prevSearchText.ToLower()))
            {
                flag = true;
                menuDef.settingFilterDict[this] = true;
                menuDef.foundResults += 1;
            }
            else if (cachedText != null && menuDef.searchTexts && cachedText.ToLower().Contains(menuDef.prevSearchText.ToLower()))
            {
                flag = true;
                menuDef.settingFilterDict[this] = true;
                menuDef.foundResults += 1;
            }
            else if (searchTag != null && searchTag.ToLower().Contains(menuDef.prevSearchText.ToLower()))
            {
                flag = true;
                menuDef.settingFilterDict[this] = true;
                menuDef.foundResults += 1;
            }
            return flag;
        }
    }
}