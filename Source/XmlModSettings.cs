using System.Collections.Generic;
using Verse;

namespace XmlExtensions
{
    public class XmlModSettings : ModSettings
    {
        public Dictionary<string, string> dataDict;
        public float testFloat = 2;
        public XmlModSettings()
        {
            dataDict = new Dictionary<string, string>();
        }
        public override void ExposeData()
        {
            Scribe_Collections.Look(ref dataDict, "dataDict", LookMode.Value, LookMode.Value);
            Scribe_Values.Look(ref testFloat, "testFloat");
            base.ExposeData();
        }
    }
}
