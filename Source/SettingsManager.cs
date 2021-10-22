using System.Collections.Generic;

namespace XmlExtensions
{
    public static class SettingsManager
    {
        /// <summary>
        /// Gets the defaultValue of a given setting.
        /// </summary>
        /// <param name="modId">The modId of your mod</param>
        /// <param name="key">The key of the setting</param>
        /// <returns>The defaultValue of the setting associated with the given key</returns>
        public static string GetDefaultValue(string modId, string key)
        {
            return XmlMod.settingsPerMod[modId].defValues[key];
        }

        /// <summary>
        /// Sets the defaultValue of a setting to the value given
        /// </summary>
        /// <param name="modId">The modId of your mod</param>
        /// <param name="key">The key of the setting</param>
        /// <param name="value">The value you want to set as default</param>
        public static void SetDefaultValue(string modId, string key, string value)
        {
            RegisterKey(modId, key, value);
            XmlMod.settingsPerMod[modId].defValues[key] = value;
        }

        /// <summary>
        /// Gets the current value of a setting.
        /// </summary>
        /// <param name="modId">The modId of your mod.</param>
        /// <param name="key">The key of the setting you want to access.</param>
        /// <returns>The current value associated with the given key.</returns>
        public static string GetSetting(string modId, string key)
        {
            try
            {
                return XmlMod.allSettings.dataDict[modId + ";" + key];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Tries to retrieve the value of a given setting, and stores it into <c>value</c>.
        /// </summary>
        /// <param name="modId">The modId of your mod.</param>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">The reference of the variable you want to store the value in.</param>
        /// <returns>Whether or not a value was found for the key.</returns>
        public static bool TryGetSetting(string modId, string key, out string value)
        {
            string temp = "";
            bool b;
            b = XmlMod.allSettings.dataDict.TryGetValue(modId + ";" + key, out temp);
            value = temp;
            return b;
        }

        /// <summary>
        /// Sets the given value to the key. The settings will be saved via Scribe as soon as the user closes the "More Mod Settings" menu.
        /// </summary>
        /// <param name="modId">The modId of your mod.</param>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">The value you want to store.</param>
        public static void SetSetting(string modId, string key, string value)
        {
            RegisterKey(modId, key, value);
            XmlMod.allSettings.dataDict[modId + ";" + key] = value;
        }

        /// <summary>
        /// Determines whether or not the given mod has a setting associated with the given key
        /// </summary>
        /// <param name="modId">The <c>modId</c> of your mod</param>
        /// <param name="key">The key of the setting you want to check</param>
        /// <returns><c>true</c> if the mod contains the key, <c>false</c> otherwise</returns>
        public static bool ContainsKey(string modId, string key)
        {
            return XmlMod.settingsPerMod[modId].keys.Contains(key);
        }

        /// <summary>
        /// Iterator for all active keys associated with the given mod
        /// </summary>
        /// <param name="modId">The modId of the mod</param>
        /// <returns>An iterator for every active key associated with the given mod</returns>
        public static IEnumerable<string> GetKeys(string modId)
        {
            return XmlMod.settingsPerMod[modId].keys;
        }

        /// <summary>
        /// Deletes the setting and key for the given mod
        /// </summary>
        /// <param name="modId">The modId of the mod</param>
        /// <param name="key">The key you want to delete</param>
        public static void DeleteSetting(string modId, string key)
        {
            if (XmlMod.allSettings.dataDict.ContainsKey(modId + ';' + key))
            {
                XmlMod.allSettings.dataDict.Remove(modId + ';' + key);
            }
        }

        /// <summary>
        /// Registers the key-value pair into XML Extensions settings database
        /// </summary>
        /// <param name="modId">The modId of the mod</param>
        /// <param name="key">The key you want to delete</param>
        /// <param name="value">The potential value to register</param>
        public static void RegisterKey(string modId, string key, string value)
        {
            if (!XmlMod.allSettings.dataDict.ContainsKey(modId + ";" + key))
            {
                XmlMod.allSettings.dataDict.Add(modId + ";" + key, value);
            }
            if (!XmlMod.settingsPerMod[modId].keys.Contains(key))
            {
                XmlMod.settingsPerMod[modId].keys.Add(key);
            }
            if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(key))
            {
                XmlMod.settingsPerMod[modId].defValues.Add(key, value);
            }
        }

        /// <summary>
        /// Register a new mod into XML Extensions, only use if you know what you are doing
        /// </summary>
        /// <param name="modId">The modId of the new mod</param>
        /// <param name="label">The label of the new mod</param>
        public static void AddMod(string modId, string label)
        {
            AddMod(modId);
            XmlMod.settingsPerMod[modId].label = label;
        }

        /// <summary>
        /// Register a new mod into XML Extensions, only use if you know what you are doing
        /// </summary>
        /// <param name="modId">The modId of the new mod</param>
        public static void AddMod(string modId)
        {
            if (XmlMod.loadedXmlMods == null)
            {
                XmlMod.loadedXmlMods = new List<string>();
            }
            if (!XmlMod.loadedXmlMods.Contains(modId))
            {
                XmlMod.loadedXmlMods.Add(modId);
            }
            if (XmlMod.settingsPerMod == null)
            {
                XmlMod.settingsPerMod = new Dictionary<string, XmlModSettings>();
            }
            if (!XmlMod.settingsPerMod.ContainsKey(modId))
            {
                XmlModSettings t = new XmlModSettings(modId);
                XmlMod.settingsPerMod.Add(modId, t);
            }
            if (XmlMod.settingsPerMod[modId].defValues == null)
            {
                XmlMod.settingsPerMod[modId].defValues = new Dictionary<string, string>();
            }
        }
    }
}