using System;

namespace XmlExtensions.Action
{
    /// <summary>
    /// Inherit from this class to embed C# code that gets called everytime a specified key is changed
    /// </summary>
    public abstract class KeyedActionContainer : ErrorHandler
    {
        /// <summary>
        /// If the Action is applied in a SettingsMenuDef, then this field will automatically be set to the correct modId
        /// </summary>
        public string modId;

        /// <summary>
        /// The key that is tied to this KeyedAction
        /// </summary>
        public string key;

        /// <summary>
        /// If you want to return a value to be used by the XML, set this field to that value
        /// </summary>
        public object output = null;

        private protected sealed override void SetException()
        {
        }

        /// <summary>
        /// Applies the keyed action
        /// </summary>
        /// <param name="oldValue">The previous value of the key</param>
        /// <param name="newValue">The new value of the key</param>
        /// <returns>false if there was an error, true otherwise</returns>
        internal bool DoKeyedAction(string oldValue, string newValue)
        {
            try
            {
                if (!ApplyKeyedAction(oldValue, newValue))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Error(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Implement a method that will be called when the associated key is changed
        /// </summary>
        /// <param name="oldValue">The previous value of the key</param>
        /// <param name="newValue">The current value of the key</param>
        /// <returns>Return true if successful, false is there was an error</returns>
        protected virtual bool ApplyKeyedAction(string oldValue, string newValue)
        {
            return true;
        }
    }
}