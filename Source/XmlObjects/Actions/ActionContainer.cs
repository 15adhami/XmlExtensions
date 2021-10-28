using System;

namespace XmlExtensions.Action
{
    public abstract class ActionContainer : ErrorHandler
    {
        public string modId;
        public object output = null;

        protected sealed override void SetException()
        {
        }

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