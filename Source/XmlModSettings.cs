using System.Collections.Generic;

namespace XmlExtensions
{
    // "Virtual" mod settings. Settings each mod has.
    // TODO: Every mod gets their own XML file
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
    }
}