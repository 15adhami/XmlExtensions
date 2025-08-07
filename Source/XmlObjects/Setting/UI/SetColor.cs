using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class SetColor : SettingContainer
    {
        protected new Color color = Color.white;

        protected override bool Init()
        {
            addDefaultSpacing = false;
            return true;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            GUI.color = (key != null) ? ParseHelper.FromString<Color>(SettingsManager.GetSetting(modId, key)) : color;
        }
    }
}