using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace XmlExtensions.Setting
{
    internal class RadioButtons : KeyedSettingContainer
    {
        public List<RadioButton> buttons;
        public bool highlight = true;
        public float spacing = -1f;
        public float height = -1f;
        public Style style = Style.RadioButton;

        public enum Style
        {
            RadioButton,
            UIButton,
            OptionButton,
            MainButton
        }

        public class RadioButton
        {
            public string label;
            public string value;
            public string tooltip;
            public string tKey;
            public string tKeyTip;
            public bool highlight = true;
        }

        protected override bool Init()
        {
            if (style != Style.RadioButton && height < 0f)
            {
                height = 30f;
            }
            else if (style == Style.RadioButton)
            {
                height = 22f;
            }
            if (spacing < 0f)
            {
                if (style == Style.OptionButton || style == Style.MainButton)
                {
                    spacing = 4f;
                }
                else
                {
                    spacing = 2f;
                }
            }
            addDefaultSpacing = false;
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

        protected override float CalculateHeight(float width)
        {
            float verticalSpacing = (spacing < 0 ? (addDefaultSpacing ? GetDefaultSpacing() : 0) : spacing);
            float settingHeight = buttons.Count * verticalSpacing + buttons.Count * height;
            return style == Style.OptionButton ? settingHeight + verticalSpacing : settingHeight;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            float verticalSpacing = (spacing < 0 ? (addDefaultSpacing ? GetDefaultSpacing() : 0) : spacing);
            if (style == Style.OptionButton)
            { // Add extra space for this style
                listingStandard.GetRect(verticalSpacing);
            }
            foreach (RadioButton button in buttons)
            {
                bool selected = SettingsManager.GetSetting(modId, key) == button.value;
                Rect rect = listingStandard.GetRect(height);
                if (DrawRadioButton(rect, Helpers.TryTranslate(button.label, button.tKey), selected, highlight, Helpers.TryTranslate(button.tooltip, button.tKeyTip)))
                {
                    SettingsManager.SetSetting(modId, key, button.value);
                }
                listingStandard.GetRect(verticalSpacing);
            }
            listingStandard.End();
        }

        private bool DrawRadioButton(Rect rect, string label, bool active, bool highlight, string tooltip = null, float? tooltipDelay = null)
        {
            if (!tooltip.NullOrEmpty())
            {
                TipSignal tip = (tooltipDelay.HasValue ? new TipSignal(tooltip, tooltipDelay.Value) : new TipSignal(tooltip));
                TooltipHandler.TipRegion(rect, tip);
            }
            bool clicked = false;
            if (style == Style.RadioButton)
            {
                if (highlight && Mouse.IsOver(rect))
                {
                    Widgets.DrawHighlight(rect);
                }
                clicked = Widgets.RadioButtonLabeled(rect, label, active);
            }
            else if (style == Style.UIButton)
            {
                if (!active)
                {
                    clicked = Widgets.ButtonText(rect, label);
                    if (clicked)
                    {
                        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                    }
                }
                else
                {
                    Widgets.DrawAtlas(rect, Widgets.ButtonBGAtlasClick);
                    Verse.Text.Anchor = TextAnchor.MiddleCenter;
                    Color tempColor = GUI.color;
                    GUI.color = Color.white;
                    bool wordWrap = Verse.Text.WordWrap;
                    if (rect.height < Verse.Text.LineHeight * 2f)
                    {
                        Verse.Text.WordWrap = false;
                    }
                    Widgets.Label(rect, label);
                    Verse.Text.WordWrap = wordWrap;
                    GUI.color = tempColor;
                    Verse.Text.Anchor = TextAnchor.UpperLeft;
                    clicked = Widgets.ButtonInvisible(rect);
                }
            }
            else if (style == Style.OptionButton)
            {
                Widgets.DrawOptionBackground(rect.MiddlePartPixels(rect.width - 8f, rect.height), active);
                Verse.Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect, label);
                Verse.Text.Anchor = TextAnchor.UpperLeft;
                clicked = Widgets.ButtonInvisible(rect);
                if (clicked && !active)
                {
                    SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                }
            }
            else if (style == Style.MainButton)
            {
                if (!active)
                {
                    clicked = Widgets.ButtonTextSubtle(rect, null, 0f, -1f, SoundDefOf.Mouseover_Category);
                    if (clicked)
                    {
                        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                    }
                }
                else
                {
                    Color colorTemp = GUI.color;
                    GUI.color = GenUI.MouseoverColor;
                    Widgets.DrawAtlas(rect, Widgets.ButtonSubtleAtlas);
                    GUI.color = Color.grey;
                    Widgets.DrawBox(rect, 2);
                    GUI.color = colorTemp;
                }
                Rect labelRect = rect;
                if (Mouse.IsOver(rect) || active)
                {
                    labelRect.x += 2f;
                    labelRect.y -= 2f;
                }
                Verse.Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(labelRect, label);
                Verse.Text.Anchor = TextAnchor.UpperLeft;

            }
            return clicked;
        }
    }
}