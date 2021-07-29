using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlExtensions
{
    public class XmlModSettings
    {
        public List<string> keys;
        public List<SettingContainer> settings;
        public string label;
        public string modId;

        public XmlModSettings()
        {
            this.keys = new List<string>();
            this.settings = new List<SettingContainer>();
        }
        public XmlModSettings(string modId)
        {
            this.keys = new List<string>();
            this.settings = new List<SettingContainer>();
            this.modId = modId;
        }
    }
}
