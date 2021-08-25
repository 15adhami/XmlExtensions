﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlExtensions.Setting;

namespace XmlExtensions
{
    // "Virtual" mod settings. Settings each mod has.
    public class XmlModSettings
    {
        public List<string> keys;
        public List<SettingContainer> settings;
        public int defaultSpacing = 2;
        public string label;
        public string modId;
        public Dictionary<string, string> defValues;
        public string tKey;

        public XmlModSettings()
        {
            this.keys = new List<string>();
            this.settings = new List<SettingContainer>();
            defValues = new Dictionary<string, string>();
        }
        public XmlModSettings(string modId)
        {
            this.keys = new List<string>();
            this.settings = new List<SettingContainer>();
            this.modId = modId;
        }

        public int calculateHeight(float width, string selectedMod)
        {
            int h = 0;
            foreach (SettingContainer setting in settings)
            {
                h += setting.getHeight(width, selectedMod);
            }
            return h;
        }

        public void resetSettings()
        {
            foreach (string key in keys)
            {
                XmlMod.allSettings.dataDict[modId + ";" + key] = defValues[key];
            }
        }
    }
}
