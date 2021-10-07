using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class RadioButtons : KeyedSettingContainer
    {
        public List<RadioButton> buttons;
        protected int spacing = -1;

        protected override float CalcHeight(float width, string selectedMod)
        {
            return buttons.Count * ((spacing < 0 ? GetDefaultSpacing() : spacing) + 22);
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.verticalSpacing = spacing < 0 ? GetDefaultSpacing() : spacing;
            foreach (RadioButton button in buttons)
            {
                bool selected = SettingsManager.GetSetting(selectedMod, key) == button.value;
                if (listingStandard.RadioButton(Helpers.TryTranslate(button.label, button.tKey), selected, 0, Helpers.TryTranslate(button.tooltip, button.tKeyTip)))
                {
                    SettingsManager.SetSetting(selectedMod, key, button.value);
                }
            }
            listingStandard.End();
        }

        public class RadioButton
        {
            public string label;
            public string value;
            public string tooltip;
            public string tKey;
            public string tKeyTip;
        }
    }
}