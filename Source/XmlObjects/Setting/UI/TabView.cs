using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class TabView : SettingContainer
    {
        public class Tab
        {
            public string label;
            public string tKey;
            public List<SettingContainer> settings = new List<SettingContainer>();
        }

        public List<Tab> tabs;
        public int rows = 1;
        public float? maxTabWidth = null; // Obsolete
        public int defaultTab = 1;

        private List<TabRecord> tabRecords;
        private int selectedTab = 0;
        private float tabHeight = 31; // Game-defined constant

        protected override bool Init()
        {
            if (defaultTab <= 0)
            {
                Error("<defaultTab> must be at least 1");
                return false;
            }
            if (defaultTab > tabs.Count)
            {
                Error("<defaultTab> must be at most " + tabs.Count.ToString());
                return false;
            }
            if (rows > tabs.Count)
            {
                rows = tabs.Count;
            }
            selectedTab = defaultTab - 1;
            searchType = SearchType.SearchAllAndHighlight;
            addDefaultSpacing = false;
            if (tabs != null)
            {
                foreach (Tab tab in tabs)
                {
                    if (!InitializeContainers(tab.settings, tab.label))
                    {
                        return false;
                    }
                }
            }

            tabRecords = [];
            for (int i = 0; i < tabs.Count; i++)
            {
                int t = i;
                TabRecord temp = new(tabs[t].label, delegate () { selectedTab = t; }, () => selectedTab == t);
                tabRecords.Add(temp);
            }
            return true;
        }

        protected override float CalculateHeight(float width)
        {
            return CalculateHeightSettingsList(width, tabs[selectedTab].settings) + rows*((int)tabHeight);
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            inRect.yMin += rows*tabHeight;
            TabDrawer.DrawTabs(inRect, tabRecords, rows, null);
            DrawSettingsList(inRect, tabs[selectedTab].settings);
        }

        protected override void DrawFilterBox(Rect inRect)
        {
            // Get Rects of all tabs with filtered settings then call FilterBox(Rect) on each one
            foreach(Tab tab in tabs)
            {
                if (containedFiltered[tab.settings])
                {
                    // TODO: Get Rect of tab, then call FilterBox(Rect);
                }
            }
        }

        protected override bool PreOpen()
        {
            selectedTab = defaultTab - 1;
            return true;
        }
    }
}