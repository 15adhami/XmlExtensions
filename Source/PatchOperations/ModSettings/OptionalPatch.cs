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

        protected override void SetException()
        {
            exceptionVals = new string[] { key, defaultValue };
            exceptionFields = new string[] { "key", "defaultValue" };
        }

        protected override bool Patch(XmlDocument xml)
        {
            if (key == null)
            {
                NullError("key");
                return false;
            }
            if (modId == null)
            {
                NullError("modId");
                return false;
            }
            if (defaultValue == null)
            {
                NullError("defaultValue");
                return false;
            }
            XmlMod.loadedMod = modId;
            XmlMod.addXmlMod(modId);
            string value = defaultValue;
            bool didContain = SettingsManager.TryGetSetting(modId, key, out value);
            if (!didContain)
            {
                value = defaultValue;
                XmlMod.addSetting(modId, key, defaultValue);
            }
            if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(key))
            {
                XmlMod.settingsPerMod[modId].defValues.Add(key, defaultValue);
            }
            if (!XmlMod.settingsPerMod[modId].keys.Contains(key))
            {
                XmlMod.settingsPerMod[modId].keys.Add(key);
            }
            return RunPatchesConditional(bool.Parse(value), caseTrue, caseFalse, xml);
        }
    }

}
