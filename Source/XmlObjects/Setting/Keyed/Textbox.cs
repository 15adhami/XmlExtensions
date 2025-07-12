using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class Textbox : KeyedSettingContainer
    { // Add max char limit
        public string tKey;
        public int lines = 1;
        public Anchor anchor = Anchor.Left;

        public enum Anchor
        {
            Left = TextAnchor.MiddleLeft,
            Middle = TextAnchor.MiddleCenter,
            Right = TextAnchor.MiddleRight
        }

        protected override float CalculateHeight(float width)
        {
            return lines * 22;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            string currStr = SettingsManager.GetSetting(modId, key);
            if (label != null)
            {
                Verse.Text.Anchor = (TextAnchor)anchor;
                Widgets.Label(inRect.LeftHalf(), Helpers.TryTranslate(label, tKey));
                SettingsManager.SetSetting(modId, key, Widgets.TextField(inRect.RightHalf(), currStr));
                Verse.Text.Anchor = TextAnchor.UpperLeft;
            }
            else
            {
                SettingsManager.SetSetting(modId, key, (lines > 1) ? Widgets.TextArea(inRect, currStr) : Widgets.TextField(inRect, currStr));
            }
        }
    }
}