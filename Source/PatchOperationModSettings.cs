using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;
using Verse;
using XmlExtensions.Setting;
using HarmonyLib;
using System.Reflection;

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
                PatchManager.errors.Add("XmlExtensions.UseSetting: <defaultValue>=null");
                return false;
            }
            if (key == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSetting: <key>=null");
                return false;
            }
            if (modId == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSetting: <modId>=null");
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

    public class UseSettingExternal : PatchOperation
    {
        protected string ModSettingsClass;
        protected string field;
        protected string brackets = "{}";
        protected XmlContainer apply;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            
            if (ModSettingsClass == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal: <ModSettingsClass>=null");
                return false;
            }
            if (field == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal: <field>=null");
                return false;
            }
            if (apply == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal: <apply>=null");
                return false;
            }
            var bindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            FieldInfo fieldInfo;
            try
            {
                
                fieldInfo = GenTypes.GetTypeInAnyAssembly(ModSettingsClass).GetField(field, bindings);
            }
            catch
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal: Failed to get field");
                return false;
            }
            if(fieldInfo == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal: Failed to get field");
                return false;
            }
            object value = fieldInfo.GetValue(null);
            XmlContainer newContainer;
            newContainer = Helpers.substituteVariableXmlContainer(apply, field, value.ToString(), brackets);
            int errNum = 0;
            if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal: Error in the operation at position=" + errNum.ToString());
                return false;
            }
            return true;
        }
    }

    public class OptionalPatchExternal : PatchOperation
    {
        protected string field;
        protected string ModSettingsClass;
        protected XmlContainer caseTrue;
        protected XmlContainer caseFalse;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            if (ModSettingsClass == null)
            {
                PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal: <ModSettingsClass>=null");
                return false;
            }
            if (field == null)
            {
                PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal: <field>=null");
                return false;
            }
            var bindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            FieldInfo fieldInfo;
            try
            {

                fieldInfo = GenTypes.GetTypeInAnyAssembly(ModSettingsClass).GetField(field, bindings);
            }
            catch
            {
                PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal: Failed to get field");
                return false;
            }
            if (fieldInfo == null)
            {
                PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal: Failed to get field");
                return false;
            }
            object value = fieldInfo.GetValue(null);

            if ((bool)value)
            {
                if (this.caseTrue != null)
                {
                    int errNum = 0;
                    if (!Helpers.runPatchesInXmlContainer(caseTrue, xml, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal: Error in <caseTrue> in the operation at position=" + errNum.ToString());
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
                        PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal: Error in <caseFalse> in the operation at position=" + errNum.ToString());
                        return false;
                    }
                }
                return true;
            }
        }
    }

    public class UseSettingsExternal : PatchOperation
    {
        protected string ModSettingsClass;
        protected List<string> fields;
        protected string brackets = "{}";
        protected XmlContainer apply;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            if (ModSettingsClass == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingsExternal: <ModSettingsClass>=null");
                return false;
            }
            if (fields == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingsExternal: <fields>=null");
                return false;
            }
            if (apply == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingsExternal: <apply>=null");
                return false;
            }
            List<string> values = new List<string>();
            var bindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            FieldInfo fieldInfo;
            for (int i = 0; i < fields.Count; i++)
            {
                try
                {

                    fieldInfo = GenTypes.GetTypeInAnyAssembly(ModSettingsClass).GetField(fields[i], bindings);
                }
                catch
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettingsExternal: Failed to get field");
                    return false;
                }
                if (fieldInfo == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettingsExternal: Failed to get field");
                    return false;
                }
                object value = fieldInfo.GetValue(null);
                values.Add((string)value);
            }
            XmlContainer newContainer = Helpers.substituteVariablesXmlContainer(apply, fields, values, brackets);
            int errNum = 0;
            if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingsExternal: Error in the operation at position=" + errNum.ToString());
                return false;
            }
            return true;
        }
    }

    public class UseSettings : PatchOperation
    {
        protected string modId;
        protected List<string> keys;
        protected List<string> defaultValues;
        protected string brackets = "{}";
        protected XmlContainer apply;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            if (defaultValues == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettings: <defaultValues>=null");
                return false;
            }
            if (keys == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettings: <keys>=null");
                return false;
            }
            if (modId == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettings: <modId>=null");
                return false;
            }
            if (keys.Count > defaultValues.Count)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettings: There are more keys than defaultValues");
                return false;
            }
            else if (keys.Count < defaultValues.Count)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettings: There are more defaultValues than keys");
                return false;
            }
            XmlMod.loadedMod = this.modId;
            XmlMod.addXmlMod(this.modId);
            List<string> values = new List<string>();
            for(int i = 0; i < keys.Count; i++)
            {
                string value;
                bool didContain = XmlMod.tryGetSetting(modId, keys[i], out value);
                if (!didContain)
                {
                    value = defaultValues[i];
                    XmlMod.addSetting(modId, keys[i], defaultValues[i]);
                }
                if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(keys[i]))
                {
                    XmlMod.settingsPerMod[modId].defValues.Add(keys[i], defaultValues[i]);
                }
                if (!XmlMod.settingsPerMod[modId].keys.Contains(keys[i]))
                {
                    XmlMod.settingsPerMod[modId].keys.Add(keys[i]);
                }
                values.Add(value);
            }            
            XmlContainer newContainer = Helpers.substituteVariablesXmlContainer(this.apply, keys, values, this.brackets);
            int errNum = 0;
            if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
            {
                PatchManager.errors.Add("XmlExtensions.UseSettings: Error in the operation at position=" + errNum.ToString());
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
                PatchManager.errors.Add("XmlExtensions.CreateSettings(" + modId + "): <label>=null");
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
                    try
                    {
                        c++;
                        XmlMod.tryAddSettings(setting, this.modId);
                        if (!setting.setDefaultValue(modId))
                        {
                            PatchManager.errors.Add("XmlExtensions.CreateSettings(" + modId + "): Error in initializing a setting at position=" + c.ToString());
                            return false;
                        }
                        setting.init();
                    }
                    catch
                    {
                        PatchManager.errors.Add("XmlExtensions.CreateSettings(" + modId + "): Error in initializing a setting at position=" + c.ToString());
                        return false;
                    }
                }
                XmlMod.loadedXmlMods.Sort(delegate (string id1, string id2) {
                    if (XmlMod.settingsPerMod[id1].label != null && XmlMod.settingsPerMod[id2].label != null)
                        return XmlMod.settingsPerMod[id1].label.CompareTo(XmlMod.settingsPerMod[id2].label);
                    else
                        return 0;
                });
            }
            catch
            {
                PatchManager.errors.Add("XmlExtensions.CreateSettings(" + modId + "): Error");
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
