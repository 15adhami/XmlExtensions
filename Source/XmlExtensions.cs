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
            // Initializing mod settings menus
            int i = 0;
            foreach (SettingsMenuDef menuDef in DefDatabase<SettingsMenuDef>.AllDefsListForReading)
            {
                XmlMod.menus.Add(menuDef.defName, menuDef);
                menuDef.Init();
                if (!menuDef.submenu)
                    i++;            
            }
            XmlMod.loadedXmlMods.Sort(delegate (string id1, string id2) {
                if (XmlMod.settingsPerMod[id1].label != null && XmlMod.settingsPerMod[id2].label != null)
                    return XmlMod.settingsPerMod[id1].label.CompareTo(XmlMod.settingsPerMod[id2].label);
                else
                    return 0;
            });
            Verse.Log.Message("[XML Extensions] Finished initializing " + i.ToString() + " SettingsMenuDefs");

            // Initializing unloaded mod settings
            int c = 0;
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
                            XmlMod.unusedMods.Add(tId);
                            XmlMod.unusedSettings.Add(tId, new List<string>());
                        }
                        c++;
                        XmlMod.unusedSettings[tId].Add(pair.Key.Split(';')[1]);
                    }
                }
                else
                {
                    XmlMod.allSettings.dataDict.Remove(pair.Key);
                }

            }
            XmlMod.unusedMods.Sort();
            foreach(List<string> list in XmlMod.unusedSettings.Values)
            {
                list.Sort();
            }
            Verse.Log.Message("[XML Extensions] Found " + c.ToString() + " extra keys from " + XmlMod.unusedMods.Count.ToString() + " unloaded mods");
                
        }
    }


}