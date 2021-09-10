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
    public class UseSetting : PatchOperationValue
    {
        protected string modId;
        protected string key;
        protected string brackets = "{}";
        protected string defaultValue;
        protected XmlContainer apply;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                string temp = "";
                if (!getValue(ref temp, xml))
                {
                    return false;
                }
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, key, temp, brackets);
                int errNum = 0;
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.UseSetting(key = " + key + "): Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.UseSetting(key=" + key + "): " + e.Message);
                return false;
            }
        }

        public override bool getVar(ref string var)
        {
            var = key;
            return true;
        }

        public override bool getValue(ref string val, XmlDocument xml)
        {
            try
            {
                if (key == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSetting(modId=" + modId + "): <key>=null");
                    return false;
                }
                if (defaultValue == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSetting(key=" + key + "): <defaultValue>=null");
                    return false;
                }
                if (modId == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSetting(key=" + key + "): <modId>=null");
                    return false;
                }
                XmlMod.loadedMod = this.modId;
                XmlMod.addXmlMod(this.modId);
                string value;
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
                val = value;
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.UseSetting(key=" + key + "): " + e.Message);
                return false;
            }
        }
    }

    public class UseSettingExternal : PatchOperationValue
    {
        protected string ModSettingsClass;
        protected string field;
        protected string brackets = "{}";
        protected XmlContainer apply;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                string temp = "";
                if (!getValue(ref temp, xml))
                {
                    return false;
                }
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, field, temp, brackets);
                int errNum = 0;
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettingExternal(field=" + field + "): Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal(ModSettingsClass=" + ModSettingsClass + ", field=" + field + "): " + e.Message);
                return false;
            }
        }

        public override bool getVar(ref string var)
        {
            var = field;
            return true;
        }

        public override bool getValue(ref string val, XmlDocument xml)
        {
            if (field == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal(ModSettingsClass=" + ModSettingsClass + "): <field>=null");
                return false;
            }
            if (ModSettingsClass == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal(field=" + field + "): <ModSettingsClass>=null");
                return false;
            }
            if (apply == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal(field=" + field + "): <apply>=null");
                return false;
            }
            var bindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            FieldInfo fieldInfo = GenTypes.GetTypeInAnyAssembly(ModSettingsClass).GetField(field, bindings);
            if (fieldInfo == null)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingExternal(field=" + field + "): Failed to get field");
                return false;
            }
            val = fieldInfo.GetValue(null).ToString();
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
            try
            {
                if (field == null)
                {
                    PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal(ModSettingsClass=" + ModSettingsClass + "): <field>=null");
                    return false;
                }
                if (ModSettingsClass == null)
                {
                    PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal(field=" + field + "): <ModSettingsClass>=null");
                    return false;
                }                
                var bindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
                FieldInfo fieldInfo = GenTypes.GetTypeInAnyAssembly(ModSettingsClass).GetField(field, bindings);
                if (fieldInfo == null)
                {
                    PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal(field=" + field + "): Failed to get field");
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
                            PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal(field=" + field + "): Error in <caseTrue> in the operation at position=" + errNum.ToString());
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
                            PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal(field=" + field + "): Error in <caseFalse> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                    return true;
                }
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.OptionalPatchExternal(ModSettingsClass=" + ModSettingsClass + ", field=" + field + "): " + e.Message);
                return false;
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
            try
            {
                if (ModSettingsClass == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettingsExternal: <ModSettingsClass>=null");
                    return false;
                }
                if (fields == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettingsExternal(ModSettingsClass=" + ModSettingsClass + "): <fields>=null");
                    return false;
                }
                if (apply == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettingsExternal(ModSettingsClass=" + ModSettingsClass + "): <apply>=null");
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
                        PatchManager.errors.Add("XmlExtensions.UseSettingsExternal(ModSettingsClass=" + ModSettingsClass + "): Failed to get field");
                        return false;
                    }
                    if (fieldInfo == null)
                    {
                        PatchManager.errors.Add("XmlExtensions.UseSettingsExternal(ModSettingsClass=" + ModSettingsClass + "): Failed to get field");
                        return false;
                    }
                    object value = fieldInfo.GetValue(null);
                    values.Add((string)value);
                }
                XmlContainer newContainer = Helpers.substituteVariablesXmlContainer(apply, fields, values, brackets);
                int errNum = 0;
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettingsExternal(ModSettingsClass=" + ModSettingsClass + "): Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettingsExternal(ModSettingsClass=" + ModSettingsClass + "): " + e.Message);
                return false;
            }
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
            try
            {
                if (defaultValues == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettings(modId=" + modId + "): <defaultValues>=null");
                    return false;
                }
                if (keys == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettings(modId=" + modId + "): <keys>=null");
                    return false;
                }
                if (modId == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettings: <modId>=null");
                    return false;
                }
                if (keys.Count > defaultValues.Count)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettings(modId=" + modId + "): There are more keys than defaultValues");
                    return false;
                }
                else if (keys.Count < defaultValues.Count)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettings(modId=" + modId + "): There are more defaultValues than keys");
                    return false;
                }
                XmlMod.loadedMod = this.modId;
                XmlMod.addXmlMod(this.modId);
                List<string> values = new List<string>();
                for (int i = 0; i < keys.Count; i++)
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
                    PatchManager.errors.Add("XmlExtensions.UseSettings(modId=" + modId + "): Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettings(modId=" + modId + "): " + e.Message);
                return false;
            }
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
            }/*
            if (xml.SelectSingleNode("Defs/XmlExtensions.SettingsMenuDef[defName=\"" + modId + "\"]") == null)
            {
                SettingsMenuDef menuDef = new SettingsMenuDef();
                menuDef.defName = modId;
                menuDef.label = label;
                menuDef.defaultSpacing = defaultSpacing;
                menuDef.settings = settings;
                menuDef.tKey = tKey;
                xml.SelectSingleNode("Defs").AppendChild(menuDef.);
            }*/
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
            try
            {
                if (modId == key)
                {
                    PatchManager.errors.Add("XmlExtensions.OptionalPatch(modId=" + modId + "): <key>=null");
                    return false;
                }
                if (modId == null)
                {
                    PatchManager.errors.Add("XmlExtensions.OptionalPatch(key=" + key + "): <modId>=null");
                    return false;
                }                
                if (defaultValue == null)
                {
                    PatchManager.errors.Add("XmlExtensions.OptionalPatch(key=" + key + "): <defaultValue>=null");
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
                            PatchManager.errors.Add("XmlExtensions.OptionalPatch(key=" + key + "): Error in <caseTrue> in the operation at position=" + errNum.ToString());
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
                            PatchManager.errors.Add("XmlExtensions.OptionalPatch(key=" + key + "): Error in <caseFalse> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.OptionalPatch(modId=" + modId + ", key=" + key + "): " + e.Message);
                return false;
            }
        }
    }

}
