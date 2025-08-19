using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class Searchbox : SettingContainer
    {
        internal bool searchLabels = true;
        internal bool searchToolTips = true;
        internal bool searchTexts = true;
        internal bool showCount = true;
        internal bool animate = false;
        internal Color highlightColor = Color.white;

        protected override bool Init()
        {
            addDefaultSpacing = false;
            menuDef.searchLabels = searchLabels;
            menuDef.searchToolTips = searchToolTips;
            menuDef.searchTexts = searchTexts;
            menuDef.animateHighlight = animate;
            menuDef.highlightColor = highlightColor;
            return true;
        }
        protected override float CalculateHeight(float width)
        {
            return 22;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            // Get searchText
            string searchText = menuDef.searchText;
            string newSearchText = Widgets.TextField(inRect, searchText);
            menuDef.searchText = newSearchText;
        }

        protected override void PostDrawSettingContents(Rect inRect)
        {
            // Draw search count
            Color colorTemp = GUI.color;
            GUI.color = new Color(0.5f, 0.5f, 0.5f);
            if (menuDef.searchText.NullOrEmpty() || !showCount)
            {
                GUI.DrawTexture(inRect.RightPartPixels(22), TexButton.Search);
            }
            else
            {
                Rect rect = new(inRect.x - 4f, inRect.y + 1f, inRect.width, inRect.height);
                Verse.Text.Anchor = TextAnchor.UpperRight;
                string translatedResults = "{0} Result(s)".TryTKey("XmlExtensions_SearchResults");
                Widgets.Label(rect, translatedResults.Replace("{0}", menuDef.foundResults.ToString()));
                Verse.Text.Anchor = TextAnchor.UpperLeft;
            }
            GUI.color = colorTemp;
        }
    }
}
