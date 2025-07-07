using UnityEngine;
using Verse;
using static HarmonyLib.Code;
using static System.Math;

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

        internal override bool PreOpen(string selectedMod)
        {

            if (allowFloat)
                buf = float.Parse(SettingsManager.GetSetting(selectedMod, key)).ToString();
            else
                buf = ((int)float.Parse(SettingsManager.GetSetting(selectedMod, key))).ToString();
            if (buf == "")
                buf = defaultValue;
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 22 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            float f = float.Parse(defaultValue);
            int i = (int)f;
            if (label != null)
            {
                Rect rect2 = inRect.LeftHalf().Rounded();
                Rect rect3 = inRect.RightHalf().Rounded();
                Verse.Text.Anchor = (TextAnchor)anchor;
                Widgets.Label(rect2, Helpers.TryTranslate(label, tKey));
                Verse.Text.Anchor = TextAnchor.UpperLeft;
                if (allowFloat)
                {
                    Widgets.TextFieldNumeric<float>(rect3, ref f, ref buf, min, max);
                }
                else
                {
                    Widgets.TextFieldNumeric<int>(rect3, ref i, ref buf, min, max);
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
            if (allowFloat)
            {
                if (buf!="" && buf !=null)
                    f = float.Parse(buf);
                SettingsManager.SetSetting(selectedMod, key, f.ToString());
            }
            else
            {
                if (buf != "" && buf != null)
                    i = int.Parse(buf);
                SettingsManager.SetSetting(selectedMod, key, i.ToString());
            }
        }
    }
}