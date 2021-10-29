using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class Text : SettingContainer
    {
        public string text;
        public GameFont font = GameFont.Small;
        public Anchor anchor = Anchor.Left;
        public string tooltip = null;
        public string tKey = null;
        public string tKeyTip = null;
        public List<string> keys;
        public string xpath;

        public enum Anchor
        {
            Left = TextAnchor.UpperLeft,
            Middle = TextAnchor.UpperCenter,
            Right = TextAnchor.UpperRight
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            Verse.Text.Font = font;
            Verse.Text.Anchor = (TextAnchor)anchor;
            string str = Helpers.TryTranslate(text, tKey);
            float h = (float)Math.Ceiling(Verse.Text.CalcHeight(str, width));
            Verse.Text.Font = GameFont.Small;
            Verse.Text.Anchor = TextAnchor.UpperLeft;
            return h + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {//M: 29 S: 22 T:18
            Verse.Text.Font = font;
            Verse.Text.Anchor = (TextAnchor)anchor;
            string str = Helpers.TryTranslate(text, tKey);
            if (keys != null)
            {
                foreach (string key in keys)
                {
                    str = Helpers.SubstituteVariable(str, key, SettingsManager.GetSetting(selectedMod, key), "{}");
                }
            }
            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(inRect, Helpers.TryTranslate(tooltip, tKeyTip));
            }
            Widgets.Label(inRect, str);
            Verse.Text.Font = GameFont.Small;
            Verse.Text.Anchor = TextAnchor.UpperLeft;
        }

        protected override bool Init(string selectedMod)
        {
            if (xpath != null)
            {
                text = PatchManager.defaultDoc.SelectSingleNode(xpath).InnerText;
            }
            return true;
        }
    }
}