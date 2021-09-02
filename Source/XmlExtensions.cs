using Verse;
using HarmonyLib;
using System.Reflection;

// This code is so bad

namespace XmlExtensions
{
    [StaticConstructorOnStartup]
    public static class XmlExtensions
    {

        
        static XmlExtensions()
        {
            int i = 0;
            foreach (SettingsMenuDef menuDef in DefDatabase<SettingsMenuDef>.AllDefsListForReading)
            {
                i++;
                menuDef.ApplyWorker();
            }
            Verse.Log.Message("[XML Extensions]: Finished initializing " + i.ToString() + " SettingsMenuDefs");
        }

        /*public override string SettingsCategory()
        {
            return "XML Extensions";
        }*/

       /* public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            List<KeyValuePair<string, string>> kvpList = XmlMod.allSettings.dataDict.ToList<KeyValuePair<string, string>>();
            int num = kvpList.Count();
            foreach (KeyValuePair<string, string> pair in kvpList)
            {
                bool del = false;
                listingStandard.CheckboxLabeled(pair.Key, ref del, "Delete");
                if (del)
                {
                    XmlMod.allSettings.dataDict.Remove(pair.Key);
                }                
            }
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }*/
    }


}