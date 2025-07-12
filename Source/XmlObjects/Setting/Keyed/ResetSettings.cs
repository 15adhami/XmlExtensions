using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class ResetSettings : SettingContainer
    { // Add list of values to set to
        protected string label;
        protected List<string> keys = null;
        protected bool confirm = true;
        public string message;
        public string tKeyMessage;
        public string tKey;
        public string tKeyTip;
        public string tooltip;

        protected override bool Init()
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

        protected override float CalculateHeight(float width)
        {
            return 30;
        }

        protected override void DrawSettingContents(Rect inRect)
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
                        foreach (string key in SettingsManager.GetKeys(modId))
                            SettingsManager.SetSetting(modId, key, SettingsManager.GetDefaultValue(modId, key));
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
                            SettingsManager.SetSetting(modId, key, SettingsManager.GetDefaultValue(modId, key));
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
                            foreach (string key in SettingsManager.GetKeys(modId))
                            {
                                if (XmlMod.allSettings.dataDict.ContainsKey(modId + ";" + key))
                                    SettingsManager.SetSetting(modId, key, SettingsManager.GetDefaultValue(modId, key));
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
                                if (SettingsManager.ContainsKey(modId, key))
                                    SettingsManager.SetSetting(modId, key, SettingsManager.GetDefaultValue(modId, key));
                            }
                        }, "No".Translate(), null, null, false, null, null));
                    }
                }
            }
        }
    }
}