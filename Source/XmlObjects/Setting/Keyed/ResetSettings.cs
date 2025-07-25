﻿using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class ResetSettings : SettingContainer
    {
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
            if (message == null)
            {
                message = "Are you sure?";
                tKeyMessage ??= "XmlExtensions_Confirmation";
            }
            if (label == null)
            {
                label = "Reset Settings";
                tKey ??= "XmlExtensions_ResetSettings";
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
            if (Widgets.ButtonText(inRect, Helpers.TryTranslate(label, tKey)))
            {
                if (!confirm)
                {
                    DoResetSettings();
                }
                else
                {
                    Find.WindowStack.Add(new Dialog_MessageBox(Helpers.TryTranslate(message, tKeyMessage), "Yes".Translate(), delegate ()
                    {
                        DoResetSettings();
                    }, "No".Translate(), null, null, false, null, null));
                }
            }
        }

        private void DoResetSettings()
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
        }
    }
}