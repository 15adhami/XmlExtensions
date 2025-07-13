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
        public string url;

        public enum Anchor
        {
            Left = TextAnchor.UpperLeft,
            Middle = TextAnchor.UpperCenter,
            Right = TextAnchor.UpperRight
        }

        private string cachedText;
        private float cachedHeight;
        private Vector2 cachedSize;

        protected override float CalculateHeight(float width)
        {
            Verse.Text.Font = font;
            Verse.Text.Anchor = (TextAnchor)anchor;
            string str = Helpers.TryTranslate(text, tKey);
            if (keys != null)
            {
                foreach (string key in keys)
                {
                    str = Helpers.SubstituteVariable(str, key, SettingsManager.GetSetting(modId, key), "{}");
                }
            }
            cachedText = str;
            cachedHeight = Verse.Text.CalcHeight(str, width);
            if (url != null)
            {
                cachedSize = Verse.Text.CalcSize(str);
                cachedSize.x = Math.Min(cachedSize.x, width);
            }
            Verse.Text.Font = GameFont.Small;
            Verse.Text.Anchor = TextAnchor.UpperLeft;
            return cachedHeight;
        }

        protected override void DrawSettingContents(Rect inRect)
        {//M: 29 S: 22 T:18
            Verse.Text.Font = font;
            Verse.Text.Anchor = (TextAnchor)anchor;
            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(inRect, Helpers.TryTranslate(tooltip, tKeyTip));
            }
            Widgets.Label(inRect, cachedText);
            Verse.Text.Font = GameFont.Small;
            Verse.Text.Anchor = TextAnchor.UpperLeft;
            cachedText = "";
        }

        protected override bool Init()
        {
            if (xpath != null)
            {
                text = PatchManager.XmlDocs.MainDocument.SelectSingleNode(xpath).InnerText;
            }
            return true;
        }
    }
}