﻿using System.Collections.Generic;
using Verse;

namespace XmlExtensions
{
    // The "Real" ModSettings Class
    public class XmlModBaseSettings : ModSettings
    {
        public Dictionary<string, string> dataDict;
        public bool trace;

        // TODO: Decide default value
        public bool vanillaMods = false;

        public XmlModBaseSettings()
        {
            dataDict = new Dictionary<string, string>();
            trace = true;
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref dataDict, "dataDict", LookMode.Value, LookMode.Value);
            Scribe_Values.Look(ref trace, "trace");
            base.ExposeData();
        }
    }
}