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
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
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
                foreach (string keyVar in settingsPerMod[modId].stringKeys)
                {
                    string currStr = allSettings.dataDict[modId + "." + keyVar];
                    allSettings.dataDict[modId+"."+keyVar] = listingStandard.TextEntryLabeled(keyVar, currStr);
                }
            }                       
            listingStandard.End();
            /*
            foreach (string modId in loadedXmlMods)
            {
                foreach (string keyVar in settingsPerMod[modId].stringKeys)
                {
                    bool btemp = false;
                    string currStr = allSettings.dataDict[modId + "." + keyVar];
                    allSettings.dataDict[modId + "." + keyVar] = listingStandard.TextEntryLabeled(keyVar, currStr);
                    listingStandard.CheckboxLabeled(keyVar, ref btemp);
                    boolList[c] = btemp;
                    c++;
                }
            }*/

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
                t.stringKeys.Add(key);
                settingsPerMod.Add(modId, t);
            }
            else
            {
                settingsPerMod[modId].stringKeys.Add(key);
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
    }
}
