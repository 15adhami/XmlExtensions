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

        private List<TabRecord> tabRecords;
        private int selectedTab = 0;
        private float tabHeight = 31;
        private float maxTabWidth = 200;

        protected override bool Init(string selectedMod)
        {
            if (tabs != null)
            {
                foreach (Tab tab in tabs)
                {
                    if (!InitializeSettingsList(selectedMod, tab.settings, tab.label))
                    {
                        return false;
                    }
                }
            }

            tabRecords = new List<TabRecord>();
            for (int i = 0; i < tabs.Count; i++)
            {
                int t = i;
                TabRecord temp = new TabRecord(tabs[t].label, delegate ()
                {
                    selectedTab = t;
                }, () => selectedTab == t);
                tabRecords.Add(temp);
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return CalculateHeightSettingsList(width, selectedMod, tabs[selectedTab].settings) + rows*((int)tabHeight);
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            inRect.yMin += rows*tabHeight;
            TabDrawer.DrawTabs(inRect, tabRecords, rows, maxTabWidth);
            DrawSettingsList(inRect, selectedMod, tabs[selectedTab].settings);
        }

        internal override bool PreOpen(string selectedMod)
        {
            foreach (Tab tab in tabs)
            {
                if (!PreOpenSettingsList(selectedMod, tab.settings))
                {
                    return false;
                }
            }
            return true;
        }
    }
}