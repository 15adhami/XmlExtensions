using System.Collections.Generic;
using Verse;

namespace XmlExtensions.Action
{
    internal class FloatMenu : ActionContainer
    {
        public string key;
        public string defaultValue;
        public List<DropdownOption> options;

        internal class DropdownOption
        {
            public string label;
            public string value;
            public string tKey;
        }

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
            var newOptions = new List<FloatMenuOption>();
            foreach (DropdownOption option in options)
            {
                newOptions.Add(new FloatMenuOption(option.label.TranslateIfTKeyAvailable(option.tKey), () => SettingsManager.SetSetting(modId, key, option.value)));
            }
            Find.WindowStack.Add(new Verse.FloatMenu(newOptions));
            return true;
        }
    }
}