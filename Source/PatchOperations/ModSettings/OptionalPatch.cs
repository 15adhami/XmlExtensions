using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{

    public class OptionalPatch : PatchOperationExtended
    {
        protected string key;
        protected string modId;
        protected string defaultValue;
        protected XmlContainer caseTrue;
        protected XmlContainer caseFalse;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                if (key == null)
                {
                    PatchManager.errors.Add("XmlExtensions.OptionalPatch: <key>=null");
                    return false;
                }
                if (modId == null)
                {
                    PatchManager.errors.Add("XmlExtensions.OptionalPatch(key=" + key + "): <modId>=null");
                    return false;
                }                
                if (defaultValue == null)
                {
                    PatchManager.errors.Add("XmlExtensions.OptionalPatch(key=" + key + "): <defaultValue>=null");
                    return false;
                }
                XmlMod.loadedMod = this.modId;
                XmlMod.addXmlMod(this.modId);
                string value = defaultValue;
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

                if (bool.Parse(value))
                {
                    if (this.caseTrue != null)
                    {
                        int errNum = 0;
                        if (!Helpers.runPatchesInXmlContainer(caseTrue, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.OptionalPatch(key=" + key + "): Error in <caseTrue> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    if (this.caseFalse != null)
                    {
                        int errNum = 0;
                        if (!Helpers.runPatchesInXmlContainer(caseFalse, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.OptionalPatch(key=" + key + "): Error in <caseFalse> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.OptionalPatch(modId=" + modId + ", key=" + key + "): " + e.Message);
                return false;
            }
        }
    }

}
