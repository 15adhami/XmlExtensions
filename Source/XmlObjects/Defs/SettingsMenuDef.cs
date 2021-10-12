using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using XmlExtensions.Action;
using XmlExtensions.Setting;

namespace XmlExtensions
{
    public class SettingsMenuDef : Def
    {
        public string tKey;
        public int defaultSpacing = 2;
        public List<SettingContainer> settings;
        public List<KeyedAction> keyedActions;
        public List<MenuAction> preOpenActions;
        public List<MenuAction> onFrameActions;
        public List<MenuAction> postCloseActions;
        public string modId;
        public bool submenu = false;

        public bool Init()
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
                if (!submenu)
                {
                    XmlMod.settingsPerMod[modId].homeMenu = defName;
                }
                if (keyedActions != null)
                {
                    foreach (KeyedAction action in keyedActions)
                    {
                        action.modId = modId;
                        XmlMod.AddKeyedAction(modId, action.key, action);
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

        public float CalculateHeight(float width, string selectedMod)
        {
            float h = 0;
            foreach (SettingContainer setting in settings)
            {
                h += setting.GetHeight(width, selectedMod);
            }
            return h;
        }

        public void DrawSettings(Rect rect)
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
    }
}