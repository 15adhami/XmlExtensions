using System.Collections.Generic;
using Verse;

namespace XmlExtensions
{
    // The "Real" ModSettings Class
    public class XmlModBaseSettings : ModSettings
    {
        public Dictionary<string, string> dataDict;
        public bool trace = true;
        public bool standardMods = false;

        public XmlModBaseSettings()
        {
            dataDict = new Dictionary<string, string>();
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref dataDict, "dataDict", LookMode.Value, LookMode.Value);
            Scribe_Values.Look(ref trace, "trace");
            Scribe_Values.Look(ref standardMods, "standardMods");
            base.ExposeData();
        }
    }
}