﻿using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class ResetSettings : SettingContainer
    {
        protected string label;
        protected List<string> keys = null;
        protected bool confirm = true;
        public string message;
        public string tKeyMessage;
        public string tKey;
        public string tKeyTip;
        public string tooltip;

        protected override bool Init(string selectedMod)
        {
            if (label == null)
            {
                label = "Reset Settings";
                tKey = "XmlExtensions_ResetSettings";
            }
            if (message == null)
            {
                tKeyMessage = "XmlExtensions_Confirmation";
                message = "Are you sure?";
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 30 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            if (!confirm)
            {
                if (keys == null)
                {
                    if (!tooltip.NullOrEmpty())
                    {
                        TooltipHandler.TipRegion(inRect, Helpers.TryTranslate(tooltip, tKeyTip));
                    }
                    if (Widgets.ButtonText(inRect, Helpers.TryTranslate(label, tKey)))
                    {
                        foreach (string key in SettingsManager.GetKeys(selectedMod))
                            SettingsManager.SetSetting(selectedMod, key, SettingsManager.GetDefaultValue(selectedMod, key));
                    }
                }
                else
                {
                    if (!tooltip.NullOrEmpty())
                    {
                        TooltipHandler.TipRegion(inRect, Helpers.TryTranslate(tooltip, tKeyTip));
                    }
                    if (Widgets.ButtonText(inRect, Helpers.TryTranslate(label, tKey)))
                    {
                        foreach (string key in keys)
                            SettingsManager.SetSetting(selectedMod, key, SettingsManager.GetDefaultValue(selectedMod, key));
                    }
                }
            }
            else
            {
                if (keys == null)
                {
                    if (!tooltip.NullOrEmpty())
                    {
                        TooltipHandler.TipRegion(inRect, Helpers.TryTranslate(tooltip, tKeyTip));
                    }
                    if (Widgets.ButtonText(inRect, Helpers.TryTranslate(label, tKey)))
                    {
                        Find.WindowStack.Add(new Dialog_MessageBox(Helpers.TryTranslate(message, tKeyMessage), "Yes".Translate(), delegate ()
                        {
                            foreach (string key in SettingsManager.GetKeys(selectedMod))
                            {
                                if (XmlMod.allSettings.dataDict.ContainsKey(selectedMod + ";" + key))
                                    SettingsManager.SetSetting(selectedMod, key, SettingsManager.GetDefaultValue(selectedMod, key));
                            }
                        }, "No".Translate(), null, null, false, null, null));
                    }
                }
                else
                {
                    if (!tooltip.NullOrEmpty())
                    {
                        TooltipHandler.TipRegion(inRect, Helpers.TryTranslate(tooltip, tKeyTip));
                    }
                    if (Widgets.ButtonText(inRect, Helpers.TryTranslate(label, tKey)))
                    {
                        Find.WindowStack.Add(new Dialog_MessageBox(Helpers.TryTranslate(message, tKeyMessage), "Yes".Translate(), delegate ()
                        {
                            foreach (string key in keys)
                            {
                                if (SettingsManager.ContainsKey(selectedMod, key))
                                    SettingsManager.SetSetting(selectedMod, key, SettingsManager.GetDefaultValue(selectedMod, key));
                            }
                        }, "No".Translate(), null, null, false, null, null));
                    }
                }
            }
        }
    }
}