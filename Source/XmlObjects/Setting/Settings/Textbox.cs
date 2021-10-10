using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class Textbox : KeyedSettingContainer
    {
        public string tKey;
        public int lines = 1;

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return lines * 22 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            string currStr = SettingsManager.GetSetting(selectedMod, key);
            if (label != null)
            {
                SettingsManager.SetSetting(selectedMod, key, Widgets.TextEntryLabeled(inRect, Helpers.TryTranslate(label, tKey), currStr));
            }
            else
            {
                SettingsManager.SetSetting(selectedMod, key, (lines > 1) ? Widgets.TextArea(inRect, currStr) : Widgets.TextField(inRect, currStr));
            }
        }
    }
}