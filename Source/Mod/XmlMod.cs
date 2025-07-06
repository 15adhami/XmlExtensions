using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using XmlExtensions.Action;

namespace XmlExtensions
{
    /// <summary>
    /// Class containing internal data and obsolete methods (for backwards compatability).
    /// </summary>
    public class XmlMod : Mod
    {
        internal static XmlModBaseSettings allSettings;

        internal static string loadedMod;
        internal static List<string> loadedXmlMods;
        internal static Dictionary<string, XmlModSettings> settingsPerMod;
        internal static HashSet<string> modsWithSettings;

        internal static Dictionary<string, SettingsMenuDef> menus;
        internal static Dictionary<string, List<string>> unusedSettings;
        internal static List<string> unusedMods;
        internal static Dictionary<string, Dictionary<string, List<KeyedActionContainer>>> keyedActionListDict;

       public XmlMod(ModContentPack content) : base(content)
        {
            allSettings = GetSettings<XmlModBaseSettings>();
            if (allSettings.PinnedMods == null)
            {
                allSettings.PinnedMods = new HashSet<string>();
            }
            var harmony = new Harmony("com.github.15adhami.xmlextensions");
            harmony.PatchAll();
            keyedActionListDict = new Dictionary<string, Dictionary<string, List<KeyedActionContainer>>>();
            unusedMods = new List<string>();
            unusedSettings = new Dictionary<string, List<string>>();
            menus = new Dictionary<string, SettingsMenuDef>();
            loadedXmlMods = new List<string>();
            modsWithSettings = new();
            foreach (string fullKey in allSettings.dataDict.Keys)
            {
                string modId = fullKey.Split(';')[0];
                modsWithSettings.Add(modId);
            }
        }

        internal static void AddKeyedAction(string modId, string key, KeyedActionContainer action)
        {
            if (!keyedActionListDict.ContainsKey(modId))
            {
                keyedActionListDict.Add(modId, new Dictionary<string, List<KeyedActionContainer>>());
            }
            if (!keyedActionListDict[modId].ContainsKey(key))
            {
                keyedActionListDict[modId].Add(key, new List<KeyedActionContainer>());
            }
            keyedActionListDict[modId][key].Add(action);
        }

        internal static bool ModHasSettings(string modId)
        {
            bool flag = false;
            foreach(string key in allSettings.dataDict.Keys)
            {
                if (key.Split(';')[0] == modId)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Find.WindowStack.Add(new XmlExtensions_MenuModSettings(null, true));
        }

        public override string SettingsCategory()
        {
            return "XmlExtensions_Label".Translate();
        }

        [Obsolete]
        public static string getSetting(string modId, string key)
        {
            return SettingsManager.GetSetting(modId, key);
        }
    }
}