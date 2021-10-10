namespace XmlExtensions.Setting
{
    public abstract class KeyedSettingContainer : SettingContainer
    {
        public string key = null;
        public string label = null;
        public string defaultValue = null;

        protected override void SetException()
        {
            CreateExceptions(key, "key", defaultValue, "defaultValue");
        }

        protected override bool SetDefaultValue(string selectedMod)
        {
            if (key == null)
            {
                Error("<key> is null");
                return false;
            }
            if (defaultValue != null)
            {
                SettingsManager.SetDefaultValue(selectedMod, key, defaultValue);
            }
            return true;
        }
    }
}