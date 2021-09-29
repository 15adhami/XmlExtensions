namespace XmlExtensions
{
    public static class SettingsManager
    {
        public static string GetDefaultValue(string modId, string key)
        {
            return XmlMod.settingsPerMod[modId].defValues[key];
        }

        public static string GetSetting(string modId, string key)
        {
            return XmlMod.allSettings.dataDict[modId + ";" + key];
        }

        public static bool TryGetSetting(string modId, string key, out string value)
        {
            string temp = "";
            bool b;
            b = XmlMod.allSettings.dataDict.TryGetValue(modId + ";" + key, out temp);
            value = temp;
            return b;
        }

        public static void SetSetting(string modId, string key, string value)
        {
            XmlMod.setSetting(modId, key, value);
        }

		public static int GetDefaultSpacing()
        {
			return XmlMod.menus[XmlMod.activeMenu].defaultSpacing;
		}
    }
}
