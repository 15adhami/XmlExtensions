using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class Checkbox : KeyedSettingContainer
    {
        public string tooltip;
        public string tKey;
        public string tKeyTip;
        public bool highlight = true;

        protected override bool Init()
        {
            if (tooltip != null)
            {
                highlight = true;
            }
            return true;
        }

        protected override float CalculateHeight(float width)
        {
            return 22;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            bool currBool = bool.Parse(SettingsManager.GetSetting(modId, key));
            if (highlight && Mouse.IsOver(inRect))
            {
                Widgets.DrawHighlight(inRect);
            }
            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(inRect, Helpers.TryTranslate(tooltip, tKeyTip));
            }
            Widgets.CheckboxLabeled(inRect, Helpers.TryTranslate(label, tKey), ref currBool);
            SettingsManager.SetSetting(modId, key, currBool.ToString());
        }
    }
}