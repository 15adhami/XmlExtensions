using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class CollapsibleSettings : SettingContainer
    {
        public GameFont headerFont = GameFont.Small;
        public State defaultState = State.Closed;
        public Anchor anchor = Anchor.Left;
        public List<SettingContainer> settingsOpen;
        public List<SettingContainer> settingsClosed;

        public enum Anchor
        {
            Left,
            Middle
        }

        public enum State
        {
            Open,
            Closed
        }

        private float headerHeight = 0f;
        private float labelPadding = 0f;
        private float buttonSize = 0f;
        private State state = State.Closed;

        protected override bool Init()
        {
            searchType = SearchType.SearchAllAndHighlight;
            if (headerFont == GameFont.Medium)
            {
                labelPadding = 3;
                headerHeight = 29 + 2 * labelPadding;
                buttonSize = 29;
            }
            else if (headerFont == GameFont.Small)
            {
                labelPadding = 2;
                headerHeight = 22 + 2 * labelPadding;
                buttonSize = 22;
            }
            else
            {
                labelPadding = 1;
                headerHeight = 18 + 2 * labelPadding;
                buttonSize = 18;
            }
            if (!InitializeContainers(settingsOpen)) { return false; }
            if (!InitializeContainers(settingsClosed)) { return false; }
            return true;
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
                height += CalculateHeightSettingsList(width, settingsOpen);
            }
            else
            {
                height += CalculateHeightSettingsList(width, settingsClosed);
            }
            return height;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            Verse.Text.Font = headerFont;

            Rect headerRect = inRect.TopPartPixels(headerHeight);
            Rect headerRectInner = headerRect.ContractedBy(0f, labelPadding);

            // Draw header
            Widgets.DrawBoxSolid(headerRect, new(1f, 1f, 1f, 0.05f));
            headerRectInner.SplitVertically(buttonSize, out Rect buttonRect, out Rect labelRect);
            buttonRect.y -= 1;
            if (anchor == Anchor.Left) 
            {
                Widgets.Label(labelRect.TrimLeftPartPixels(6f), label.TryTKey(tKey)); 
            }
            else
            {
                Verse.Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(headerRectInner, label.TryTKey(tKey));
                Verse.Text.Anchor = TextAnchor.UpperLeft;
            }
            if (state == State.Open) { Widgets.DrawTextureRotated(buttonRect, TexButton.Reveal, 90); }
            else { GUI.DrawTexture(buttonRect, TexButton.Reveal); }

            Verse.Text.Font = GameFont.Small;

            // Draw highlight and tooltip
            Widgets.DrawHighlightIfMouseover(headerRect);
            if (!tooltip.NullOrEmpty()) { TooltipHandler.TipRegion(headerRect, tooltip.TryTKey(tKeyTip)); }

            // Toggle settings
            if (Widgets.ButtonInvisible(headerRect)) { state = state == State.Open ? State.Closed : State.Open; }

            // Draw settings
            if (state == State.Open) { DrawSettingsList(inRect.TrimTopPartPixels(headerHeight), settingsOpen); }
            else { DrawSettingsList(inRect.TrimTopPartPixels(headerHeight), settingsClosed); }
        }
    }
}