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
            if(defaultValue == null)
            {
                PatchManager.errors.Add("Error in XmlExtensions.UseSetting in <defaultValue>: No value given");
                return false;
            }
            if (key == null)
            {
                PatchManager.errors.Add("Error in XmlExtensions.UseSetting in <key>: No key given");
                return false;
            }
            if (modId == null)
            {
                PatchManager.errors.Add("Error in XmlExtensions.UseSetting in <modId>: No value given");
                return false;
            }
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
                PatchManager.errors.Add("XmlExtensions.UseSetting: Error in the operation at position=" + errNum.ToString());
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
            if (modId == null)
            {
                PatchManager.errors.Add("XmlExtensions.CreateSettings: <modId>=null");
                return false;
            }
            if (label == null)
            {
                PatchManager.errors.Add("XmlExtensions.CreateSettings: <label>=null");
                return false;
            }
            try
            {
                XmlMod.loadedMod = this.modId;
                XmlMod.addXmlMod(this.modId, label);
                XmlMod.settingsPerMod[modId].tKey = tKey;
                if (XmlMod.settingsPerMod[modId].defaultSpacing == 2)
                {
                    XmlMod.settingsPerMod[modId].defaultSpacing = this.defaultSpacing;
                }
                int c = 0;
                foreach (SettingContainer setting in this.settings)
                {
                    c++;
                    XmlMod.tryAddSettings(setting, this.modId);
                    if(!setting.setDefaultValue(modId))
                    {
                        PatchManager.errors.Add("XmlExtensions.CreateSettings: Error in initializing a setting at position=" + c.ToString());
                        return false;
                    }
                    setting.init();
                }
                XmlMod.loadedXmlMods.Sort(delegate (string id1, string id2) { return XmlMod.settingsPerMod[id1].label.CompareTo(XmlMod.settingsPerMod[id2].label); });
            }
            catch
            {
                PatchManager.errors.Add("XmlExtensions.CreateSettings: Error (<modId>="+modId+")");
                return false;
            }
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
            if (modId == null)
            {
                PatchManager.errors.Add("XmlExtensions.OptionalPatch: <modId>=null");
                return false;
            }
            if (modId == key)
            {
                PatchManager.errors.Add("XmlExtensions.OptionalPatch: <key>=null");
                return false;
            }
            if (defaultValue == null)
            {
                PatchManager.errors.Add("XmlExtensions.OptionalPatch: <defaultValue>=null");
                return false;
            }
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
                        PatchManager.errors.Add("XmlExtensions.OptionalPatch: Error in <caseTrue> in the operation at position=" + errNum.ToString());
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
                        PatchManager.errors.Add("XmlExtensions.OptionalPatch: Error in <caseFalse> in the operation at position=" + errNum.ToString());
                        return false;
                    }
                }
                return true;
            }
        }
    }

}
