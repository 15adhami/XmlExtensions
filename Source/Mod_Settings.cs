using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace XmlExtensions
{
    public class Mod_Settings : ModSettings
    {
        public Dictionary<string, int> intDict;
        public Dictionary<string, float> floatDict;
        public Dictionary<string, bool> boolDict;
        public Dictionary<string, string> stringDict;
        public Dictionary<string, string> textDict;
        public float testFloat = 2;

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref intDict, "intDict", LookMode.Value);
            Scribe_Collections.Look(ref floatDict, "floatDict", LookMode.Value);
            Scribe_Collections.Look(ref boolDict, "boolDict", LookMode.Value);
            Scribe_Collections.Look(ref stringDict, "stringDict", LookMode.Value);
            Scribe_Collections.Look(ref textDict, "textDict", LookMode.Value);
            Scribe_Values.Look(ref testFloat, "textFloat");
            base.ExposeData();
        }
    }
}
