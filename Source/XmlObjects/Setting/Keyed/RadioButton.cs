﻿using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace XmlExtensions.Setting
{
    internal class RadioButton : KeyedSettingContainer
    { // TODO: Add actions to RadioButton
        public string value;
        public string tooltip;
        public string tKey;
        public string tKeyTip;
        public bool highlight = true;
        public Anchor anchor = Anchor.Middle;

        private Texture2D RadioButOnTex;
        private Texture2D RadioButOffTex;

        // Used if label is null
        public enum Anchor
        {
            Left,
            Right,
            Middle
        }

        protected override bool Init()
        {
            RadioButOnTex = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOn");
            RadioButOffTex = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOff");
            return true;
        }

        protected override float CalculateHeight(float width)
        {
            return 22;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            bool selected = SettingsManager.GetSetting(modId, key) == value;
            string resolvedLabel = Helpers.TryTranslate(label, tKey);
            string resolvedTooltip = Helpers.TryTranslate(tooltip, tKeyTip);

            if (resolvedLabel == null)
            {
                Rect buttonRect = new(inRect.x, inRect.y + inRect.height / 2f - 11f, 22f, 22f);

                // Handle tooltip on icon only
                if (!resolvedTooltip.NullOrEmpty() && Mouse.IsOver(buttonRect))
                {
                    TooltipHandler.TipRegion(buttonRect, resolvedTooltip);
                }

                // Anchor alignment
                float iconX = anchor switch
                {
                    Anchor.Left => inRect.x,
                    Anchor.Right => inRect.xMax - 22f,
                    _ => inRect.x + (inRect.width - 22f) / 2f,
                };
                buttonRect.x = iconX;

                bool clicked = Widgets.ButtonImage(buttonRect, selected ? RadioButOnTex : RadioButOffTex);
                if (clicked && !selected)
                {
                    SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                }

                if (clicked)
                {
                    SettingsManager.SetSetting(modId, key, value);
                }
            }
            else
            {
                if (DrawRadioButton(inRect, resolvedLabel, selected, highlight, resolvedTooltip))
                {
                    SettingsManager.SetSetting(modId, key, value);
                }
            }
        }

        private bool DrawRadioButton(Rect rect, string label, bool active, bool highlight, string tooltip = null, float? tooltipDelay = null)
        {
            if (highlight && Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }
            if (!tooltip.NullOrEmpty())
            {
                TipSignal tip = tooltipDelay.HasValue ? new TipSignal(tooltip, tooltipDelay.Value) : new TipSignal(tooltip);
                TooltipHandler.TipRegion(rect, tip);
            }

            TextAnchor prevAnchor = Verse.Text.Anchor;
            Verse.Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect, label);
            Verse.Text.Anchor = prevAnchor;

            bool clicked = Widgets.ButtonInvisible(rect);
            if (clicked && !active)
            {
                SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
            }

            Color prevColor = GUI.color;
            GUI.color = Color.white;
            GUI.DrawTexture(
                new Rect(rect.x + rect.width - 22f, rect.y + rect.height / 2f - 11f, 22f, 22f),
                active ? RadioButOnTex : RadioButOffTex
            );
            GUI.color = prevColor;

            return clicked;
        }
    }
}
