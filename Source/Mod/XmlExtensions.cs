﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace XmlExtensions
{
    [StaticConstructorOnStartup]
    internal static class XmlExtensions
    {
        static XmlExtensions()
        {
            ErrorManager.ClearErrors();
            foreach (PatchOperationExtended op in PatchManager.Coordinator.DelayedPatches)
            {
                op.Apply(null);
            }
            PatchManager.Coordinator.DelayedPatches.Clear();
            PatchManager.Coordinator.PatchModDict.Clear();
            HashSet<SettingsMenuDef> modsForMenu = new();
            // Initializing mod settings menus
            int i = 0;
            foreach (SettingsMenuDef menuDef in DefDatabase<SettingsMenuDef>.AllDefsListForReading)
            {
                if (!menuDef.submenu && !menuDef.Init())
                {
                    ErrorManager.PrintErrors(menuDef.label, menuDef.modContentPack);
                }
                else if (!menuDef.submenu)
                {
                    modsForMenu.Add(menuDef);
                    i++;
                }
                if (XmlMod.settingsPerMod[menuDef.modId].menus == null)
                {
                    XmlMod.settingsPerMod[menuDef.modId].menus = new Dictionary<string, SettingsMenuDef>();
                }
                XmlMod.settingsPerMod[menuDef.modId].menus.Add(menuDef.defName, menuDef);
            }
            XmlMod.loadedXmlMods.Sort(delegate (string id1, string id2)
            {
                if (XmlMod.settingsPerMod[id1].label != null && XmlMod.settingsPerMod[id2].label != null)
                    return XmlMod.settingsPerMod[id1].label.CompareTo(XmlMod.settingsPerMod[id2].label);
                else
                    return 0;
            });


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
            foreach (List<string> list in XmlMod.unusedSettings.Values)
            {
                list.Sort();
            }
            Verse.Log.Message("[XML Extensions] Initialized " + i.ToString() + " SettingsMenuDef(s) and found " + c.ToString() + " unused key(s) from " + XmlMod.unusedMods.Count.ToString() + " mod(s)");
            Verse.Log.Message(string.Concat("[XML Extensions] ", PatchManager.Profiler.TotalPatches, " total patch operations run in ", PatchManager.Profiler.globalWatch.ElapsedMilliseconds, "ms, ", PatchManager.Profiler.FailedPatches, " failed"));
            PatchManager.Coordinator.PatchModDict.Clear();
            DefDatabase<MainButtonDef>.GetNamed("XmlExtensions_MainButton_ModSettings").buttonVisible = XmlMod.allSettings.mainButton;
            LoadedModManager.GetMod(typeof(XmlMod)).WriteSettings();

            // Emit Mod classes
            Dictionary<string, ModContentPack> contentLookup = LoadedModManager.RunningMods.ToDictionary(b => b.PackageId.ToLower());
            int emiitedMods = 0;

            foreach (SettingsMenuDef menu in modsForMenu)
            {
                try
                {
                    Type emittedModType = ModEmitter.EmitMod(menu);
                    LoadedModManager.runningModClasses[emittedModType] = (Mod)Activator.CreateInstance(emittedModType, menu.modContentPack);
                    emiitedMods += 1;
                }
                catch
                {
                    Verse.Log.Error("[XML Extensions] Failed to emit " + menu.defName + " Mod class");
                }
            }
            Verse.Log.Message("[XML Extensions] Emitted " + emiitedMods.ToString() + " mod class(es)");
        }
    }
}