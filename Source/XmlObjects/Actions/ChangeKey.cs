using System.Collections.Generic;
using XmlExtensions.Setting;

namespace XmlExtensions.Action
{
    internal class ChangeKey : ActionContainer
    { // TOD: Make sure this works for settings in EmbedMenu
        protected string key;
        protected string defaultValue;
        protected List<string> tags = null;

        protected override bool Init()
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
            
            return true;
        }

        protected override bool ApplyAction()
        {
            foreach (string tag in tags)
            {
                foreach (SettingContainer setting in menuDef.tagMap[tag])
                    setting.key=key;
            }
            return true;
        }
    }
}
