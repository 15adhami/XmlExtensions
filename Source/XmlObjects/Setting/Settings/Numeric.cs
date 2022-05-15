using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class Numeric : KeyedSettingContainer
    {
        public float min;
        public float max;
        public string tKey;
        public Anchor anchor = Anchor.Left;

        public enum Anchor
        {
            Left = TextAnchor.MiddleLeft,
            Middle = TextAnchor.MiddleCenter,
            Right = TextAnchor.MiddleRight
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 22 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            float f = float.Parse(SettingsManager.GetSetting(selectedMod, key));
            string buf = f.ToString();
            if (label != null)
            {
                Rect rect2 = inRect.LeftHalf().Rounded();
                Rect rect3 = inRect.RightHalf().Rounded();
                Verse.Text.Anchor = (TextAnchor)anchor;
                Widgets.Label(rect2, Helpers.TryTranslate(label, tKey));
                Verse.Text.Anchor = TextAnchor.UpperLeft;
                Widgets.TextFieldNumeric(rect3, ref f, ref buf, min, max);
            }
            else
            {
                Widgets.TextFieldNumeric(inRect, ref f, ref buf, min, max);
            }
            SettingsManager.SetSetting(selectedMod, key, f.ToString());
        }
    }
}