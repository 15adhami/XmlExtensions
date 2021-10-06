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
        public Dictionary<string, SettingsMenuDef> menus;
        public string tKey;
        public string homeMenu;

        public XmlModSettings()
        {
            keys = new List<string>();
            defValues = new Dictionary<string, string>();
            menus = new Dictionary<string, SettingsMenuDef>();
        }
        public XmlModSettings(string modId)
        {
            keys = new List<string>();
            this.modId = modId;
        }

        public bool PreClose()
        {
            foreach(SettingsMenuDef menu in menus.Values)
            {
                if (!menu.PreClose())
                {
                    return false;
                }
            }
            return true;
        }
    }
}
