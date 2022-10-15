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
                DrawXmlExtensionsSettings(rect.BottomPartPixels(rect.height - 40f));
            }
        }

        public override void PreClose()
        {
            prevMod = SelectedMod;
            SetSelectedMod(null);
            LoadedModManager.GetMod(typeof(XmlMod)).WriteSettings();
            base.PreClose();
        }

        private void LoadMods()
        {
            loadedMods.Clear();
            foreach (string id in XmlMod.loadedXmlMods)
            {
                if (XmlMod.settingsPerMod[id].label != null)
                {
                    loadedMods.Add(new ModContainer(id));
                }
            }
            if (XmlMod.allSettings.standardMods)
            {
                foreach (Mod item in from mod in LoadedModManager.ModHandles
                                     where !mod.SettingsCategory().NullOrEmpty()
                                     select mod)
                {
                    if (item.GetType() != typeof(XmlMod))
                    {
                        loadedMods.Add(new ModContainer(item));
                    }
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

        private void DrawXmlExtensionsSettings(Rect rect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            listingStandard.CheckboxLabeled("XmlExtensions_EnableStackTrace".Translate(), ref XmlMod.allSettings.trace, "XmlExtensions_StackTraceTip".Translate());
            bool b = XmlMod.allSettings.standardMods;
            listingStandard.CheckboxLabeled("XmlExtensions_IncludeStandardMods".Translate(), ref b, "XmlExtensions_IncludeStandardModsTip".Translate());
            if (b != XmlMod.allSettings.standardMods)
            {
                XmlMod.allSettings.standardMods = b;
                LoadMods();
            }
            XmlMod.allSettings.standardMods = b;
            b = XmlMod.allSettings.mainButton;
            listingStandard.CheckboxLabeled("XmlExtensions_AddMainButton".Translate(), ref XmlMod.allSettings.mainButton, "XmlExtensions_AddMainButtonTip".Translate());
            if (b != XmlMod.allSettings.mainButton)
            {
                DefDatabase<MainButtonDef>.GetNamed("XmlExtensions_MainButton_ModSettings").buttonVisible = XmlMod.allSettings.mainButton;
            }
            listingStandard.CheckboxLabeled("XmlExtensions_AdvancedDebugging".Translate(), ref XmlMod.allSettings.advancedDebugging, "XmlExtensions_AdvancedDebuggingTip".Translate());
            Rect buttonRect = listingStandard.GetRect(30f);
            Listing_Standard listingStandard2 = new();
            listingStandard2.Begin(buttonRect.LeftHalf().LeftPartPixels(buttonRect.LeftHalf().width - 3));
            if (listingStandard2.ButtonText("XmlExtensions_ViewUnusedSettings".Translate()))
            {
                Find.WindowStack.Add(new UnusedSettings_Window());
            }
            listingStandard2.End();
            listingStandard2 = new();
            listingStandard2.Begin(buttonRect.RightHalf().RightPartPixels(buttonRect.RightHalf().width - 3));
            if (listingStandard2.ButtonText("XmlExtensions_PatchAnalyzer".Translate()))
            {
                Find.WindowStack.Add(new PatchAnalyzer_Window());
            }
            listingStandard2.End();
            listingStandard.End();
        }
    }
}