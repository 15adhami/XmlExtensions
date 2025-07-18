using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using XmlExtensions.Action;

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
        public List<ActionContainer> actions;

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
            if (key != null)
            {
                str = Helpers.SubstituteVariable(str, "key", SettingsManager.GetSetting(modId, key), "{}");
            }
            cachedText = str;
            cachedHeight = Verse.Text.CalcHeight(str, width);
            if (actions != null)
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
                string tooltipLabel = Helpers.TryTranslate(tooltip, tKeyTip);
                if (keys != null)
                {
                    foreach (string key in keys)
                    {
                        tooltipLabel = Helpers.SubstituteVariable(tooltipLabel, key, SettingsManager.GetSetting(modId, key), "{}");
                    }
                }
                if (key != null)
                {
                    tooltipLabel = Helpers.SubstituteVariable(tooltipLabel, "key", SettingsManager.GetSetting(modId, key), "{}");
                }
                TooltipHandler.TipRegion(inRect, tooltipLabel);
            }
            Rect alignedRect;
            if (anchor == Anchor.Left)
            {
                alignedRect = inRect.LeftPartPixels(cachedSize.x);
            }
            else if (anchor == Anchor.Right)
            {
                alignedRect = inRect.RightPartPixels(cachedSize.x);
            }
            else
            {
                alignedRect = inRect.MiddlePartPixels(cachedSize.x, cachedHeight);
            }
            if (actions != null)
            {
                Widgets.DrawHighlightIfMouseover(alignedRect);
                if (Widgets.ButtonInvisible(alignedRect))
                {
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                    int i = 0;
                    foreach (ActionContainer action in actions)
                    {
                        i++;
                        if (!action.DoAction())
                        {
                            Error("Failed action at index=" + i.ToString());
                            ErrorManager.PrintErrors();
                            break;
                        }
                    }
                }
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