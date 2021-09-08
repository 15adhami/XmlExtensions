using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using XmlExtensions.Setting;

namespace XmlExtensions
{
    public class SettingsMenuDef : Def
    {
        public string tKey;
        public int defaultSpacing = 2;
        public List<SettingContainer> settings;
        public string modId;

        public bool ApplyWorker()
        {
            if (modId == null)
            {
                PatchManager.errors.Add("XmlExtensions.CreateSettings: <modId>=null");
                return false;
            }
            if (label == null)
            {
                PatchManager.errors.Add("XmlExtensions.CreateSettings(" + modId + "): <label>=null");
                return false;
            }
            try
            {
                XmlMod.loadedMod = modId;
                XmlMod.addXmlMod(modId, label);
                XmlMod.settingsPerMod[modId].tKey = tKey;
                if (XmlMod.settingsPerMod[modId].defaultSpacing == 2)
                {
                    XmlMod.settingsPerMod[modId].defaultSpacing = this.defaultSpacing;
                }
                int c = 0;
                foreach (SettingContainer setting in this.settings)
                {
                    try
                    {
                        c++;
                        XmlMod.tryAddSettings(setting, modId);
                        if (!setting.setDefaultValue(modId))
                        {
                            PatchManager.errors.Add("XmlExtensions.CreateSettings(" + modId + "): Error in initializing a setting at position=" + c.ToString());
                            return false;
                        }
                        setting.init();
                    }
                    catch
                    {
                        PatchManager.errors.Add("XmlExtensions.CreateSettings(" + modId + "): Error in initializing a setting at position=" + c.ToString());
                        return false;
                    }
                }
                XmlMod.loadedXmlMods.Sort(delegate (string id1, string id2) {
                    if (XmlMod.settingsPerMod[id1].label != null && XmlMod.settingsPerMod[id2].label != null)
                        return XmlMod.settingsPerMod[id1].label.CompareTo(XmlMod.settingsPerMod[id2].label);
                    else
                        return 0;
                });
            }
            catch
            {
                PatchManager.errors.Add("XmlExtensions.CreateSettings(" + modId + "): Error");
                return false;
            }
            return true;
        }
    }
}
