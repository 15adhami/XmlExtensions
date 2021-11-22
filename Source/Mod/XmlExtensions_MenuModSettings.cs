using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using XmlExtensions.Action;

namespace XmlExtensions
{
    // Window that appears when you press More Mod Settings
    internal class XmlExtensions_MenuModSettings : Window
    {
        public static SettingsMenuDef activeMenu = null;
        public SettingWindow activeSettingWindow;

        public static Vector2 settingsPosition;
        public static Vector2 modListPosition;
        public static ModContainer SelectedMod = null;

        private readonly List<ModContainer> loadedMods;
        private readonly List<ModContainer> cachedFilteredList;
        private bool pinned = false;
        private static ModContainer prevMod = null;
        private string searchText = "";
        private static Dictionary<string, string> oldValuesCache;
        private bool focusSearchBox = false;
        private readonly float ListWidth = 256f;

        private Texture2D pinTex;

        public XmlExtensions_MenuModSettings()
        {
            loadedMods = new();
            cachedFilteredList = new();
            oldValuesCache = new();
            doCloseButton = true;
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
            doCloseX = true;
            closeOnAccept = false;
        }

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(900f + ListWidth + 6f, 700f);
            }
        }

        public override void PreOpen()
        {
            base.PreOpen();
            pinTex = ContentFinder<Texture2D>.Get("UI/Icons/Pin-Outline", false);
            focusSearchBox = true;
            // activeSettingWindow
            LoadMods();
            FilterModlist();
            if (prevMod != null && prevMod.ToString() != "CharacterEditor") // For compatibility
            {
                foreach (ModContainer mod in loadedMods)
                {
                    if (mod.ToString() == prevMod.ToString())
                    {
                        SetSelectedMod(mod);
                    }
                }
            }
            else
            {
                SetSelectedMod(null);
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect rectSettings = inRect.RightPartPixels(864f).TopPartPixels(inRect.height - 40);
            Rect headerRect = rectSettings.TopPartPixels(40f);
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(headerRect);
            Text.Font = GameFont.Medium;
            listing.verticalSpacing = 0;
            listing.Label(Helpers.TryTranslate("Mod settings for ", "XmlExtensions_ModSettingsFor") + (SelectedMod != null ? SelectedMod.ToString() : "XML Extensions"));
            Text.Font = GameFont.Small;
            listing.GapLine(9f);
            listing.End();
            GUI.BeginGroup(rectSettings);
            Text.Font = GameFont.Small;
            DrawXmlModSettings(rectSettings);
            Text.Font = GameFont.Small;
            GUI.EndGroup();

            string temp = searchText;
            Rect rectMods = inRect.LeftPartPixels(ListWidth).TopPartPixels(inRect.height - 40); //.285f
            GUI.SetNextControlName("searchbox");
            searchText = Widgets.TextField(rectMods.TopPartPixels(22).LeftPartPixels(rectMods.width - 22 - 22), temp);
            GUI.color *= new Color(0.33f, 0.33f, 0.33f);
            if (searchText == null || searchText == "")
            {
                GUI.color = new Color(0.5f, 0.5f, 0.5f);
                GUI.DrawTexture(rectMods.TopPartPixels(22).RightPartPixels(44).LeftPartPixels(22), TexButton.Search);
            }
            else
            {
                if (Widgets.ButtonImage(rectMods.TopPartPixels(22).RightPartPixels(44).LeftPartPixels(22).ContractedBy(2f), TexButton.CloseXSmall))
                {
                    searchText = "";
                }
            }
            if (searchText != temp)
            {
                FilterModlist();
            }
            if (Widgets.ButtonImage(rectMods.TopPartPixels(22).RightPartPixels(22).ContractedBy(2f), pinTex, pinned ? new Color(0.75f, 0.75f, 0.75f) : new Color(0.5f, 0.5f, 0.5f)))
            {
                pinned = !pinned;
                FilterModlist();
            }
            GUI.color = Color.white;
            if (focusSearchBox)
            {
                GUI.FocusControl("searchbox");
                focusSearchBox = false;
            }
            DrawXmlModList(rectMods.BottomPartPixels(rectMods.height - 24));
        }

        private void DrawXmlModList(Rect rect)
        {
            int count = 0;
            foreach (string modId in XmlMod.loadedXmlMods)
            {
                if (XmlMod.settingsPerMod[modId].label != null)
                    count++;
            }
            Rect scrollRect = new Rect(0, 0, rect.width - 20f, Math.Max(cachedFilteredList.Count * (30 + 2) + 38, rect.height + 1));
            Widgets.BeginScrollView(rect, ref modListPosition, scrollRect);
            Listing_Standard listingStandard = new();
            Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
            listingStandard.Begin(rect2);
            int currMod = 0;
            // Draw the button list
            foreach (ModContainer mod in cachedFilteredList)
            {
                // Only draw the button if it is within the viewing window
                if (currMod * 32 + 32 >= modListPosition.y && currMod * 32 <= modListPosition.y + rect.height)
                {
                    if (mod == SelectedMod)
                    {
                        GUI.color = new Color(0.7f, 0.7f, 0.7f);
                    }
                    Rect buttonRect = listingStandard.GetRect(30);
                    if (Widgets.ButtonText(buttonRect, mod.ToString()))
                    {
                        if (Event.current.button == 1)
                        {
                            var newOptions = new List<FloatMenuOption>();
                            if (XmlMod.allSettings.PinnedMods.Contains(mod.ToString()))
                            {
                                newOptions.Add(new FloatMenuOption(Helpers.TryTranslate("Unpin mod", "XmlExtensions_Unpin"), delegate ()
                                {
                                    XmlMod.allSettings.PinnedMods.Remove(mod.ToString());
                                    FilterModlist();
                                    if (cachedFilteredList.Count == 0)
                                    {
                                        pinned = false;
                                        FilterModlist();
                                    }
                                }));
                            }
                            else
                            {
                                newOptions.Add(new FloatMenuOption(Helpers.TryTranslate("Pin mod", "XmlExtensions_Pin"), delegate ()
                                {
                                    XmlMod.allSettings.PinnedMods.Add(mod.ToString());
                                }));
                            }
                            Find.WindowStack.Add(new FloatMenu(newOptions));
                        }
                        else
                        {
                            SetSelectedMod(mod);
                        }
                    }
                    listingStandard.GetRect(2);
                    GUI.color = Color.white;
                }
                else
                {
                    listingStandard.GetRect(32);
                }
                currMod++;
            }
            if (pinned && cachedFilteredList.Count == 0)
            {
                listingStandard.Label(Helpers.TryTranslate("Right-click a mod to pin it", "XmlExtensions_HowToPin"));
            }
            listingStandard.GapLine(4);
            listingStandard.Gap(2);
            if (SelectedMod == null)
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
            }
            if (listingStandard.ButtonText(Helpers.TryTranslate("XML Extensions", "XmlExtensions_Label")))
            {
                SetSelectedMod(null);
            }
            GUI.color = Color.white;
            listingStandard.End();
            Widgets.EndScrollView();
        }

        public void SetSelectedMod(ModContainer mod)
        {
            // Run KeyedActions
            if (SelectedMod != null && SelectedMod.IsXmlMod() && XmlMod.keyedActionListDict.ContainsKey(SelectedMod.modId))
            {
                foreach (string key in XmlMod.keyedActionListDict[SelectedMod.modId].Keys)
                {
                    foreach (KeyedAction action in XmlMod.keyedActionListDict[SelectedMod.modId][key])
                    {
                        if (!action.DoKeyedAction(oldValuesCache[key], SettingsManager.GetSetting(SelectedMod.modId, key)))
                        {
                            ErrorManager.PrintErrors();
                        }
                    }
                }
            }
            oldValuesCache.Clear();
            if (mod != null && mod.IsXmlMod())
            {
                // Cache values for next mod
                if (XmlMod.keyedActionListDict.ContainsKey(mod.modId))
                {
                    foreach (string key in XmlMod.keyedActionListDict[mod.modId].Keys)
                    {
                        oldValuesCache.Add(key, SettingsManager.GetSetting(mod.modId, key));
                    }
                }
                SetActiveMenu(XmlMod.settingsPerMod[mod.modId].homeMenu);
            }
            else
            {
                SetActiveMenu(null);
            }
            if (SelectedMod != null)
            {
                SelectedMod.WriteSettings();
            }
            else
            {
                activeSettingWindow = new MainSettings();
                activeSettingWindow.Init(this);
            }
            SelectedMod = mod;
        }

        private void DrawXmlModSettings(Rect rect)
        {
            rect.x = 0;
            rect.y = 0;
            if (SelectedMod != null)
            {
                if (SelectedMod.IsXmlMod())
                {
                    Rect scrollRect = new Rect(0, 0, rect.width - 20f, activeMenu.CalculateHeight(rect.width - 20f, SelectedMod.modId));
                    Widgets.BeginScrollView(rect.BottomPartPixels(rect.height - 40), ref settingsPosition, scrollRect);
                    Rect rect2 = new Rect(0f, 0f, scrollRect.width, 999999f);
                    activeMenu.DrawSettings(rect2);
                    if (activeMenu.onFrameActions != null)
                    {
                        ErrorManager.ClearErrors();
                        foreach (ActionContainer action in activeMenu.onFrameActions)
                        {
                            if (!action.DoAction())
                            {
                                ErrorManager.PrintErrors();
                            }
                        }
                    }
                    GUI.color = Color.white;
                    Widgets.EndScrollView();
                }
                else
                {
                    SelectedMod.mod.DoSettingsWindowContents(rect.BottomPartPixels(rect.height - 40));
                }
            }
            else
            {
                activeSettingWindow.DrawContents(rect.BottomPartPixels(rect.height - 40f));
            }
        }

        public override void PreClose()
        {
            prevMod = SelectedMod;
            SetSelectedMod(null);
            LoadedModManager.GetMod(typeof(XmlMod)).WriteSettings();
            base.PreClose();
        }

        public void LoadMods()
        {
            loadedMods.Clear();
            foreach (string id in XmlMod.loadedXmlMods)
            {
                loadedMods.Add(new ModContainer(id));
            }
            if (XmlMod.allSettings.standardMods)
            {
                foreach (Mod item in from mod in LoadedModManager.ModHandles
                                     where !mod.SettingsCategory().NullOrEmpty()
                                     select mod)
                {
                    loadedMods.Add(new ModContainer(item));
                }
            }
            loadedMods.Sort();
            FilterModlist();
        }

        private void FilterModlist()
        {
            cachedFilteredList.Clear();
            foreach (ModContainer mod in loadedMods)
            {
                if (searchText == null || searchText == "" || mod.ToString().ToLower().Contains(searchText.ToLower()))
                {
                    if (!pinned || (pinned && XmlMod.allSettings.PinnedMods.Contains(mod.ToString())))
                    {
                        cachedFilteredList.Add(mod);
                    }
                }
            }
        }

        public static void SetActiveMenu(string defName)
        {
            if (activeMenu != null)
            {
                activeMenu.RunPostCloseActions();
            }
            if (defName != null)
            {
                activeMenu = DefDatabase<SettingsMenuDef>.GetNamed(defName);
                activeMenu.RunPreOpenActions();
            }
        }

        public abstract class SettingWindow
        {
            public XmlExtensions_MenuModSettings MoreModSettings = null;

            public virtual void Init(XmlExtensions_MenuModSettings obj)
            {
                MoreModSettings = obj;
            }

            public abstract void DrawContents(Rect rect);
        }

        public class MainSettings : SettingWindow
        {
            public override void DrawContents(Rect rect)
            {
                Listing_Standard listingStandard = new Listing_Standard();
                listingStandard.Begin(rect);
                listingStandard.CheckboxLabeled("XmlExtensions_EnableStackTrace".Translate(), ref XmlMod.allSettings.trace, "XmlExtensions_StackTraceTip".Translate());
                bool b = XmlMod.allSettings.standardMods;
                listingStandard.CheckboxLabeled("XmlExtensions_IncludeStandardMods".Translate(), ref b, "XmlExtensions_IncludeStandardModsTip".Translate());
                if (b != XmlMod.allSettings.standardMods)
                {
                    XmlMod.allSettings.standardMods = b;
                    MoreModSettings.LoadMods();
                }
                XmlMod.allSettings.standardMods = b;
                b = XmlMod.allSettings.mainButton;
                listingStandard.CheckboxLabeled("XmlExtensions_AddMainButton".Translate(), ref XmlMod.allSettings.mainButton, "XmlExtensions_AddMainButtonTip".Translate());
                if (b != XmlMod.allSettings.mainButton)
                {
                    DefDatabase<MainButtonDef>.GetNamed("XmlExtensions_MainButton_ModSettings").buttonVisible = XmlMod.allSettings.mainButton;
                }
                listingStandard.CheckboxLabeled("XmlExtensions_AdvancedDebugging".Translate(), ref XmlMod.allSettings.advancedDebugging, "XmlExtensions_AdvancedDebuggingTip".Translate());
                if (listingStandard.ButtonText("XmlExtensions_ViewUnusedSettings".Translate()))
                {
                    MoreModSettings.activeSettingWindow = new ViewUnusedSettings();
                    MoreModSettings.activeSettingWindow.Init(MoreModSettings);
                }
                if (listingStandard.ButtonText("XmlExtensions_ViewPatchedDefs".Translate()))
                {
                    MoreModSettings.activeSettingWindow = new ViewPatchedDefs();
                    MoreModSettings.activeSettingWindow.Init(MoreModSettings);
                }
                listingStandard.End();
            }
        }

        public class ViewUnusedSettings : SettingWindow
        {
            public string selectedMod = null;
            public Vector2 keyPosition = new();
            public Vector2 settingsPosition = new();

            public override void DrawContents(Rect rect)
            {
                Rect modListRect = rect.LeftPartPixels(300f);
                Rect keyListRect = rect.RightPartPixels(rect.width - 300f - 8f);
                Listing_Standard modListListing = new Listing_Standard();
                modListListing.Begin(modListRect);
                modListListing.Label(Helpers.TryTranslate("Currently inactive mods:", "XmlExtensions_InactiveMods"));
                modListListing.GapLine(4);
                modListListing.Gap(2);
                Rect modRect = modListListing.GetRect(rect.height - 30f);
                Rect scrollRect = new Rect(0, 0, modRect.width - 20f, Math.Max(XmlMod.unusedMods.Count > 0 ? XmlMod.unusedMods.Count * 32 : 30, rect.height - 29f));
                Widgets.BeginScrollView(modRect, ref settingsPosition, scrollRect);
                Rect rect2 = new Rect(0f, 0f, scrollRect.width, 99999f);
                Listing_Standard modListing = new Listing_Standard();
                modListing.Begin(rect2);
                if (XmlMod.unusedMods.Count == 0)
                {
                    modListing.Label(Helpers.TryTranslate("No extra settings at the moment", "XmlExtensions_NoExtraSettings"));
                }
                foreach (string mod in XmlMod.unusedMods)
                {
                    if (mod == selectedMod)
                    {
                        GUI.color = new Color(0.7f, 0.7f, 0.7f);
                    }
                    if (modListing.ButtonText(mod)) { selectedMod = mod; }
                    GUI.color = Color.white;
                }
                modListing.End();
                Widgets.EndScrollView();
                modListListing.End();

                // Draw right column
                if (selectedMod != null)
                {
                    Listing_Standard keyListListing = new Listing_Standard();
                    keyListListing.Begin(keyListRect);
                    keyListListing.Label(Helpers.TryTranslate("Currently selected mod's unused settings:", "XmlExtensions_SelectedModUnusedKeys"));
                    keyListListing.GapLine(4);
                    keyListListing.Gap(2);

                    Rect keyRect = keyListListing.GetRect(rect.height - 30f - 36f);
                    Rect scrollRect2 = new Rect(0, 0, keyRect.width - 20f, Math.Max(XmlMod.unusedSettings[selectedMod].Count * 24f, rect.height - 29f - 36f));
                    Widgets.BeginScrollView(keyRect, ref keyPosition, scrollRect2);
                    rect2 = new Rect(0f, 0f, scrollRect2.width, 99999f);
                    Listing_Standard keyListing = new Listing_Standard();
                    keyListing.Begin(rect2);
                    string tempStr = null;
                    foreach (string key in XmlMod.unusedSettings[selectedMod])
                    {
                        bool del = false;
                        keyListing.CheckboxLabeled(key + ": " + SettingsManager.GetSetting(selectedMod, key), ref del, Helpers.TryTranslate("Delete", "XmlExtensions_Delete"));
                        if (del)
                        {
                            SettingsManager.DeleteSetting(selectedMod, key);
                            tempStr = key;
                        }
                    }
                    if (tempStr != null)
                    {
                        XmlMod.unusedSettings[selectedMod].Remove(tempStr);
                        if (XmlMod.unusedSettings[selectedMod].Count == 0)
                        {
                            XmlMod.unusedMods.Remove(selectedMod);
                            XmlMod.unusedSettings.Remove(selectedMod);
                            selectedMod = null;
                        }
                    }
                    keyListing.End();
                    Widgets.EndScrollView();
                    // Make sure the last key wasn't just deleted (otherwise missing key in dict exception)
                    if (selectedMod != null)
                    {
                        keyListListing.GapLine(4);
                        Rect tempRect = keyListListing.GetRect(32f);
                        Rect firstRect = tempRect.LeftPart(0.495f);
                        Listing_Standard buttonLeftListing = new Listing_Standard();
                        buttonLeftListing.Begin(firstRect);
                        if (buttonLeftListing.ButtonText(Helpers.TryTranslate("Delete {0} keys", "XmlExtensions_DeleteKeys").Replace("{0}", XmlMod.unusedSettings[selectedMod].Count.ToString())))
                        {
                            Find.WindowStack.Add(new Dialog_MessageBox(Helpers.TryTranslate("Are you sure you want to delete every unused key of the current mod?", "XmlExtensions_ConfirmationResetMod"), "Yes".Translate(), delegate ()
                            {
                                foreach (string key in XmlMod.unusedSettings[selectedMod])
                                {
                                    SettingsManager.DeleteSetting(selectedMod, key);
                                }
                                XmlMod.unusedMods.Remove(selectedMod);
                                XmlMod.unusedSettings.Remove(selectedMod);
                                selectedMod = null;
                            }, "No".Translate(), null, null, false, null, null));
                        }
                        buttonLeftListing.End();

                        Rect secondRect = tempRect.RightPart(0.495f);
                        Listing_Standard buttonRightListing = new Listing_Standard();
                        buttonRightListing.Begin(secondRect);
                        GUI.color = Color.red;
                        int count = 0;
                        foreach (List<string> list in XmlMod.unusedSettings.Values)
                        {
                            count += list.Count;
                        }
                        if (buttonRightListing.ButtonText(Helpers.TryTranslate("Delete all {0} unused keys", "XmlExtensions_DeleteAllUnusedKeys").Replace("{0}", count.ToString()), null))
                        {
                            Find.WindowStack.Add(new Dialog_MessageBox(Helpers.TryTranslate("Are you sure you want to delete all unused keys from every mod?", "XmlExtensions_ConfirmResetAll"), "Yes".Translate(), delegate ()
                            {
                                foreach (string mod in XmlMod.unusedMods)
                                {
                                    foreach (string key in XmlMod.unusedSettings[mod])
                                    {
                                        SettingsManager.DeleteSetting(mod, key);
                                    }
                                }
                                XmlMod.unusedSettings.Clear();
                                XmlMod.unusedMods.Clear();
                                selectedMod = null;
                            }, "No".Translate(), null, null, false, null, null));
                        }
                        GUI.color = Color.white;
                        buttonRightListing.End();
                    }
                    keyListListing.End();
                }
            }
        }

        public class ViewPatchedDefs : SettingWindow
        {
            public List<string> defTypes = new();
            public Vector2 scroll1 = new();
            public string selectedDefType = null;
            public List<string> defs = new();
            public Vector2 scroll2 = new();
            public string selectedDef = null;
            public List<ModContentPackContainer> mods = new();
            public Vector2 scroll3 = new();

            public override void Init(XmlExtensions_MenuModSettings obj)
            {
                base.Init(obj);
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

            public override void DrawContents(Rect rect)
            {
                Rect defTypeRect = rect.LeftPart(0.33f);
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
                foreach (string defType in defTypes)
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
                listing2.End();
                Widgets.EndScrollView();
                listing.End();

                Rect defRect = rect.LeftPart(0.66f).RightPartPixels(rect.width * 0.33f);
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
                foreach (string def in defs)
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
                listing2.End();
                Widgets.EndScrollView();
                listing.End();
                Rect modRect = rect.RightPartPixels(rect.width * 0.33f);
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
                        string str = mod.Pack.Name + ":";
                        foreach (Type type in mod.OperationTypes)
                        {
                            str += "\n" + (type?.ToString() ?? "Source");
                        }
                        listing2.Label(str);
                    }
                }
                listing2.End();
                Widgets.EndScrollView();
                listing.End();
            }
        }
    }
}