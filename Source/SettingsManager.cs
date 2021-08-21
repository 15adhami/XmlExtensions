using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlExtensions
{
    public static class SettingsManager
    {
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
    }
}
