using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace XmlExtensions.Setting
{
    internal class Checkbox : KeyedSettingContainer
    {
        public bool highlight = true;
        public Style style = Style.Checkbox;
        public float height = -1f;

        public enum Style
        {
            Checkbox,
            UIButton,
            OptionButton,
            MainButton
        }

        protected override bool Init()
        {
            if (style != Style.Checkbox && height < 0f)
            {
                height = 30f;
            }
            else if (style == Style.Checkbox)
            {
                height = 22f;
            }
            if (tooltip != null)
            {
                highlight = true;
            }
            return true;
        }

        protected override float CalculateHeight(float width)
        {
            return style == Style.OptionButton ? height + 6f : height;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            bool currBool = bool.Parse(SettingsManager.GetSetting(modId, key));
            Rect drawRect = inRect;
            if (style == Style.OptionButton)
            { // Add extra space for this style
                drawRect = inRect.TopPartPixels(inRect.height - 3f).BottomPartPixels(inRect.height - 6f);
            }
            DrawCheckbox(drawRect, label.TranslateIfTKeyAvailable(tKey), ref currBool, tooltip.TranslateIfTKeyAvailable(tKeyTip));
            SettingsManager.SetSetting(modId, key, currBool.ToString());
        }

        private void DrawCheckbox(Rect rect, string label, ref bool active, string tooltip = null, float? tooltipDelay = null)
        {
            if (!tooltip.NullOrEmpty())
            {
                TipSignal tip = (tooltipDelay.HasValue ? new TipSignal(tooltip, tooltipDelay.Value) : new TipSignal(tooltip));
                TooltipHandler.TipRegion(rect, tip);
            }
            bool clicked;
            if (style == Style.Checkbox)
            {
                if (highlight && Mouse.IsOver(rect))
                {
                    Widgets.DrawHighlight(rect);
                }
                Widgets.CheckboxLabeled(rect, label, ref active);
            }
            else if (style == Style.UIButton)
            {
                if (!active)
                {
                    clicked = Widgets.ButtonText(rect, label);
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
                if (clicked)
                {
                    if (active)
                    {
                        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                        active = false;
                    }
                    else
                    {
                        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                        active = true;
                    }
                }
            }
            else if (style == Style.OptionButton)
            {
                Widgets.DrawOptionBackground(rect.MiddlePartPixels(rect.width - 6f, rect.height), active);
                Verse.Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect, label);
                Verse.Text.Anchor = TextAnchor.UpperLeft;
                clicked = Widgets.ButtonInvisible(rect);
                if (clicked)
                {
                    if (active)
                    {
                        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                        active = false;
                    }
                    else
                    {
                        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                        active = true;
                    }
                }
            }
            else if (style == Style.MainButton)
            {
                if (!active)
                {
                    clicked = Widgets.ButtonTextSubtle(rect, null, 0f, -1f, SoundDefOf.Mouseover_Category);
                }
                else
                {
                    Color colorTemp = GUI.color;
                    GUI.color = GenUI.MouseoverColor;
                    Widgets.DrawAtlas(rect, Widgets.ButtonSubtleAtlas);
                    GUI.color = Color.grey;
                    Widgets.DrawBox(rect, 2);
                    GUI.color = colorTemp;
                    clicked = Widgets.ButtonInvisible(rect);
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
                if (clicked)
                {
                    if (active)
                    {
                        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                        active = false;
                    }
                    else
                    {
                        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                        active = true;
                    }
                }
            }
        }
    }
}