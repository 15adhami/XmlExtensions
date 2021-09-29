using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class Tab : SettingContainer
    {
        public string label;
        public string tKey;
        public List<SettingContainer> settings = new List<SettingContainer>();

        public override int getHeight(float width, string selectedMod)
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

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            
            Rect rectTab = listingStandard.GetRect(tabs[selectedTab].getHeight(listingStandard.ColumnWidth, selectedMod) + tabHeight+4);
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

        public override int getHeight(float width, string selectedMod)
        {
            return tabs[selectedTab].getHeight(width, selectedMod) + (int)tabHeight+4;
        }

        public override void Init()
        {
            base.Init();
            if(tabs != null)
            {
                foreach (Tab tab in tabs)
                {
                    if (tab.settings != null)
                    {
                        foreach (SettingContainer setting in tab.settings)
                        {
                            setting.Init();
                        }
                    }                    
                }
            }
            
            tabRecords = new List<TabRecord>();
            for (int i = 0; i < tabs.Count; i++)
            {
                int t = i;
                TabRecord temp = new TabRecord(tabs[t].label, delegate()
                {
                    this.selectedTab = t;
                }, () => this.selectedTab == t);
                tabRecords.Add(temp);
            }
        }

        public override bool SetDefaultValue(string modId)
        {
            int t = 0;
            foreach(Tab tab in tabs)
            {
                t++;
                int c = 0;
                foreach(SettingContainer setting in tab.settings)
                {
                    c++;
                    if (!setting.SetDefaultValue(modId))
                    {
                        PatchManager.errors.Add("Error in XmlExtensions.Setting.TabView: failed to initialize a setting in tab: " + t.ToString() + ", at position: " + c.ToString());
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
