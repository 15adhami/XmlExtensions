using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class Checkbox : KeyedSettingContainer
    {
        public string tooltip;
        public string tKey;
        public string tKeyTip;

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 22 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            // TODO: Add tooltip
            bool currBool = bool.Parse(SettingsManager.GetSetting(selectedMod, key));
            Widgets.CheckboxLabeled(inRect, Helpers.TryTranslate(label, tKey), ref currBool);
            SettingsManager.SetSetting(selectedMod, key, currBool.ToString());
        }        
    }
}
