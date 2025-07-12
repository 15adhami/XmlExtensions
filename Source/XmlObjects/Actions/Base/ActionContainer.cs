using System;
using XmlExtensions.Setting;

namespace XmlExtensions.Action
{
    /// <summary>
    /// Inherit from this class in order to embed C# code into XML
    /// </summary>
    public abstract class ActionContainer : Container
    {
        /// <summary>
        /// If you want to return a value to be used by the XML, set this field to that value
        /// </summary>
        public object output = null;

        private protected sealed override void SetException() { }

        /// <summary>
        /// Runs the ApplyAction() method of the ActionContainer
        /// </summary>
        /// <returns>true if it succeeded, false if there was an error</returns>
        public bool DoAction()
        {
            try
            {
                if (!ApplyAction())
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
        /// The method you should implement
        /// </summary>
        /// <returns>Return false if there was an error, true otherwise</returns>
        protected virtual bool ApplyAction()
        {
            return true;
        }
    }
}