using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class Tab
    {
        public string label;
        public string tKey;
        public List<SettingContainer> settings = new List<SettingContainer>();

        public int CalcHeight(float width, string selectedMod)
        {
            int h = 0;
            if (settings != null)
            {
                foreach (SettingContainer setting in settings)
                {
                    h += setting.GetHeight(width, selectedMod);
                }
            }
            return h;
        }
    }

    public class TabView : SettingContainer
    {
        protected List<Tab> tabs = new List<Tab>();
        private List<TabRecord> tabRecords;
        private int selectedTab = 0;
        private float tabHeight = 32;

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            
            Rect rectTab = listingStandard.GetRect(tabs[selectedTab].CalcHeight(listingStandard.ColumnWidth, selectedMod) + tabHeight+4);
            rectTab.yMin += tabHeight;
            TabDrawer.DrawTabs<TabRecord>(rectTab, tabRecords, 200f);            
            Listing_Standard tempListing = new Listing_Standard();
            tempListing.Begin(rectTab);
            
            for (int i = 0; i < tabs.Count; i++)
            {
                if (selectedTab == i)
                {
                    foreach(SettingContainer setting in tabs[i].settings)
                    {
                        setting.DrawSetting(tempListing, selectedMod);
                    }
                }
            }
            tempListing.End();
        }

        protected override int CalcHeight(float width, string selectedMod)
        {
            return tabs[selectedTab].CalcHeight(width, selectedMod) + (int)tabHeight+4;
        }

        protected override bool Init()
        {
            base.Init();
            if(tabs != null)
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
                TabRecord temp = new TabRecord(tabs[t].label, delegate()
                {
                    selectedTab = t;
                }, () => selectedTab == t);
                tabRecords.Add(temp);
            }
            return true;
        }

        protected override bool SetDefaultValue(string modId)
        {
            foreach(Tab tab in tabs)
            {
                if (!DefaultValueSettingsList(modId, tab.settings, tab.label))
                {
                    return false;
                }
            }
            return true;
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
