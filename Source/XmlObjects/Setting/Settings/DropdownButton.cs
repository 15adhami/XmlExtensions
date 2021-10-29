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
    {
        public List<DropdownOption> options;
        public string tKey;

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 30 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            if (Widgets.ButtonText(inRect, label != null ? Helpers.TryTranslate(label, tKey) : SettingsManager.GetSetting(selectedMod, key)))
            {
                var newOptions = new List<FloatMenuOption>();
                foreach (DropdownOption option in options)
                {
                    newOptions.Add(new FloatMenuOption(option.label, () => SettingsManager.SetSetting(selectedMod, key, option.value) ));
                }
                Find.WindowStack.Add(new FloatMenu(newOptions));
            }
        }
    }
}