using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class SetColor : SettingContainer
    {
        protected Color color = Color.white;
        public string key;

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            if (key != null)
            {
                color = ParseHelper.FromString<Color>(SettingsManager.GetSetting(selectedMod, key));
            }
            GUI.color = color;
        }
    }
}