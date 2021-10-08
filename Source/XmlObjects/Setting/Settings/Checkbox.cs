using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class Checkbox : KeyedSettingContainer
    {
        public string tooltip;
        public string tKey;
        public string tKeyTip;
        public bool highlight = true;

        protected override bool Init(string selectedMod)
        {
            if (tooltip != null)
            {
                highlight = true;
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 22 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            bool currBool = bool.Parse(SettingsManager.GetSetting(selectedMod, key));            
            if (highlight && Mouse.IsOver(inRect))
            {
                Widgets.DrawHighlight(inRect);
            }
            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(inRect, Helpers.TryTranslate(tooltip, tKeyTip));
            }
            Widgets.CheckboxLabeled(inRect, Helpers.TryTranslate(label, tKey), ref currBool);
            SettingsManager.SetSetting(selectedMod, key, currBool.ToString());
        }
    }
}
