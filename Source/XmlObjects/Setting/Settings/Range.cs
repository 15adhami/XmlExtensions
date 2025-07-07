using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace XmlExtensions.Setting
{
    internal class Range : KeyedSettingContainer
    {
        public int min;
        public int max;
        public string key2;
        public int id = 0;
        public int decimals = 0;

        private Color cColor;

        protected override bool Init(string selectedMod)
        {
            id = PatchManager.rangeCount;
            PatchManager.rangeCount++;
            return true;
        }

        protected override bool SetDefaultValue(string selectedMod)
        {
            if (key == null)
            {
                Error("<key> is null");
                return false;
            }
            if (key2 == null)
            {
                SettingsManager.SetDefaultValue(selectedMod, key, defaultValue);
            }
            else
            {
                SettingsManager.SetDefaultValue(selectedMod, key, defaultValue.Split('~')[0]);
                SettingsManager.SetDefaultValue(selectedMod, key2, defaultValue.Split('~')[1]);
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 28 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            cColor = GUI.color;
            Rect rect = inRect.TopPartPixels(28f);
            if (key2 == null)
            {
                FloatRange range = Verse.FloatRange.FromString(SettingsManager.GetSetting(selectedMod, key));
                Widgets.FloatRange(rect, id, ref range, min, max, null);
                SettingsManager.SetSetting(selectedMod, key, Math.Round(range.min, decimals).ToString() + "~" + Math.Round(range.max, decimals).ToString());
            }
            else
            {
                FloatRange range = Verse.FloatRange.FromString(SettingsManager.GetSetting(selectedMod, key) + "~" + SettingsManager.GetSetting(selectedMod, key2));
                Widgets.FloatRange(rect, id, ref range, min, max, null);
                SettingsManager.SetSetting(selectedMod, key, Math.Round(range.min, decimals).ToString());
                SettingsManager.SetSetting(selectedMod, key2, Math.Round(range.max, decimals).ToString());
            }
            GUI.color = cColor;
        }
    }
}