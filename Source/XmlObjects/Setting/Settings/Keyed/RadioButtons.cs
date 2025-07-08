using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class RadioButtons : KeyedSettingContainer
    {
        public List<RadioButton> buttons;
        public bool highlight = true;
        public int spacing = -1;

        public class RadioButton
        {
            public string label;
            public string value;
            public string tooltip;
            public string tKey;
            public string tKeyTip;
            public bool highlight = true;
        }

        protected override bool Init(string selectedMod)
        {
            foreach (RadioButton button in buttons)
            {
                if (button.tooltip == null)
                {
                    button.tooltip = "";
                }
                else
                {
                    button.highlight = true;
                }
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
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
                Rect rect = listingStandard.GetRect(22f);
                if (DrawRadioButton(rect, Helpers.TryTranslate(button.label, button.tKey), selected, button.highlight, Helpers.TryTranslate(button.tooltip, button.tKeyTip)))
                {
                    SettingsManager.SetSetting(selectedMod, key, button.value);
                }
                listingStandard.GetRect(GetDefaultSpacing());
            }
            listingStandard.End();
        }

        private bool DrawRadioButton(Rect rect, string label, bool active, bool highlight, string tooltip = null, float? tooltipDelay = null)
        {
            if (highlight && Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }
            if (!tooltip.NullOrEmpty())
            {
                TipSignal tip = (tooltipDelay.HasValue ? new TipSignal(tooltip, tooltipDelay.Value) : new TipSignal(tooltip));
                TooltipHandler.TipRegion(rect, tip);
            }
            bool result = Widgets.RadioButtonLabeled(rect, label, active);
            return result;
        }
    }
}