using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    /// <summary>
    /// Inherit from this class in order to create a new setting that manages a key.
    /// </summary>
    public abstract class KeyedSettingContainer : SettingContainer
    {
        
        /// <summary>
        /// The default value of the key
        /// </summary>
        public string defaultValue = null;
        private protected override void SetException()
        {
            CreateExceptions(key, "key", defaultValue, "defaultValue");
        }

        /// <summary>
        /// Automatically sets the default value of the key.
        /// </summary>
        /// <returns></returns>
        protected override bool SetDefaultValue()
        {
            if (key == null)
            {
                Error("<key> is null");
                return false;
            }
            if (defaultValue != null)
            {
                SettingsManager.SetDefaultValue(modId, key, defaultValue);
            }
            return base.SetDefaultValue();
        }
    }
}