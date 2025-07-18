﻿using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace XmlExtensions.Setting
{
    internal class IntEntry : KeyedSettingContainer
    {
        public int multiplier = 1;
        public string min;
        public string max;

        protected override float CalculateHeight(float width)
        {
            return 24;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            int f = int.Parse(SettingsManager.GetSetting(modId, key));

            if (min != null && f < int.Parse(min))
                f = int.Parse(min);
            if (max != null && f > int.Parse(max))
                f = int.Parse(max);
            string editBuffer = f.ToString();
            int value = f;
            Rect rect = inRect.TopPartPixels(24f);
            int num = Mathf.Min(40, (int)rect.width / 5);
            if (Widgets.ButtonText(new Rect(rect.xMin, rect.yMin, (float)num, rect.height), (-10 * multiplier).ToStringCached(), true, true, true))
            {
                value -= 10 * multiplier * GenUI.CurrentAdjustmentMultiplier();
                editBuffer = value.ToStringCached();
                SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
            }
            if (Widgets.ButtonText(new Rect(rect.xMin + (float)num, rect.yMin, (float)num, rect.height), (-1 * multiplier).ToStringCached(), true, true, true))
            {
                value -= multiplier * GenUI.CurrentAdjustmentMultiplier();
                editBuffer = value.ToStringCached();
                SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
            }
            if (Widgets.ButtonText(new Rect(rect.xMax - (float)num, rect.yMin, (float)num, rect.height), "+" + (10 * multiplier).ToStringCached(), true, true, true))
            {
                value += 10 * multiplier * GenUI.CurrentAdjustmentMultiplier();
                editBuffer = value.ToStringCached();
                SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
            }
            if (Widgets.ButtonText(new Rect(rect.xMax - (float)(num * 2), rect.yMin, (float)num, rect.height), "+" + multiplier.ToStringCached(), true, true, true))
            {
                value += multiplier * GenUI.CurrentAdjustmentMultiplier();
                editBuffer = value.ToStringCached();
                SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
            }
            Widgets.TextFieldNumeric<int>(new Rect(rect.xMin + (float)(num * 2), rect.yMin, rect.width - (float)(num * 4), rect.height), ref value, ref editBuffer, -9999999f, 1E+09f);
            SettingsManager.SetSetting(modId, key, value.ToString());
        }
    }
}