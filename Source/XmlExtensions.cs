using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

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
            Verse.Log.Message("[XML Extensions] Finished initializing " + i.ToString() + " SettingsMenuDefs");

            int unique = 0;
            string tId = "";
            List<KeyValuePair<string, string>> kvpList = XmlMod.allSettings.dataDict.ToList<KeyValuePair<string, string>>();
            kvpList.Sort(delegate (KeyValuePair<string, string> pair1, KeyValuePair<string, string> pair2) { return pair1.Key.CompareTo(pair2.Key); });
            foreach (KeyValuePair<string, string> pair in kvpList)
            {
                if (pair.Key.Contains(";"))
                {
                    if (!XmlMod.loadedXmlMods.Contains(pair.Key.Split(';')[0]) || !XmlMod.settingsPerMod[pair.Key.Split(';')[0]].keys.Contains(pair.Key.Split(';')[1]))
                    {
                        if (tId != pair.Key.Split(';')[0])
                        {
                            tId = pair.Key.Split(';')[0];
                            unique++;
                        }
                        XmlMod.keyList.Add(pair.Key);
                    }
                }
                else
                {
                    XmlMod.allSettings.dataDict.Remove(pair.Key);
                }

            }
            Verse.Log.Message("[XML Extensions] Found " + XmlMod.keyList.Count.ToString() + " extra keys from " + unique.ToString() + " unloaded mods");
            XmlMod.keyList.Sort();
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