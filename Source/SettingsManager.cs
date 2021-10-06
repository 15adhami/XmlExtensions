using System.Collections.Generic;

namespace XmlExtensions
{
    public static class SettingsManager
    {
        /// <summary>
        /// Gets the defaultValue of a given setting.
        /// </summary>
        /// <param name="modId">The modId of your mod.</param>
        /// <param name="key">The key of the setting.</param>
        /// <returns>The defaultValue of the setting associated with the given key.</returns>
        public static string GetDefaultValue(string modId, string key)
        {
            return XmlMod.settingsPerMod[modId].defValues[key];
        }

        /// <summary>
        /// Gets the current value of a setting.
        /// </summary>
        /// <param name="modId">The modId of your mod.</param>
        /// <param name="key">The key of the setting you want to access.</param>
        /// <returns>The current value associated with the given key.</returns>
        public static string GetSetting(string modId, string key)
        {
            return XmlMod.allSettings.dataDict[modId + ";" + key];
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
            XmlMod.setSetting(modId, key, value);
        }

        /// <summary>
        /// Gets the defaultSpacing of the currently open <c>SettingsMenuDef</c>.
        /// </summary>
        /// <returns>The spacing of the current <c>SettingsMenuDef</c>.</returns>
		public static int GetDefaultSpacing()
        {
            return XmlMod.menus[XmlMod.activeMenu].defaultSpacing;
        }

        // TODO: Finalize
        public static IEnumerable<string> GetKeys(string modId)
        {
            return XmlMod.settingsPerMod[modId].keys;
        }
    }
}