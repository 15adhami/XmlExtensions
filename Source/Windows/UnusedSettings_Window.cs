using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions
{
    internal class UnusedSettings_Window : Window
    {
        public string selectedMod = null;
        public Vector2 keyPosition = new();
        public Vector2 settingsPosition = new();
        public override Vector2 InitialSize => new Vector2(900, 740);

        public UnusedSettings_Window()
        {
            doCloseButton = true;
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
            doCloseX = true;
            closeOnAccept = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect rect = inRect.TopPartPixels(inRect.height - 40f);
            Rect modListRect = rect.LeftPartPixels(300f);
            Rect keyListRect = rect.RightPartPixels(rect.width - 300f - 8f);
            Listing_Standard modListListing = new Listing_Standard();
            modListListing.Begin(modListRect);
            modListListing.Label("Currently inactive mods:".TranslateIfTKeyAvailable("XmlExtensions_InactiveMods"));
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
                modListing.Label("No extra settings at the moment".TranslateIfTKeyAvailable("XmlExtensions_NoExtraSettings"));
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
                keyListListing.Label("Currently selected mod's unused settings:".TranslateIfTKeyAvailable("XmlExtensions_SelectedModUnusedKeys"));
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
                    keyListing.CheckboxLabeled(key + ": " + SettingsManager.GetSetting(selectedMod, key), ref del, "Delete".TranslateIfTKeyAvailable("XmlExtensions_Delete"));
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
                    if (buttonLeftListing.ButtonText("Delete {0} keys".TranslateIfTKeyAvailable("XmlExtensions_DeleteKeys").Replace("{0}", XmlMod.unusedSettings[selectedMod].Count.ToString())))
                    {
                        Find.WindowStack.Add(new Dialog_MessageBox("Are you sure you want to delete every unused key of the current mod?".TranslateIfTKeyAvailable("XmlExtensions_ConfirmationResetMod"), "Yes".Translate(), delegate ()
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
                    if (buttonRightListing.ButtonText("Delete all {0} unused keys".TranslateIfTKeyAvailable("XmlExtensions_DeleteAllUnusedKeys").Replace("{0}", count.ToString()), null))
                    {
                        Find.WindowStack.Add(new Dialog_MessageBox("Are you sure you want to delete all unused keys from every mod?".TranslateIfTKeyAvailable("XmlExtensions_ConfirmResetAll"), "Yes".Translate(), delegate ()
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
}
