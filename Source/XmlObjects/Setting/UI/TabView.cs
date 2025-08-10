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
        public float maxTabWidth = 200;

        private List<TabRecord> tabRecords;
        private int selectedTab = 0;
        private float tabHeight = 31; // Game-defined constant

        protected override bool Init()
        {
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
            TabDrawer.DrawTabs(inRect, tabRecords, rows, maxTabWidth);
            DrawSettingsList(inRect, tabs[selectedTab].settings);
        }

        protected override void DrawFilterBox(Rect inRect)
        {
            // Get Rects of all tabs with filtered settings then call FilterBox(Rect) on each one
            foreach(Tab tab in tabs)
            {
                if (containedFiltered[tab.settings])
                {
                    // Get Rect of tab, then call FilterBox(Rect);
                }
            }
        }
    }
}