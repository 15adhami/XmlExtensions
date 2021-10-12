namespace XmlExtensions.Action
{
    public class SetKeyValue : MenuAction
    {
        public string key;
        public string value;

        protected override bool ApplyAction()
        {
            if (!SettingsManager.ContainsKey(modId, key))
            {
                Error("Failed to find a setting with the given key");
            }
            SettingsManager.SetSetting(modId, key, value);
            return true;
        }
    }
}