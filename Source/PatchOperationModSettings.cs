﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;
using Verse;
using XmlExtensions.Setting;

namespace XmlExtensions
{
    public class UseSetting : PatchOperation
    {
        protected string modId;
        protected string key;
        protected string brackets = "{}";
        protected string defaultValue;
        protected XmlContainer apply;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlMod.loadedMod = this.modId;
            XmlMod.addXmlMod(this.modId, this.key);
            string value;
            bool didContain = XmlMod.allSettings.dataDict.TryGetValue(this.modId + "." + this.key, out value);
            XmlContainer newContainer;
            if (!didContain)
            {
                value = defaultValue;
                XmlMod.addSetting(this.modId, this.key, defaultValue);
            }
            if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(key))
            {
                XmlMod.settingsPerMod[modId].defValues.Add(key, defaultValue);
            }
            if (!XmlMod.settingsPerMod[modId].keys.Contains(key))
            {
                XmlMod.settingsPerMod[modId].keys.Add(key);
            }
            newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.key, value, this.brackets);
            Helpers.runPatchesInXmlContainer(newContainer, xml);
            return true;
        }
    }

    public class CreateSettings : PatchOperation
    {
        protected string modId;
        protected string label;
        protected int defaultSpacing = 2;
        protected List<SettingContainer> settings;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlMod.loadedMod = this.modId;
            XmlMod.addXmlMod(this.modId, this.label);
            if (XmlMod.settingsPerMod[modId].defaultSpacing == 2)
            {
                XmlMod.settingsPerMod[modId].defaultSpacing = defaultSpacing;
            }            
            foreach (SettingContainer setting in this.settings)
            {
                XmlMod.tryAddSettings(setting, this.modId);
                if (setting.GetType().IsSubclassOf(typeof(KeyedSettingContainer)))
                {
                    if (!XmlMod.settingsPerMod[modId].keys.Contains(((KeyedSettingContainer)(setting)).key))
                    {
                        XmlMod.settingsPerMod[modId].keys.Add(((KeyedSettingContainer)(setting)).key);
                    }
                    if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(((KeyedSettingContainer)(setting)).key))
                    {
                        XmlMod.settingsPerMod[modId].defValues.Add(((KeyedSettingContainer)(setting)).key, ((KeyedSettingContainer)(setting)).defaultValue);
                    }
                } 
            }
            
            return true;
        }
    }    

}
