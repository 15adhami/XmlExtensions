using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using XmlExtensions.Action;

namespace XmlExtensions.Setting
{
    public class CollapsibleSettings : SettingContainer
    {
        public Style style = Style.Standard;
        public GameFont font = GameFont.Medium;
        public Anchor? anchor = null;
        public State defaultState = State.Closed;
        public List<SettingContainer> settings;

        public enum Style
        {
            Standard,
            OptionButton,
            MainButton
        }

        public enum Anchor
        {
            Left,
            Middle,
            Right
        }

        public enum State
        {
            Open,
            Closed
        }

        private float headerHeight = 0f;
        private float labelPadding = 0;
        private State state = State.Closed;

        protected override bool Init()
        {
            searchType = SearchType.SearchAllAndHighlight;
            if (style == Style.Standard)
                anchor ??= Anchor.Left;
            else
                anchor ??= Anchor.Middle;
            if (font == GameFont.Medium)
            {
                labelPadding = 6;
                headerHeight = 29 + 2 * labelPadding;
            }
            else if (font == GameFont.Small)
            {
                labelPadding = 4;
                headerHeight = 22 + 2 * labelPadding;
            }
            else
            {
                labelPadding = 2;
                headerHeight = 18 + 2 * labelPadding;
            }
            return InitializeContainers(settings);
        }

        protected override bool PreOpen()
        {
            state = defaultState;
            return true;
        }

        protected override float CalculateHeight(float width)
        {
            float height = headerHeight;
            if (state == State.Open)
            {
                height += CalculateHeightSettingsList(width, settings);
            }
            return height;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            Verse.Text.Font = font;
            Verse.Text.Anchor = (TextAnchor)anchor;

            Rect headerRect = inRect.TopPartPixels(headerHeight);

            if (style == Style.Standard)
            {
                Widgets.DrawBoxSolid(headerRect, new(1f, 1f, 1f, 0.05f));
            }

            Widgets.DrawHighlightIfMouseover(headerRect);

            if (!tooltip.NullOrEmpty())
            { // Implement
                //string tooltipLabel = Helpers.TryTranslate(tooltip, tKeyTip);
                //TooltipHandler.TipRegion(headerRect, tooltipLabel);
            }

            Rect labelRect = headerRect.ContractedBy(0f, labelPadding);
            if (anchor == Anchor.Left)
            {
                labelRect = labelRect.RightPartPixels(labelRect.width - labelPadding);
                if (state == State.Closed)
                {
                    Widgets.Label(labelRect, "> " + Helpers.TryTranslate(label, tKey));
                }
                else
                {
                    Widgets.Label(labelRect,  Helpers.TryTranslate(label, tKey));
                }
            }
            else if (anchor == Anchor.Right)
            {
                labelRect = labelRect.LeftPartPixels(labelRect.width - labelPadding);
                if (state == State.Closed)
                {
                    Widgets.Label(labelRect, Helpers.TryTranslate(label, tKey) + " <");
                }
                else
                {
                    Widgets.Label(labelRect, Helpers.TryTranslate(label, tKey));
                }
            }
            else
            {
                if (state == State.Closed)
                {
                    Widgets.Label(labelRect, "> " + Helpers.TryTranslate(label, tKey) + " <");
                }
                else
                {
                    Widgets.Label(labelRect, Helpers.TryTranslate(label, tKey));
                }
            }

            if (Widgets.ButtonInvisible(headerRect))
            {
                if (state == State.Open)
                {
                    state = State.Closed;
                }
                else
                {
                    state = State.Open;
                }
            }

            Verse.Text.Font = GameFont.Small;
            Verse.Text.Anchor = TextAnchor.UpperLeft;

            if (state == State.Open)
            {
                DrawSettingsList(inRect.BottomPartPixels(inRect.height - headerHeight), settings);
            }
        }
    }
}