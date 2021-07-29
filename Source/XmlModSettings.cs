using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlExtensions
{
    public class XmlModSettings
    {
        public List<string> stringKeys;

        public string label;

        public string modId;
        public XmlModSettings()
        {
            this.stringKeys = new List<string>();
        }
        public XmlModSettings(string modId)
        {
            this.stringKeys = new List<string>();
            this.modId = modId;
        }
    }
}
