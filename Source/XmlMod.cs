using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace XmlExtensions
{
    public class XmlMod : Mod
    {
        public static XmlModSettings allSettings;
        public static string loadedMod;
        public Dictionary<string, XmlModSettings> modSettingsDict;

        public XmlMod(ModContentPack content) : base(content)
        {
            allSettings = GetSettings<XmlModSettings>();
        }
        

        public override string SettingsCategory()
        {
            return "XML Mod Settings";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            // settings.DoSettingsWindowContents(inRect);
        }

        public static void addSetting(string modId, string key, string value)
        {
            allSettings.dataDict.Add(modId+"."+key, value);
        }
        
        public static void getSetting(string modId, string key, string value)
        {
            allSettings.dataDict.TryGetValue<string, string>(modId+"."+key);
        }
    }
}
