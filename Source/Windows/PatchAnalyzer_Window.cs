using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions
{
    internal class PatchAnalyzer_Window : Window
    {
        public List<string> defTypes = new();
        public Vector2 scroll1 = new();
        public string selectedDefType = null;
        public List<string> defs = new();
        public Vector2 scroll2 = new();
        public string selectedDef = null;
        public List<ModContentPackContainer> mods = new();
        public Vector2 scroll3 = new();
        public override Vector2 InitialSize => new Vector2(900, 740);

        public PatchAnalyzer_Window()
        {
            doCloseButton = true;
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
            doCloseX = true;
            closeOnAccept = true;
        }

        public override void PreOpen()
        {
            base.PreOpen();
            foreach (string name in PatchManager.DefModDict.Keys)
            {
                string temp = name.Split(';')[0];
                if (!defTypes.Contains(temp) && PatchManager.PatchedDefSet.Contains(name))
                {
                    defTypes.Add(temp);
                }
            }
            defTypes.Sort();
        }

        public override void DoWindowContents(Rect inRect)
        {
            inRect = inRect.TopPartPixels(inRect.height - 40f);
            if (defTypes.Count == 0)
            {
                Listing_Standard listing = new();
                listing.Begin(inRect);
                listing.Label("XmlExtensions_EnableAdvancedDebuggingAndReboot".Translate());
                listing.End();
            }
            else
            {
                Rect infoRect = inRect.TopPartPixels(24f);
                Listing_Standard listingInfo = new();
                listingInfo.Begin(infoRect);
                listingInfo.Label("XmlExtensions_PatchesApplied".Translate() + PatchManager.PatchCount.ToString() + "    " + "XmlExtensions_PatchedDefs".Translate() + PatchManager.PatchedDefSet.Count.ToString());
                listingInfo.End();
                inRect = inRect.BottomPartPixels(inRect.height - 24f);
                Rect defTypeRect = inRect.LeftPart(0.33f);
                Listing_Standard listing = new();
                listing.Begin(defTypeRect);
                listing.Label("XmlExtensions_DefTypes".Translate());
                listing.GapLine(6f);
                Rect defTypeListRect = listing.GetRect(defTypeRect.height - listing.CurHeight);
                Rect scrollRect = new Rect(defTypeListRect.x, defTypeListRect.y, defTypeListRect.width - 16f, Math.Max(defTypes.Count * 32, defTypeListRect.height + 1));
                Widgets.BeginScrollView(defTypeListRect, ref scroll1, scrollRect);
                Rect rect2 = new Rect(defTypeListRect.x, defTypeListRect.y, scrollRect.width, 99999f);
                Listing_Standard listing2 = new();
                listing2.Begin(rect2);
                int curr = 0;
                foreach (string defType in defTypes)
                {
                    if (curr * 32 + 32 >= scroll1.y && curr * 32 <= scroll1.y + defTypeListRect.height)
                    {
                        if (defType == selectedDefType)
                        {
                            GUI.color = new Color(0.7f, 0.7f, 0.7f);
                        }
                        if (listing2.ButtonText(defType))
                        {
                            selectedDefType = defType;
                            defs.Clear();
                            foreach (string name in PatchManager.DefModDict.Keys)
                            {
                                string temp = name.Split(';')[0];
                                if (temp == defType)
                                {
                                    string temp2 = name.Split(';')[1];
                                    if (!defs.Contains(temp2) && PatchManager.PatchedDefSet.Contains(selectedDefType + ";" + temp2))
                                    {
                                        defs.Add(temp2);
                                    }
                                }
                            }
                            defs.Sort();
                        }
                        GUI.color = Color.white;
                    }
                    else
                    {
                        listing2.GetRect(32);
                    }
                    curr++;
                }
                listing2.End();
                Widgets.EndScrollView();
                listing.End();

                Rect defRect = inRect.LeftPart(0.66f).RightPartPixels(inRect.width * 0.33f);
                listing = new();
                listing.Begin(defRect);
                listing.Label("XmlExtensions_Defs".Translate());
                listing.GapLine(6f);
                Rect defListRect = listing.GetRect(defRect.height - listing.CurHeight);
                scrollRect = new Rect(defListRect.x, defListRect.y, defListRect.width - 16f, Math.Max(defs.Count * 32, defListRect.height + 1));
                Widgets.BeginScrollView(defListRect, ref scroll2, scrollRect);
                rect2 = new Rect(defListRect.x, defListRect.y, scrollRect.width, 99999f);
                listing2 = new();
                listing2.Begin(rect2);
                curr = 0;
                foreach (string def in defs)
                {
                    if (curr * 32 + 32 >= scroll2.y && curr * 32 <= scroll2.y + defListRect.height)
                    {
                        if (def == selectedDef)
                        {
                            GUI.color = new Color(0.7f, 0.7f, 0.7f);
                        }
                        if (listing2.ButtonText(def))
                        {
                            selectedDef = def;
                            mods.Clear();
                            foreach (ModContentPackContainer pack in PatchManager.DefModDict[selectedDefType + ";" + selectedDef])
                            {
                                mods.Add(pack);
                            }
                            mods.Sort();
                        }
                        GUI.color = Color.white;
                    }
                    else
                    {
                        listing2.GetRect(32);
                    }
                    curr++;
                }
                listing2.End();
                Widgets.EndScrollView();
                listing.End();
                Rect modRect = inRect.RightPartPixels(inRect.width * 0.33f);
                listing = new();
                listing.Begin(modRect);
                listing.Label("XmlExtensions_Mods".Translate());
                listing.GapLine(6f);
                Rect modListRect = listing.GetRect(modRect.height - listing.CurHeight);
                scrollRect = new Rect(modListRect.x, modListRect.y, modListRect.width - 16f, Math.Max(mods.Count * 24, modListRect.height + 1));
                Widgets.BeginScrollView(modListRect, ref scroll3, scrollRect);
                rect2 = new Rect(modListRect.x, modListRect.y, scrollRect.width, 99999f);
                listing2 = new();
                listing2.Begin(rect2);
                foreach (ModContentPackContainer mod in mods)
                {
                    if (mod.Pack != null && mod.Pack.Name != null)
                    {
                        listing2.Label(mod.Pack.Name + ":");
                        GUI.color = Color.gray;
                        foreach (Type type in mod.OperationTypes)
                        {
                            listing2.Label("- " + (type?.ToString() ?? "Source"));
                        }
                        listing2.GetRect(12);
                        GUI.color = Color.white;
                    }
                }
                listing2.End();
                Widgets.EndScrollView();
                listing.End();
            }
        }
    }
}