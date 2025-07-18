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

        protected override float CalculateHeight(float width)
        {
            return (label == null ? 0 : 22) + 22;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            Color color = GUI.color;
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.verticalSpacing = 0;
            listingStandard.Begin(inRect);
            float currFloat = float.Parse(SettingsManager.GetSetting(modId, key));
            float newFloat;
            if (label != null)
            {
                string substiteLegacy = Helpers.SubstituteVariable(Helpers.TryTranslate(label, tKey), key, currFloat.ToString(), "{}");
                listingStandard.Label(Helpers.SubstituteVariable(substiteLegacy, "key", currFloat.ToString(), "{}"));
            }
            newFloat = listingStandard.Slider(currFloat, min, max);
            listingStandard.End();
            SettingsManager.SetSetting(modId, key, Math.Round(newFloat, decimals).ToString());
            GUI.color = color;
        }
    }
}