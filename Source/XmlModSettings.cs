using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using XmlExtensions.Setting;

namespace XmlExtensions
{
    // "Virtual" mod settings. Settings each mod has.
    public class XmlModSettings
    {
        public List<string> keys;
        public string label;
        public string modId;
        public Dictionary<string, string> defValues;
        public string tKey;
        public string homeMenu;

        public XmlModSettings()
        {
            keys = new List<string>();
            defValues = new Dictionary<string, string>();
        }
        public XmlModSettings(string modId)
        {
            this.keys = new List<string>();
            this.modId = modId;
        }

        public int calculateHeight(float width, string selectedMod)
        {
            int h = 0;
            foreach (SettingContainer setting in XmlMod.menus[XmlMod.activeMenu].settings)
            {
                h += setting.GetHeight(width, selectedMod);
            }
            return h;
        }

        public void resetSettings()
        {
            foreach (string key in keys)
            {
                XmlMod.allSettings.dataDict[modId + ";" + key] = defValues[key];
            }
        }
    }
}
