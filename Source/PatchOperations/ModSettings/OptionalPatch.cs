using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class OptionalPatch : PatchOperationExtended
    {
        public string key;
        public string modId;
        public string defaultValue;
        public XmlContainer caseTrue;
        public XmlContainer caseFalse;

        protected override void SetException()
        {
            CreateExceptions(key, "key", defaultValue, "defaultValue");
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
            SettingsManager.AddMod(modId);
            SettingsManager.SetDefaultValue(modId, key, defaultValue);
            bool didContain = SettingsManager.TryGetSetting(modId, key, out string value);
            if (!didContain)
            {
                value = defaultValue;
                SettingsManager.SetSetting(modId, key, defaultValue);
            }
            return RunPatchesConditional(bool.Parse(value), caseTrue, caseFalse, xml);
        }
    }
}