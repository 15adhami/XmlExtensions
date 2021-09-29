using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class Range : KeyedSettingContainer
    {
        public int min;
        public int max;
        public string key2;
        public int id = 0;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            if (key2 == null)
            {
                IntRange range = IntRange.FromString(XmlMod.allSettings.dataDict[selectedMod + ";" + this.key]);
                Color currColor = GUI.color;
                Rect rect = listingStandard.GetRect(28f);
                Widgets.IntRange(rect, id, ref range, min, max, null, 0);
                listingStandard.Gap(XmlMod.menus[XmlMod.activeMenu].defaultSpacing);
                //listingStandard.IntRange(ref range, min, max);
                GUI.color = currColor;
                XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = range.ToString();
            }
            else
            {
                IntRange range = IntRange.FromString(XmlMod.allSettings.dataDict[selectedMod + ";" + key]+"~"+ XmlMod.allSettings.dataDict[selectedMod + ";" + key2]);
                Color currColor = GUI.color;
                Rect rect = listingStandard.GetRect(28f);
                Widgets.IntRange(rect, id, ref range, min, max, null, 0);
                listingStandard.Gap(XmlMod.menus[XmlMod.activeMenu].defaultSpacing);
                //listingStandard.IntRange(ref range, min, max);
                GUI.color = currColor;
                XmlMod.allSettings.dataDict[selectedMod + ";" + this.key] = range.min.ToString();
                XmlMod.allSettings.dataDict[selectedMod + ";" + this.key2] = range.max.ToString();
            }
            
        }

        public override bool SetDefaultValue(string modId)
        {
            if (key == null)
            {
                PatchManager.errors.Add("Error in XmlExtensions.Setting.Range: <key> is null");
                return false;
            }
            if (key2 == null)
            {
                if (!XmlMod.settingsPerMod[modId].keys.Contains(key))
                {
                    XmlMod.settingsPerMod[modId].keys.Add(key);
                }
                if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(key))
                {
                    if (defaultValue != null)
                    {
                        XmlMod.settingsPerMod[modId].defValues.Add(key, defaultValue);
                        if (!XmlMod.allSettings.dataDict.ContainsKey(modId + ";" + key))
                            XmlMod.allSettings.dataDict.Add(modId + ";" + key, defaultValue);
                    }
                }
            }
            else
            {
                if (!XmlMod.settingsPerMod[modId].keys.Contains(key))
                {
                    XmlMod.settingsPerMod[modId].keys.Add(key);
                }
                if (!XmlMod.settingsPerMod[modId].keys.Contains(key2))
                {
                    XmlMod.settingsPerMod[modId].keys.Add(key2);
                }
                if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(key))
                {
                    if (defaultValue != null)
                    {
                        XmlMod.settingsPerMod[modId].defValues.Add(key, defaultValue.Split('~')[0]);
                        if (!XmlMod.allSettings.dataDict.ContainsKey(modId + ";" + key))
                            XmlMod.allSettings.dataDict.Add(modId + ";" + key, defaultValue.Split('~')[0]);
                    }
                }
                if (!XmlMod.settingsPerMod[modId].defValues.ContainsKey(key2))
                {
                    if (defaultValue != null)
                    {
                        XmlMod.settingsPerMod[modId].defValues.Add(key2, defaultValue.Split('~')[1]);
                        if (!XmlMod.allSettings.dataDict.ContainsKey(modId + ";" + key2))
                            XmlMod.allSettings.dataDict.Add(modId + ";" + key2, defaultValue.Split('~')[1]);
                    }
                }
            }
            return true;
        }

        public override int getHeight(float width, string selectedMod) { return (28 + XmlMod.menus[XmlMod.activeMenu].defaultSpacing); }

        public override void Init()
        {
            base.Init();
            id = PatchManager.rangeCount;
            PatchManager.rangeCount++;
        }
    }
}
