using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace XmlExtensions.Setting
{
    internal class RadioButton : KeyedSettingContainer
    { // TODO: Add actions to RadioButton
        public string value;
        public bool highlight = true;
        public float height = -1f;
        public Anchor anchor = Anchor.Middle;
        public Style style = Style.RadioButton;

        public enum Style
        {
            RadioButton,
            UIButton,
            OptionButton,
            MainButton
        }

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
            if (style != Style.RadioButton && height < 0f)
            {
                height = 30f;
            }
            else if (style == Style.RadioButton)
            {
                height = 22f;
            }
            RadioButOnTex = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOn");
            RadioButOffTex = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOff");
            return true;
        }

        protected override float CalculateHeight(float width)
        {
            return style == Style.OptionButton ? height + 6f : height;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            bool selected = SettingsManager.GetSetting(modId, key) == value;
            string resolvedLabel = Helpers.TryTranslate(label, tKey);
            string resolvedTooltip = Helpers.TryTranslate(tooltip, tKeyTip);

            if (resolvedLabel == null && style == Style.RadioButton)
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
                Rect drawRect = inRect;
                if (style == Style.OptionButton)
                { // Add extra space for this style
                    drawRect = inRect.TopPartPixels(inRect.height - 3f).BottomPartPixels(inRect.height - 6f);
                }
                if (DrawRadioButton(drawRect, resolvedLabel, selected, highlight, resolvedTooltip))
                {
                    SettingsManager.SetSetting(modId, key, value);
                }
            }
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
                TextAnchor prevAnchor = Verse.Text.Anchor;
                Verse.Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(rect, label);
                Verse.Text.Anchor = prevAnchor;

                clicked = Widgets.ButtonInvisible(rect);
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
                Widgets.DrawOptionBackground(rect.MiddlePartPixels(rect.width - 6f, rect.height), active);
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
