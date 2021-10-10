using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class Numeric : KeyedSettingContainer
    {
        public float min;
        public float max;
        public string tKey;

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 22 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            float f = float.Parse(SettingsManager.GetSetting(selectedMod, key));
            string buf = f.ToString();
            Widgets.TextFieldNumericLabeled(inRect, Helpers.TryTranslate(label, tKey), ref f, ref buf, min, max);
            SettingsManager.SetSetting(selectedMod, key, f.ToString());
        }
    }
}