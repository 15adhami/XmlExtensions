using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class ResetSettings : SettingContainer
    { // Add list of values to set to
        protected string label = "Reset Settings";
        protected bool confirm = true;
        protected List<string> keys = null;
        protected List<string> values = null;
        public string message = "Are you sure?";
        public string tKeyMessage = "XmlExtensions_Confirmation";
        public string tKey = "XmlExtensions_ResetSettings";
        public string tKeyTip;
        public string tooltip;

        protected override float CalculateHeight(float width)
        {
            return 30;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(inRect, Helpers.TryTranslate(tooltip, tKeyTip));
            }
            if (Widgets.ButtonText(inRect, Helpers.TryTranslate(label, tKey)))
            {
                if (!confirm)
                {
                    if (keys == null)
                    {
                        foreach (string key in SettingsManager.GetKeys(modId))
                            SettingsManager.SetSetting(modId, key, SettingsManager.GetDefaultValue(modId, key));
                    }
                    else if (values != null)
                    {
                        for (int i = 0; i < keys.Count; i++)
                            SettingsManager.SetSetting(modId, keys[i], values[i]);
                    }
                    else
                    {
                        foreach (string key in keys)
                            SettingsManager.SetSetting(modId, key, SettingsManager.GetDefaultValue(modId, key));
                    }
                }
                else
                {
                    Find.WindowStack.Add(new Dialog_MessageBox(Helpers.TryTranslate(message, tKeyMessage), "Yes".Translate(), delegate ()
                    {
                        if (keys == null)
                        {
                            foreach (string key in SettingsManager.GetKeys(modId))
                            {
                                SettingsManager.SetSetting(modId, key, SettingsManager.GetDefaultValue(modId, key));
                            }
                        }
                        else if (values != null)
                        {
                            for (int i = 0; i < keys.Count; i++)
                            {
                                SettingsManager.SetSetting(modId, keys[i], values[i]);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < keys.Count; i++)
                            {
                                SettingsManager.SetSetting(modId, keys[i], SettingsManager.GetDefaultValue(modId, keys[i]));
                            }
                        }
                    }, "No".Translate(), null, null, false, null, null));
                }
            }
        }
    }
}