using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class TabView : SettingContainer
    {
        public class Tab
        {
            public string label;
            public string tKey;
            public List<SettingContainer> settings = new List<SettingContainer>();

        }
        public List<Tab> tabs;

        private List<TabRecord> tabRecords;
        private int selectedTab = 0;
        private float tabHeight = 32;

        protected override bool Init()
        {
            if (tabs != null)
            {
                foreach (Tab tab in tabs)
                {
                    if (!InitializeSettingsList(tab.settings, tab.label))
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

        protected override bool SetDefaultValue(string modId)
        {
            foreach (Tab tab in tabs)
            {
                if (!SetDefaultValueSettingsList(modId, tab.settings, tab.label))
                {
                    return false;
                }
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return GetHeightSettingsList(width, selectedMod, tabs[selectedTab].settings) + (int)tabHeight + 4;
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            inRect.yMin += tabHeight;
            TabDrawer.DrawTabs(inRect, tabRecords, 200f);
            DrawSettingsList(inRect, selectedMod, tabs[selectedTab].settings);
        }

        protected override bool PreClose(string selectedMod)
        {
            foreach (Tab tab in tabs)
            {
                if (!DoPreCloseSettingsList(selectedMod, tab.settings, tab.label))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
