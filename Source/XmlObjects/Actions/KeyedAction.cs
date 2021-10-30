using System;

namespace XmlExtensions.Action
{
    /// <summary>
    /// Inherit from this class to embed C# code that gets called everytime a specified key is changed
    /// </summary>
    public abstract class KeyedAction : ActionContainer
    {
        /// <summary>
        /// The key that is tied to this KeyedAction
        /// </summary>
        public string key;

        /// <summary>
        /// Applies the keyed action
        /// </summary>
        /// <param name="oldValue">The previous value of the key</param>
        /// <param name="newValue">The new value of the key</param>
        /// <returns>false if there was an error, true otherwise</returns>
        public bool DoKeyedAction(string oldValue, string newValue)
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
        /// This method should not be called
        /// </summary>
        /// <returns></returns>
        protected sealed override bool ApplyAction()
        {
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