using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<ModContentPack> packs = new();
        public List<DefNameContainer> defContainers = new();
        public ModContentPack selectedPack;
        public Vector2 scroll3 = new();
        public int mode = 0;

        public override Vector2 InitialSize => new Vector2(900f + 256 + 6f, 740);

        public PatchAnalyzer_Window()
        {
            doCloseButton = true;
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
            doCloseX = false;
            closeOnAccept = true;
        }

        public override void PreOpen()
        {
            base.PreOpen();
            Init();
        }

        private void Init()
        {
            defTypes.Clear();
            defs.Clear();
            mods.Clear();
            packs.Clear();
            defContainers.Clear();
            selectedDef = null;
            selectedDefType = null;
            selectedPack = null;
            if (mode == 0)
            {
                foreach (string name in PatchManager.DefModDict.Keys)
                {
                    string temp = name.Split(';')[0];
                    if (!defTypes.Any(d => d.Split(';')[0] == temp) && PatchManager.PatchedDefSet.Contains(name))
                    {
                        defTypes.Add(name);
                    }
                }
                defTypes.Sort();
            }
            else
            {
                foreach (ModContentPack pack in PatchManager.ModDefDict.Keys)
                {
                    if (PatchManager.PatchedModSet.Contains(pack))
                    {
                        packs.Add(pack);
                    }
                }
                packs.Sort(delegate (ModContentPack p1, ModContentPack p2) { return p1.Name.CompareTo(p2.Name); });
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            inRect = inRect.TopPartPixels(inRect.height - 40f);
            if (defTypes.Count == 0 && packs.Count == 0)
            {
                Listing_Standard listing = new();
                listing.Begin(inRect);
                listing.Label("XmlExtensions_EnableAdvancedDebuggingAndReboot".Translate());
                listing.End();
            }
            else
            {
                Rect infoRect = inRect.TopPartPixels(24f).LeftPart(0.49f);
                Listing_Standard listingInfo = new();
                listingInfo.Begin(infoRect);
                listingInfo.Label("XmlExtensions_AppliedPatches".Translate() + PatchManager.PatchCount.ToString() + "    " + "XmlExtensions_PatchedDefs".Translate() + PatchManager.PatchedDefSet.Count.ToString() + "    " + "XmlExtensions_TimeTaken".Translate() + PatchManager.watch2.ElapsedMilliseconds.ToString() + "ms");
                listingInfo.End();
                Rect modeRect = inRect.TopPartPixels(24f).RightPart(0.49f).LeftPart(0.3333f);
                Listing_Standard modeListing = new();
                modeListing.Begin(modeRect);
                Verse.Text.Anchor = TextAnchor.UpperRight;
                modeListing.Label("XmlExtensions_Mode".Translate() + "   ");
                Verse.Text.Anchor = TextAnchor.UpperLeft;
                modeListing.End();
                modeRect = inRect.TopPartPixels(24f).RightPart(0.49f).RightPart(0.6666f).LeftPart(0.3333f).LeftPart(0.95f);
                modeListing = new();
                modeListing.Begin(modeRect);
                if (modeListing.RadioButton("XmlExtensions_ByDef".Translate(), mode == 0, 0, "XmlExtensions_ByDefTip".Translate()))
                {
                    mode = 0;
                    Init();
                }
                modeListing.End();
                modeRect = inRect.TopPartPixels(24f).RightPart(0.49f).RightPart(0.6666f).LeftPart(0.6666f).RightPart(0.5f).LeftPart(0.975f).RightPart(0.9875f);
                modeListing = new();
                modeListing.Begin(modeRect);
                if (modeListing.RadioButton("XmlExtensions_ByMod".Translate(), mode == 1, 0, "XmlExtensions_ByModTip".Translate()))
                {
                    mode = 1;
                    Init();
                }
                modeListing.End();
                modeRect = inRect.TopPartPixels(24f).RightPart(0.49f).RightPart(0.6666f).RightPart(0.3333f).RightPart(0.95f);
                modeListing = new();
                modeListing.Begin(modeRect);
                if (modeListing.RadioButton("XmlExtensions_Summary".Translate(), mode == 2, 0, "XmlExtensions_SummaryTip".Translate()))
                {
                    mode = 2;
                    Init();
                }
                modeListing.End();
                inRect = inRect.BottomPartPixels(inRect.height - 24f);
                if (mode == 0)
                {
                    Rect defTypeRect = inRect.LeftPartPixels(0.3333f * inRect.width);
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
                            if (listing2.ButtonText(defType.Split(';')[0]))
                            {
                                selectedDefType = defType;
                                defs.Clear();
                                foreach (string name in PatchManager.DefModDict.Keys)
                                {
                                    string temp = name.Split(';')[0];
                                    if (temp == defType.Split(';')[0])
                                    {
                                        string temp2 = name.Split(';')[1];
                                       /* if (!defs.Any(d => d.Split(';')[1] == temp2) && PatchManager.PatchedDefSet.Contains(name))
                                        {
                                            defs.Add(name);
                                        }*/

                                        if (PatchManager.PatchedDefSet.Contains(name))
                                        {
                                            defs.Add(name);
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

                    Rect defRect = inRect.LeftPart(0.6666f).RightPartPixels(inRect.width * 0.3333f);
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
                            if (listing2.ButtonText(def.Split(';')[1]))
                            {
                                selectedDef = def;
                                mods.Clear();
                                foreach (ModContentPackContainer pack in PatchManager.DefModDict[selectedDef])
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

                    Rect modRect = inRect.RightPartPixels(inRect.width * 0.3333f);
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
                else if (mode == 1)
                {
                    Rect modRect = inRect.LeftPartPixels(0.3333f * inRect.width);
                    Listing_Standard listing = new();
                    listing.Begin(modRect);
                    listing.Label("XmlExtensions_Mods".Translate());
                    listing.GapLine(6f);
                    Rect modListRect = listing.GetRect(modRect.height - listing.CurHeight);
                    Rect scrollRect = new Rect(modListRect.x, modListRect.y, modListRect.width - 16f, Math.Max(packs.Count * 32, modListRect.height + 1));
                    Widgets.BeginScrollView(modListRect, ref scroll1, scrollRect);
                    Rect rect2 = new Rect(modListRect.x, modListRect.y, scrollRect.width, 99999f);
                    Listing_Standard listing2 = new();
                    listing2.Begin(rect2);
                    int curr = 0;
                    foreach (ModContentPack pack in packs)
                    {
                        if (curr * 32 + 32 >= scroll1.y && curr * 32 <= scroll1.y + modListRect.height)
                        {
                            if (pack == selectedPack)
                            {
                                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                            }
                            if (listing2.ButtonText(pack.Name))
                            {
                                selectedPack = pack;
                                defTypes.Clear();
                                foreach (string name in PatchManager.DefModDict.Keys)
                                {
                                    string temp = name.Split(';')[0];
                                    if (PatchManager.DefModDict[name].Any(p => p.Pack == pack))
                                    {
                                        ModContentPackContainer cont = PatchManager.DefModDict[name].Single(p => p.Pack == pack);
                                        if (!defTypes.Any(d => d.Split(';')[0] == temp) && ((cont.OperationTypes.Contains(null) && cont.OperationTypes.Count > 1) || (!cont.OperationTypes.Contains(null) && cont.OperationTypes.Count > 0)))
                                        {
                                            defTypes.Add(name);
                                        }
                                    }
                                }
                                defTypes.Sort();
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

                    Rect defTypeRect = inRect.LeftPart(0.6666f).RightPart(0.5f);
                    listing.Begin(defTypeRect);
                    listing.Label("XmlExtensions_DefTypes".Translate());
                    listing.GapLine(6f);
                    Rect defTypeListRect = listing.GetRect(defTypeRect.height - listing.CurHeight);
                    scrollRect = new Rect(defTypeListRect.x, defTypeListRect.y, defTypeListRect.width - 16f, Math.Max(defTypes.Count * 32, defTypeListRect.height + 1));
                    Widgets.BeginScrollView(defTypeListRect, ref scroll2, scrollRect);
                    rect2 = new Rect(defTypeListRect.x, defTypeListRect.y, scrollRect.width, 99999f);
                    listing2 = new();
                    listing2.Begin(rect2);
                    curr = 0;
                    foreach (string defType in defTypes)
                    {
                        if (curr * 32 + 32 >= scroll2.y && curr * 32 <= scroll2.y + defTypeListRect.height)
                        {
                            if (defType == selectedDefType)
                            {
                                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                            }
                            if (listing2.ButtonText(defType.Split(';')[0]))
                            {
                                selectedDefType = defType;
                                defContainers.Clear();
                                foreach (DefNameContainer nameContainer in PatchManager.ModDefDict[selectedPack])
                                {
                                    string temp = nameContainer.Name.Split(';')[0];
                                    if (temp == defType.Split(';')[0])
                                    {
                                        if (PatchManager.PatchedDefSet.Contains(nameContainer.Name) && PatchManager.ModDefDict[selectedPack].Any(p => ((p.Name == nameContainer.Name) && ((p.OperationTypes.Contains(null) && p.OperationTypes.Count > 1) || (!p.OperationTypes.Contains(null) && p.OperationTypes.Count > 0)))))
                                        {
                                            defContainers.Add(nameContainer);
                                        }
                                    }
                                }
                                defContainers.Sort();
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

                    Rect defRect = inRect.RightPart(0.3333f);
                    listing = new();
                    listing.Begin(defRect);
                    listing.Label("XmlExtensions_Defs".Translate());
                    listing.GapLine(6f);
                    Rect defListRect = listing.GetRect(defRect.height - listing.CurHeight);
                    float h = 0;
                    foreach (DefNameContainer defName in defContainers)
                    {
                        h += 24;
                        foreach (Type type in defName.OperationTypes)
                        {
                            h += 24;
                        }
                        h += 12;
                    }
                    scrollRect = new Rect(defListRect.x, defListRect.y, defListRect.width - 16f, Math.Max(h - 12, defListRect.height + 1));
                    Widgets.BeginScrollView(defListRect, ref scroll3, scrollRect);
                    rect2 = new Rect(defListRect.x, defListRect.y, scrollRect.width, 99999f);
                    listing2 = new();
                    listing2.Begin(rect2);
                    curr = 0;
                    foreach (DefNameContainer defName in defContainers)
                    {
                        listing2.Label(defName.Name.Split(';')[1] + ":");
                        GUI.color = Color.gray;
                        foreach (Type type in defName.OperationTypes)
                        {
                            listing2.Label("- " + (type?.ToString() ?? "Source"));
                        }
                        listing2.GetRect(12);
                        GUI.color = Color.white;
                        curr++;
                    }
                    listing2.End();
                    Widgets.EndScrollView();
                    listing.End();
                }
                else if (mode == 2)
                {
                    Rect modRect = inRect;
                    Listing_Standard listing = new();
                    listing.Begin(modRect);
                    listing.Label("XmlExtensions_Mods".Translate());
                    listing.GapLine(6f);
                    Rect modListRect = listing.GetRect(modRect.height - listing.CurHeight);
                    Rect scrollRect = new Rect(modListRect.x, modListRect.y, modListRect.width - 16f, Math.Max(PatchManager.PatchedModSet.Count * 100 - 12, modListRect.height + 1));
                    Widgets.BeginScrollView(modListRect, ref scroll3, scrollRect);
                    Rect rect2 = new Rect(modListRect.x, modListRect.y, scrollRect.width, 99999f);
                    Listing_Standard listing2 = new();
                    listing2.Begin(rect2);
                    listing2.verticalSpacing = 0;
                    foreach (ModContentPack pack in PatchManager.PatchedModSet)
                    {
                        listing2.Label(pack.Name + ":");
                        GUI.color = Color.gray;
                        listing2.Label("- " + PatchManager.ModPatchInfoDict[pack].patchCount.ToString() + " " + "XmlExtensions_PatchesApplied".Translate());
                        listing2.Label("- " + PatchManager.ModPatchInfoDict[pack].defNames.Count.ToString() + " " + "XmlExtensions_DefsPatched".Translate());
                        listing2.Label("- " + PatchManager.ModPatchInfoDict[pack].elapsedTime.ToString() + "ms " + "XmlExtensions_Elapsed".Translate());
                        listing2.GetRect(12);
                        GUI.color = Color.white;
                    }
                    listing2.End();
                    Widgets.EndScrollView();
                    listing.End();
                }
            }
        }
    }
}