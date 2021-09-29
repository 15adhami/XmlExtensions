using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class UseSetting : PatchOperationValue
    {
        protected string modId;
        protected string key;
        protected string brackets = "{}";
        protected string defaultValue;
        protected XmlContainer apply;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                string temp = "";
                if (!getValue(ref temp, xml))
                {
                    return false;
                }
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, key, temp, brackets);
                int errNum = 0;
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.UseSetting(key = " + key + "): Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.UseSetting(key=" + key + "): " + e.Message);
                return false;
            }
        }

        public override bool getVar(ref string var)
        {
            var = key;
            return true;
        }

        public override bool getValue(ref string val, XmlDocument xml)
        {
            try
            {
                if (key == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSetting(modId=" + modId + "): <key>=null");
                    return false;
                }
                if (defaultValue == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSetting(key=" + key + "): <defaultValue>=null");
                    return false;
                }
                if (modId == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSetting(key=" + key + "): <modId>=null");
                    return false;
                }
                XmlMod.loadedMod = this.modId;
                XmlMod.addXmlMod(this.modId);
                string value;
                bool didContain = XmlMod.allSettings.dataDict.TryGetValue(this.modId + ";" + this.key, out value);
                if (!didContain)
                {
                    value = defaultValue;
                    XmlMod.addSetting(this.modId, this.key, defaultValue);
                }
                if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(key))
                {
                    XmlMod.settingsPerMod[modId].defValues.Add(key, defaultValue);
                }
                if (!XmlMod.settingsPerMod[modId].keys.Contains(key))
                {
                    XmlMod.settingsPerMod[modId].keys.Add(key);
                }
                val = value;
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.UseSetting(key=" + key + "): " + e.Message);
                return false;
            }
        }
    }

}
