using System;
using Verse;

namespace XmlExtensions
{
    public class ModContainer : IComparable
    {
        public string modId;
        public Mod mod;

        public ModContainer(Mod mod)
        {
            this.mod = mod;
        }

        public ModContainer(string modId)
        {
            this.modId = modId;
        }

        public void WriteSettings()
        {
            if (IsXmlMod())
            {
            }
            else
            {
                mod.WriteSettings();
            }
        }

        public bool IsXmlMod()
        {
            return modId != null;
        }

        public override string ToString()
        {
            if (IsXmlMod())
            {
                return XmlMod.settingsPerMod[modId].label.TranslateIfTKeyAvailable(XmlMod.settingsPerMod[modId].tKey);
            }
            else
            {
                return mod.SettingsCategory();
            }
        }

        public int CompareTo(object obj)
        {
            return ToString().CompareTo(((ModContainer)obj).ToString());
        }
    }
}