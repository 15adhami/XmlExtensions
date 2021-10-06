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

        public virtual void DrawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            try
            {
                DrawSettingContents(listingStandard, selectedMod);
            }
            catch
            {
                GUI.color = Color.red;
                listingStandard.Label("Error drawing setting");
                errHeight = 22;
                GUI.color = Color.white;
            }
        }

        protected virtual void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
        }

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

        protected virtual bool SetDefaultValue(string modId)
        {
            return true;
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

        public int GetHeight(float width, string selectedMod)
        {
            return (errHeight < 0 ? CalcHeight(width, selectedMod) : errHeight);
        }

        protected virtual int CalcHeight(float width, string selectedMod)
        {
            return 0;
        }

        /// <summary>
        /// This method will be run exactly one time after the game finishes booting, it is used to initialize the setting.
        /// </summary>
        /// <returns>Returns <c>false</c> if there was an error, <c>true</c> otherwise.</returns>
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
        /// This method will be run exactly one time after the game finishes booting. You may run any initialization or pre-computations here.
        /// </summary>
        /// <returns>Return <c>false</c> if there was an error, <c>true</c> otherwise.</returns>
        protected virtual bool Init() { return true; }

        public bool DoPreClose(string selectedMod)
        {
            return PreClose(selectedMod);
        }

        protected virtual bool PreClose(string selectedMod)
        {
            return true;
        }

        protected bool DoPreCloseSetting(SettingContainer setting, string selectedMod)
        {
            if (!setting.DoPreClose(selectedMod))
            {
                ThrowError("Failed to run PreClose() for a setting");
                return false;
            }
            return true;
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

        protected bool InitializeSetting(SettingContainer setting)
        {
            if (!setting.Initialize())
            {
                ThrowError("Failed to initialize a setting");
                return false;
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