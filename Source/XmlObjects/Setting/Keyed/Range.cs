using System;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{ // TODO: Allow Range to be colored
    internal class Range : KeyedSettingContainer
    {
        public int min;
        public int max;
        public string key2;
        public int id = 0;
        public int decimals = 0;

        protected override bool Init()
        {
            id = SettingsManager.rangeCount;
            SettingsManager.rangeCount++;
            return true;
        }

        protected override bool SetDefaultValue()
        {
            if (key == null)
            {
                Error("<key> is null");
                return false;
            }
            else if (tag != null)
            {
                menuDef.AddTag(tag, this);
            }
            if (key2 == null)
            {
                SettingsManager.SetDefaultValue(modId, key, defaultValue);
            }
            else
            {
                SettingsManager.SetDefaultValue(modId, key, defaultValue.Split('~')[0]);
                SettingsManager.SetDefaultValue(modId, key2, defaultValue.Split('~')[1]);
            }
            return true;
        }

        protected override float CalculateHeight(float width)
        {
            return 28;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            Color color = GUI.color;
            if (key2 == null)
            {
                FloatRange range = Verse.FloatRange.FromString(SettingsManager.GetSetting(modId, key));
                Widgets.FloatRange(inRect, id, ref range, min, max, null);
                SettingsManager.SetSetting(modId, key, Math.Round(range.min, decimals).ToString() + "~" + Math.Round(range.max, decimals).ToString());
            }
            else
            {
                FloatRange range = Verse.FloatRange.FromString(SettingsManager.GetSetting(modId, key) + "~" + SettingsManager.GetSetting(modId, key2));
                Widgets.FloatRange(inRect, id, ref range, min, max, null);
                SettingsManager.SetSetting(modId, key, Math.Round(range.min, decimals).ToString());
                SettingsManager.SetSetting(modId, key2, Math.Round(range.max, decimals).ToString());
            }
            GUI.color = color;
        }
    }
}