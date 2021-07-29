using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace XmlExtensions
{
    public class XmlMod : Mod
    {
        public static XmlModBaseSettings allSettings;
        public static string loadedMod;
        public static List<string> loadedXmlMods;
        public static Dictionary<string, XmlModSettings> settingsPerMod;

        static XmlMod()
        {
            allSettings = new XmlModBaseSettings();
        }

        public XmlMod(ModContentPack content) : base(content)
        {
            allSettings = GetSettings<XmlModBaseSettings>();
        }
        

        public override string SettingsCategory()
        {
            return "XML Mod Settings"; 
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Rect rect = inRect.RightPart(0.75f);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            List<KeyValuePair<string, string>> kvpList = allSettings.dataDict.ToList<KeyValuePair<string, string>>();
            int num = kvpList.Count();
            List<bool> boolList = new List<bool>();

            for (int i=0; i<num; i++)
            {
                boolList.Add(false);
            }
            int c = 0;
            foreach(string modId in loadedXmlMods)
            {
                listingStandard.Label(settingsPerMod[modId].label);
                foreach (string key in settingsPerMod[modId].keys)
                {
                    string currStr = allSettings.dataDict[modId+"."+key];
                    allSettings.dataDict[modId + "." + key] = listingStandard.TextEntryLabeled(modId + "." + key, currStr);
                }
            }                       
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
	

        public static void addSetting(string modId, string key, string value)
        {
            if (allSettings == null)
            {
                allSettings = new XmlModBaseSettings();
            }
            allSettings.dataDict.Add(modId+"."+key, value);
        }
        
        public static bool tryGetSetting(string modId, string key, out string value)
        {
            string temp = "";
            bool b;
            b = allSettings.dataDict.TryGetValue(modId+"."+key, out temp);
            value = temp;
            return b;
        }

        public static void addXmlMod(string modId, string key, string type)
        {
            if (loadedXmlMods == null)
            {
                loadedXmlMods = new List<string>();
            }
            if (!loadedXmlMods.Contains(modId))
            {
                loadedXmlMods.Add(modId);
            }
            if (settingsPerMod == null)
            {
                settingsPerMod = new Dictionary<string, XmlModSettings>();
            }
            if (!settingsPerMod.Keys.Contains<string>(modId))
            {
                XmlModSettings t = new XmlModSettings(modId);
                t.keys.Add(key);
                settingsPerMod.Add(modId, t);
            }
            else
            {
                settingsPerMod[modId].keys.Add(key);
            }
        }

        public static void addXmlMod(string modId, string label)
        {
            if (loadedXmlMods == null)
            {
                loadedXmlMods = new List<string>();
            }
            if (!loadedXmlMods.Contains(modId))
            {
                loadedXmlMods.Add(modId);
            }
            if (settingsPerMod == null)
            {
                settingsPerMod = new Dictionary<string, XmlModSettings>();
            }
            if (!settingsPerMod.Keys.Contains<string>(modId))
            {
                XmlModSettings t = new XmlModSettings(modId);
                t.label = label;
                settingsPerMod.Add(modId, t);
            }
            else
            {
                settingsPerMod[modId].label = label;
            }
        }

        public static void tryAddSettings(SettingContainer container, string modId)
        {
            settingsPerMod[modId].settings.Add(container);
        }
    }
}
