using HarmonyLib;
using System;
using System.Collections.Generic;
using Verse;
using XmlExtensions.Action;

namespace XmlExtensions
{
    public class XmlMod : Mod
    {
        internal static XmlModBaseSettings allSettings;

        internal static string loadedMod;
        internal static List<string> loadedXmlMods;
        internal static Dictionary<string, XmlModSettings> settingsPerMod;

        internal static Dictionary<string, SettingsMenuDef> menus;
        internal static Dictionary<string, List<string>> unusedSettings;
        internal static List<string> unusedMods;
        internal static Dictionary<string, Dictionary<string, List<KeyedAction>>> keyedActionListDict;

       public XmlMod(ModContentPack content) : base(content)
        {
            allSettings = GetSettings<XmlModBaseSettings>();
            if (allSettings.PinnedMods == null)
            {
                allSettings.PinnedMods = new HashSet<string>();
            }
            var harmony = new Harmony("com.github.15adhami.xmlextensions");
            harmony.PatchAll();
            keyedActionListDict = new Dictionary<string, Dictionary<string, List<KeyedAction>>>();
            unusedMods = new List<string>();
            unusedSettings = new Dictionary<string, List<string>>();
            menus = new Dictionary<string, SettingsMenuDef>();
            loadedXmlMods = new List<string>();
        }

        internal static void AddKeyedAction(string modId, string key, KeyedAction action)
        {
            if (!keyedActionListDict.ContainsKey(modId))
            {
                keyedActionListDict.Add(modId, new Dictionary<string, List<KeyedAction>>());
            }
            if (!keyedActionListDict[modId].ContainsKey(key))
            {
                keyedActionListDict[modId].Add(key, new List<KeyedAction>());
            }
            keyedActionListDict[modId][key].Add(action);
        }

        [Obsolete]
        public static string getSetting(string modId, string key)
        {
            return SettingsManager.GetSetting(modId, key);
        }
    }
}