﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using XmlExtensions.Action;

namespace XmlExtensions
{
    public class XmlMod : Mod
    {
        public static XmlModBaseSettings allSettings;
        
        public static string loadedMod;
        public static List<string> loadedXmlMods;
        public static Dictionary<string, XmlModSettings> settingsPerMod;
        
        
        public static Dictionary<string, SettingsMenuDef> menus;
        public static Dictionary<string, List<string>> unusedSettings;
        public static List<string> unusedMods;
        public static Dictionary<string, Dictionary<string, List<KeyedAction>>> keyedActionListDict;

        

        static XmlMod()
        {
            var harmony = new Harmony("com.github.15adhami.xmlextensions");
            harmony.PatchAll();
            keyedActionListDict = new Dictionary<string, Dictionary<string, List<KeyedAction>>>();
            unusedMods = new List<string>();
            unusedSettings = new Dictionary<string, List<string>>();
            menus = new Dictionary<string, SettingsMenuDef>();
            allSettings = new XmlModBaseSettings();
            loadedXmlMods = new List<string>();
        }

        public XmlMod(ModContentPack content) : base(content)
        {
            allSettings = GetSettings<XmlModBaseSettings>();
        }

        public static void AddKeyedAction(string modId, string key, KeyedAction action)
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
    }
}