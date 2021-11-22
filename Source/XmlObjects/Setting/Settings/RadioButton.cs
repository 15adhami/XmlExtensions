using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace XmlExtensions.Setting
{
    internal class RadioButton : KeyedSettingContainer
    {
        public string value;
        public string tooltip;
        public string tKey;
        public string tKeyTip;
        public Color color = Color.white;
        public bool highlight = true;

        private Texture2D RadioButOnTex;
        private Texture2D RadioButOffTex;

        protected override bool Init(string selectedMod)
        {
            RadioButOnTex = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOn");
            RadioButOffTex = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOff");
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return GetDefaultSpacing() + 22;
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            bool selected = SettingsManager.GetSetting(selectedMod, key) == value;
            Rect rect = inRect.TopPartPixels(22f);
            if (DrawRadioButton(rect, Helpers.TryTranslate(label, tKey), selected, highlight, Helpers.TryTranslate(tooltip, tKeyTip)))
            {
                SettingsManager.SetSetting(selectedMod, key, value);
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
                TipSignal tip = (tooltipDelay.HasValue ? new TipSignal(tooltip, tooltipDelay.Value) : new TipSignal(tooltip));
                TooltipHandler.TipRegion(rect, tip);
            }
            TextAnchor anchor = Verse.Text.Anchor;
            Verse.Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect, label);
            Verse.Text.Anchor = anchor;
            bool num = Widgets.ButtonInvisible(rect);
            if (num && !active)
            {
                SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
            }
            Color colorTemp = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(image: (!active) ? RadioButOffTex : RadioButOnTex, position: new Rect(rect.x + rect.width - 24f, rect.y + rect.height / 2f - 12f, 24f, 24f));
            GUI.color = colorTemp;
            return num;
        }
    }
}