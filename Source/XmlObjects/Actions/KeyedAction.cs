using System;

namespace XmlExtensions.Action
{
    public abstract class KeyedAction : MenuAction
    {
        public string key;

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

        protected sealed override bool ApplyAction()
        {
            return true;
        }

        protected virtual bool ApplyKeyedAction(string oldValue, string newValue)
        {
            return true;
        }
    }
}