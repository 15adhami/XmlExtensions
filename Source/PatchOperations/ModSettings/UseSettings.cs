using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class UseSettings : PatchOperationExtended
    {
        protected string modId;
        protected List<string> keys;
        protected List<string> defaultValues;
        protected string brackets = "{}";
        protected XmlContainer apply;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                if (defaultValues == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettings(modId=" + modId + "): <defaultValues>=null");
                    return false;
                }
                if (keys == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettings(modId=" + modId + "): <keys>=null");
                    return false;
                }
                if (modId == null)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettings: <modId>=null");
                    return false;
                }
                if (keys.Count > defaultValues.Count)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettings(modId=" + modId + "): There are more keys than defaultValues");
                    return false;
                }
                else if (keys.Count < defaultValues.Count)
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettings(modId=" + modId + "): There are more defaultValues than keys");
                    return false;
                }
                XmlMod.loadedMod = this.modId;
                XmlMod.addXmlMod(this.modId);
                List<string> values = new List<string>();
                for (int i = 0; i < keys.Count; i++)
                {
                    string value;
                    bool didContain = XmlMod.tryGetSetting(modId, keys[i], out value);
                    if (!didContain)
                    {
                        value = defaultValues[i];
                        XmlMod.addSetting(modId, keys[i], defaultValues[i]);
                    }
                    if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(keys[i]))
                    {
                        XmlMod.settingsPerMod[modId].defValues.Add(keys[i], defaultValues[i]);
                    }
                    if (!XmlMod.settingsPerMod[modId].keys.Contains(keys[i]))
                    {
                        XmlMod.settingsPerMod[modId].keys.Add(keys[i]);
                    }
                    values.Add(value);
                }
                XmlContainer newContainer = Helpers.substituteVariablesXmlContainer(this.apply, keys, values, this.brackets);
                int errNum = 0;
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.UseSettings(modId=" + modId + "): Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.UseSettings(modId=" + modId + "): " + e.Message);
                return false;
            }
        }
    }

}
