using System.Collections.Generic;
using Verse;

namespace XmlExtensions
{
    public class XmlModBaseSettings : ModSettings
    {
        public Dictionary<string, string> dataDict;

        
        public XmlModBaseSettings()
        {
            dataDict = new Dictionary<string, string>();
            
        }
        public override void ExposeData()
        {
            Scribe_Collections.Look(ref dataDict, "dataDict", LookMode.Value, LookMode.Value);
            base.ExposeData();
        }
    }
}
