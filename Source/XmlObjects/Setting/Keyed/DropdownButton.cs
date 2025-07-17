using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class DropdownOption
    {
        public string label;
        public string value;
    }

    internal class DropdownButton : KeyedSettingContainer
    { // TODO: Add dropdown actions
        public List<DropdownOption> options;
        public string tKey;

        protected override float CalculateHeight(float width)
        {
            return 30;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            if (Widgets.ButtonText(inRect, label != null ? Helpers.TryTranslate(label, tKey) : SettingsManager.GetSetting(modId, key)))
            {
                var newOptions = new List<FloatMenuOption>();
                foreach (DropdownOption option in options)
                {
                    newOptions.Add(new FloatMenuOption(option.label, () => SettingsManager.SetSetting(modId, key, option.value) ));
                }
                Find.WindowStack.Add(new FloatMenu(newOptions));
            }
        }
    }
}