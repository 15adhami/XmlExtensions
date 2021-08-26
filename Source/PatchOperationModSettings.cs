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
            XmlMod.addXmlMod(this.modId);
            string value;
            bool didContain = XmlMod.allSettings.dataDict.TryGetValue(this.modId + ";" + this.key, out value);
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
            int errNum = 0;
            if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
            {
                PatchManager.errors.Add("Error in XmlExtensions.UseSetting at operation index: " + errNum.ToString());
                return false;
            }
            return true;
        }
    }

    public class CreateSettings : PatchOperation
    {
        protected string modId;
        protected string label;
        protected int defaultSpacing = 2;
        protected List<SettingContainer> settings;
        protected string tKey;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlMod.loadedMod = this.modId;
            XmlMod.addXmlMod(this.modId, label);
            XmlMod.settingsPerMod[modId].tKey = tKey;
            if (XmlMod.settingsPerMod[modId].defaultSpacing == 2)
            {
                XmlMod.settingsPerMod[modId].defaultSpacing = this.defaultSpacing;
            }            
            foreach (SettingContainer setting in this.settings)
            {
                XmlMod.tryAddSettings(setting, this.modId);
                setting.setDefaultValue(modId);
                setting.init();
            }
            XmlMod.loadedXmlMods.Sort(delegate (string id1, string id2) { return XmlMod.settingsPerMod[id1].label.CompareTo(XmlMod.settingsPerMod[id2].label); });
            return true;
        }
    }

    public class OptionalPatch : PatchOperation
    {
        protected string key;
        protected string modId;
        protected string defaultValue;
        protected XmlContainer caseTrue;
        protected XmlContainer caseFalse;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlMod.loadedMod = this.modId;
            XmlMod.addXmlMod(this.modId);
            string value = defaultValue;
            bool didContain = XmlMod.allSettings.dataDict.TryGetValue(this.modId + ";" + this.key, out value);
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

            if (bool.Parse(value))
            {
                if (this.caseTrue != null)
                {
                    int errNum = 0;
                    if (!Helpers.runPatchesInXmlContainer(caseTrue, xml, ref errNum))
                    {
                        PatchManager.errors.Add("Error in XmlExtensions.OptionalPatch in caseTrue, operation index: " + errNum.ToString());
                        return false;
                    }
                }
                return true;
            }
            else
            {
                if (this.caseFalse != null)
                {
                    int errNum = 0;
                    if (!Helpers.runPatchesInXmlContainer(caseFalse, xml, ref errNum))
                    {
                        PatchManager.errors.Add("Error in XmlExtensions.OptionalPatch in caseFalse, operation index: " + errNum.ToString());
                        return false;
                    }
                }
                return true;
            }
        }
    }

}
