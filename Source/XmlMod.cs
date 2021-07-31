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
        public static Vector2 settingsPosition;
        public static Vector2 modListPosition;
        public static string loadedMod;
        public static List<string> loadedXmlMods;
        public static Dictionary<string, XmlModSettings> settingsPerMod;
        public static string selectedMod;
        static XmlMod()
        {
            allSettings = new XmlModBaseSettings();
            settingsPosition = new Vector2();
            modListPosition = new Vector2();
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
            Rect rectSettings = inRect.RightPart(0.7f);
            Rect rectMods = inRect.LeftPart(0.28f);
            drawXmlModSettings(rectSettings);
            drawXmlModList(rectMods);
            base.DoSettingsWindowContents(inRect);
        }

        private void drawXmlModSettings(Rect rect)
        {
            if (selectedMod != null)
            {
                //settingsPerMod[selectedMod].calculateHeight() + 24 + 48
                Rect scrollRect = new Rect(0, 0, rect.width - 20f, settingsPerMod[selectedMod].calculateHeight() + 22 + 32);
                Listing_Standard listingStandard = new Listing_Standard();
                listingStandard.BeginScrollView(rect, ref settingsPosition, ref scrollRect);
                listingStandard.verticalSpacing = settingsPerMod[selectedMod].defaultSpacing;
                listingStandard.Label(settingsPerMod[selectedMod].label);
                foreach (SettingContainer setting in settingsPerMod[selectedMod].settings)
                {
                    setting.drawSetting(listingStandard, selectedMod);
                }
                bool def = listingStandard.ButtonText("Default settings");
                if (def)
                {
                    settingsPerMod[selectedMod].resetSettings();
                    def = false;
                }
                listingStandard.EndScrollView(ref scrollRect);
            }
        }


        public static int tempInt = 600;
        private void drawXmlModList(Rect rect)
        {
            Rect scrollRect = new Rect(0, 0, rect.width - 20f, Math.Max(loadedXmlMods.Count*(22+2+30+2),585));
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.BeginScrollView(rect, ref modListPosition, ref scrollRect);
            foreach (string modId in loadedXmlMods)
            {
                bool t = false;
                listingStandard.Label(settingsPerMod[modId].label);
                t = listingStandard.ButtonText(settingsPerMod[modId].label, "test?");
                if (t) { selectedMod = modId; }
            }
            float f = (float)(tempInt);
            string buf = f.ToString();
            listingStandard.TextFieldNumericLabeled<float>("height", ref f, ref buf, 0, 99999);
            tempInt = (int)f;
            listingStandard.EndScrollView(ref scrollRect);
        }


        public static void addSetting(string modId, string key, string value)
        {
            if (allSettings == null)
            {
                allSettings = new XmlModBaseSettings();
            }
            allSettings.dataDict.Add(modId + "." + key, value);
            if(settingsPerMod[modId].defValues == null)
            {
                settingsPerMod[modId].defValues = new Dictionary<string, string>();
            }
            if (!settingsPerMod[modId].defValues.ContainsKey(key))
            {
                settingsPerMod[modId].defValues.Add(key, value);
            }
        }

        public static bool tryGetSetting(string modId, string key, out string value)
        {
            string temp = "";
            bool b;
            b = allSettings.dataDict.TryGetValue(modId + "." + key, out temp);
            value = temp;
            return b;
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
            if (settingsPerMod[modId].defValues == null)
            {
                settingsPerMod[modId].defValues = new Dictionary<string, string>();
            }
        }

        public static void tryAddSettings(SettingContainer container, string modId)
        {
            if (settingsPerMod[modId].settings == null)
            {
                settingsPerMod[modId].settings = new List<SettingContainer>();
            }
            settingsPerMod[modId].settings.Add(container);
        }

        public static string getSetting(string modId, string key)
        {
            return allSettings.dataDict.TryGetValue<string, string>(modId + "." + key);
        }

        public static void setSetting(string modId, string key, string value)
        {
            string fullKey = modId + "." + key;
            if (allSettings.dataDict.ContainsKey(fullKey))
            {

            }
            else
            {
                addSetting(modId, key, value);
            }
        }
    }
}
