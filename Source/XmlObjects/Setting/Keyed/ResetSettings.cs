using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class ResetSettings : SettingContainer
    { // Add list of values to set to
        protected string label;
        protected bool confirm = true;
        protected List<string> keys = null;
        protected List<string> values = null;
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
            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(inRect, Helpers.TryTranslate(tooltip, tKeyTip));
            }
            if (!confirm)
            {
                if (Widgets.ButtonText(inRect, Helpers.TryTranslate(label, tKey)))
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
            }
            else
            {
                if (Widgets.ButtonText(inRect, Helpers.TryTranslate(label, tKey)))
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