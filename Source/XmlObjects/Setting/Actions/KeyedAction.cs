using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlExtensions.Action
{
    public abstract class KeyedAction : Action
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
