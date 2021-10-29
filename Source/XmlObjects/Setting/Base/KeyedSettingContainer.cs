namespace XmlExtensions.Setting
{
    /// <summary>
    /// Inherit from this class in order to create a new setting that manages a key.
    /// </summary>
    public abstract class KeyedSettingContainer : SettingContainer
    {
        /// <summary>
        /// They key that this setting manages
        /// </summary>
        public string key = null;

        /// <summary>
        /// The label to print (optional)
        /// </summary>
        public string label = null;
        
        /// <summary>
        /// The default value of the key
        /// </summary>
        public string defaultValue = null;

        #pragma warning disable 1591
        protected internal override void SetException()
        {
            CreateExceptions(key, "key", defaultValue, "defaultValue");
        }
        #pragma warning restore 1591

        /// <summary>
        /// Automatically sets the default value of the key.
        /// </summary>
        /// <param name="selectedMod">The modId</param>
        /// <returns></returns>
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