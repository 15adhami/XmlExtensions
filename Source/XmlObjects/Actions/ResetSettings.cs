using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace XmlExtensions.Action
{
    internal class ResetSettings : ActionContainer
    {
        protected List<string> keys = null;
        protected List<string> values = null;

        protected override bool ApplyAction()
        {
            if (keys == null)
            {
                foreach (string key in SettingsManager.GetKeys(modId))
                    SettingsManager.SetSetting(modId, key, SettingsManager.GetDefaultValue(modId, key));
            }
            else if (values != null)
            {
                for (int i = 0; i < keys.Count; i++)
                    SettingsManager.SetSetting(modId, keys[i], values[i]);
            }
            else
            {
                foreach (string key in keys)
                    SettingsManager.SetSetting(modId, key, SettingsManager.GetDefaultValue(modId, key));
            }
            return true;
        }
    }
}
