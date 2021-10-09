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
        public string modId;
        public bool submenu = false;

        public bool Init()
        {
            if (modId == null)
            {
                PatchManager.errors.Add("XmlExtensions.SettingsMenuDef: <modId>=null");
                return false;
            }
            try
            {
                // TODO: Replace with label is null
                if(submenu)
                    SettingsManager.AddMod(modId);
                else
                    SettingsManager.AddMod(modId, label);
                Verse.Log.Message("t0");
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
                            PatchManager.errors.Add("XmlExtensions.SettingsMenuDef(" + defName + "): Error in initializing a setting at position=" + c.ToString());
                            return false;
                        }
                    }
                    catch
                    {
                        PatchManager.errors.Add("XmlExtensions.SettingsMenuDef(" + defName + "): Error in initializing a setting at position=" + c.ToString());
                        return false;
                    }
                }
                if(!submenu)
                {
                    XmlMod.settingsPerMod[modId].homeMenu = defName;                    
                }
                if (keyedActions != null)
                {
                    foreach (KeyedAction action in keyedActions)
                    {
                        XmlMod.AddKeyedAction(modId, action.key, action);
                    }
                }
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.SettingsMenuDef(" + defName + "): " + e.Message);
                return false;
            }
            return true;
        }

        public void PostOpen()
        {
            PatchManager.ClearErrors();
            foreach (SettingContainer setting in settings)
            {
                if (!setting.DoPostOpen(modId))
                {
                    PatchManager.AddError("Failed to run PostOpen() for modId=" + modId);
                    PatchManager.PrintErrors();
                }
            }
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

        public void PreClose()
        {
            PatchManager.ClearErrors();
            foreach(SettingContainer setting in settings)
            {
                if(!setting.DoPreClose(modId))
                {
                    PatchManager.AddError("Failed to run PreClose() for modId=" + modId);
                    PatchManager.PrintErrors();
                }
            }
        }
    }
}
