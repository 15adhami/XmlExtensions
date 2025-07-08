using System;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class Slider : KeyedSettingContainer
    {
        public float min;
        public float max;
        public string tKey;
        public int decimals = 6;

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return (label == null ? 0 : 22) + 22 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.verticalSpacing = 0;
            listingStandard.Begin(inRect);
            float currFloat = float.Parse(SettingsManager.GetSetting(selectedMod, key));
            float newFloat;
            if (label != null)
            {
                listingStandard.Label(Helpers.SubstituteVariable(Helpers.TryTranslate(label, tKey), key, currFloat.ToString(), "{}"));
            }
            newFloat = listingStandard.Slider(currFloat, min, max);
            listingStandard.End();
            SettingsManager.SetSetting(selectedMod, key, Math.Round(newFloat, decimals).ToString());
        }
    }
}