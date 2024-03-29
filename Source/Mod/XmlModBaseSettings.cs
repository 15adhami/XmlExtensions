﻿using System.Collections.Generic;
using Verse;

namespace XmlExtensions
{
    // The "Real" ModSettings Class
    internal class XmlModBaseSettings : ModSettings
    {
        public Dictionary<string, string> dataDict;
        public bool trace = true;
        public bool standardMods = false;
        public bool mainButton = false;
        public bool advancedDebugging = false;
        public HashSet<string> PinnedMods;

        public XmlModBaseSettings()
        {
            dataDict = new Dictionary<string, string>();
            PinnedMods = new HashSet<string>();
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref dataDict, "dataDict", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref PinnedMods, "PinnedMods", LookMode.Value);
            Scribe_Values.Look(ref trace, "trace");
            Scribe_Values.Look(ref standardMods, "standardMods");
            Scribe_Values.Look(ref mainButton, "mainButton");
            Scribe_Values.Look(ref advancedDebugging, "advancedDebugging");
            base.ExposeData();
        }
    }
}