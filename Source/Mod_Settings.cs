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

        public override void ExposeData()
        {
            Scribe_Values.Look(ref intDict, "intDict");
            Scribe_Values.Look(ref floatDict, "floatDict");
            Scribe_Values.Look(ref boolDict, "boolDict");
            Scribe_Values.Look(ref stringDict, "stringDict");
            Scribe_Values.Look(ref textDict, "textDict");
            base.ExposeData();
        }
    }
}
