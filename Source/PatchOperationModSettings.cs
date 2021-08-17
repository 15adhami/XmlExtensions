using System;
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
                XmlMod.settingsPerMod[modId].defaultSpacing = this.defaultSpacing;
            }            
            foreach (SettingContainer setting in this.settings)
            {
                XmlMod.tryAddSettings(setting, this.modId);
                trySetDefaultValue(setting);
            }
            
            return true;
        }

        private void trySetDefaultValue(SettingContainer setting)
        {
            if (setting.GetType().IsSubclassOf(typeof(KeyedSettingContainer)))
            {
                if (!XmlMod.settingsPerMod[modId].keys.Contains(((KeyedSettingContainer)(setting)).key))
                {
                    XmlMod.settingsPerMod[modId].keys.Add(((KeyedSettingContainer)(setting)).key);
                }
                if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(((KeyedSettingContainer)(setting)).key))
                {
                    if (((KeyedSettingContainer)(setting)).defaultValue != null)
                    {
                        XmlMod.settingsPerMod[modId].defValues.Add(((KeyedSettingContainer)(setting)).key, ((KeyedSettingContainer)(setting)).defaultValue);
                        if (!XmlMod.allSettings.dataDict.ContainsKey(modId + ";" + ((KeyedSettingContainer)(setting)).key))
                            XmlMod.allSettings.dataDict.Add(modId + ";" + ((KeyedSettingContainer)(setting)).key, ((KeyedSettingContainer)(setting)).defaultValue);
                    }
                    else
                    {// TODO: Make a check after game boots up
                        //Log.Error("[XML Extensions] " + modId + "." + ((KeyedSettingContainer)(setting)).key + " has no default value defined.");
                    }
                }
            }
            else if (setting.GetType().Equals(typeof(SplitColumn)))
            {
                Log.Message("split");
                foreach (SettingContainer colSetting in ((SplitColumn)(setting)).leftCol)
                {
                    trySetDefaultValue(colSetting);
                }
                foreach (SettingContainer colSetting in ((SplitColumn)(setting)).rightCol)
                {
                    trySetDefaultValue(colSetting);
                }
            }
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
            XmlMod.addXmlMod(this.modId, this.key);
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
                    if (!Helpers.runPatchesInXmlContainer(caseTrue, xml))
                        return false;
                }
                return true;
            }
            else
            {
                if(this.caseFalse != null)
                {
                    if (!Helpers.runPatchesInXmlContainer(caseFalse, xml))
                        return false;
                }
                return true;
            }
        }
    }

}
