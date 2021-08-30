using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using HarmonyLib;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(LoadedModManager))]
    [HarmonyPatch("ApplyPatches")]
    static class ApplyPatches_Patch
    {

        static void Postfix()
        {
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
    }
}
