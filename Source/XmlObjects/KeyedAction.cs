using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlExtensions.Setting
{
    public class KeyedAction
    {
        public string key;

        public bool DoAction(string oldValue, string newValue)
        {
            try
            {
                return Action(oldValue, newValue);
            }
            catch (Exception e)
            {
                PatchManager.AddError(GetType().ToString() + ": " + e.Message);
            }
            return true;
        }

        protected virtual bool Action(string oldValue, string newValue)
        {
            return true;
        }
    }
}
