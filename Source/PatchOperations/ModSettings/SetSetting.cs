using System.Xml;

namespace XmlExtensions
{
    internal class SetSetting : PatchOperationExtended
    {
        public string modId;
        public string key;
        public string value;
        public bool createKey = false;

        protected override bool Patch(XmlDocument xml)
        {
            if (!SettingsManager.ContainsKey(modId, key) && !createKey)
            {
                Error("Failed to find a setting with the given key");
                return false;
            }
            SettingsManager.SetSetting(modId, key, value);
            return true;
        }
    }
}