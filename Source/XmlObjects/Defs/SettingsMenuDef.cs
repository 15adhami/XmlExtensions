using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using XmlExtensions.Setting;

namespace XmlExtensions
{
    public class SettingsMenuDef : Def
    {
        public string tKey;
        public int defaultSpacing = 2;
        public List<SettingContainer> settings;
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
                if(submenu)
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
            }
            catch
            {
                PatchManager.errors.Add("XmlExtensions.SettingsMenuDef(" + defName + "): Error");
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

        public void DrawSettings(Listing_Standard listingStandard)
        {
            listingStandard.verticalSpacing = defaultSpacing;
            float width = listingStandard.ColumnWidth;
            foreach (SettingContainer setting in settings)
            {
                setting.DrawSetting(listingStandard.GetRect(setting.GetHeight(width, modId)), modId);
            }
        }

        public void PreClose()
        {
            PatchManager.ClearErrors();
            foreach(SettingContainer setting in settings)
            {
                if(!setting.DoPreClose(modId))
                {
                    PatchManager.errors.Add("Failed to run PreClose() for modId=" + modId);
                    PatchManager.PrintErrors();
                }
            }
        }
    }
}
