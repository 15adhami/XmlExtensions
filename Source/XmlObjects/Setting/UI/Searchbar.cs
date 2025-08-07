using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class Searchbar : SettingContainer
    {
        internal bool useLabels = true;
        internal bool useText = false;

        protected override bool Init()
        {
            addDefaultSpacing = false;
            menuDef.useLabels = useLabels;
            menuDef.useText = useText;
            return true;
        }
        protected override float CalculateHeight(float width)
        {
            return 22;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            string searchText = menuDef.searchText;
            string newSearchText = Widgets.TextField(inRect, searchText);
            menuDef.searchText = newSearchText;
        }
    }
}
