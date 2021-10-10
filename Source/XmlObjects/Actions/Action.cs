using System;

namespace XmlExtensions.Action
{
    public abstract class Action : ErrorHandler
    {
        public string modId;

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

        protected virtual bool ApplyAction()
        {
            return true;
        }
    }
}