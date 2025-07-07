using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace XmlExtensions.Setting
{
    internal class Numeric : KeyedSettingContainer
    {
        public float min;
        public float max;
        public string tKey;
        public bool allowFloat = false;
        public Anchor anchor = Anchor.Left;

        private string buf = null;

        public enum Anchor
        {
            Left = TextAnchor.MiddleLeft,
            Middle = TextAnchor.MiddleCenter,
            Right = TextAnchor.MiddleRight
        }

        protected override bool Init(string selectedMod)
        {

            if (allowFloat)
                buf = float.Parse(SettingsManager.GetSetting(selectedMod, key)).ToString();
            else
                buf = ((int)float.Parse(SettingsManager.GetSetting(selectedMod, key))).ToString();
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 22 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            float f = 0;
            int i = 0;
            if (label != null)
            {
                Rect rect2 = inRect.LeftHalf().Rounded();
                Rect rect3 = inRect.RightHalf().Rounded();
                Verse.Text.Anchor = (TextAnchor)anchor;
                Widgets.Label(rect2, Helpers.TryTranslate(label, tKey));
                Verse.Text.Anchor = TextAnchor.UpperLeft;
                if (allowFloat)
                {
                    Widgets.TextFieldNumeric<float>(inRect, ref f, ref buf, min, max);
                }
                else
                {
                    Widgets.TextFieldNumeric<int>(inRect, ref i, ref buf, min, max);
                }
            }
            else
            {
                if (allowFloat)
                {
                    Widgets.TextFieldNumeric<float>(inRect, ref f, ref buf, min, max);
                }
                else
                {
                    Widgets.TextFieldNumeric<int>(inRect, ref i, ref buf, min, max);
                }
            }
            SettingsManager.SetSetting(selectedMod, key, f.ToString());
        }
    }
}