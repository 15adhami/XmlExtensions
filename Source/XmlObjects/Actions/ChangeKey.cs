using System.Collections.Generic;

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
                menuDef.tagSettingDict[tag].key=key;
            }
            return true;
        }
    }
}
