using UnityEngine;
using Verse;
using System;

namespace XmlExtensions.Setting
{
    internal class Numeric : KeyedSettingContainer
    {
        public float min;
        public float max;
        public string tKey;
        public int decimals = 0;
        public bool percent = false;
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

            buf = Math.Round(float.Parse(SettingsManager.GetSetting(selectedMod, key)), decimals).ToString();
            if (buf == "" || buf == null)
                buf = ((float)Math.Round(double.Parse(defaultValue), decimals)).ToString();
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 22 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            float f = (float)Math.Round(double.Parse(defaultValue), decimals);
            int i = (int)f;
            if (buf != null && buf != "")
            {
                try
                {
                    if (buf[buf.Length - 1] != '.' && buf[0] != '.' && !(buf[0] == '-' && buf[1] == '.'))
                        buf = Math.Round(float.Parse(buf), decimals).ToString();
                    if (decimals == 0)
                        buf = buf.Replace(".","");
                }
                catch { }
            }
            if (label != null)
            {
                Rect rect2 = inRect.LeftHalf().Rounded();
                Rect rect3 = inRect.RightHalf().Rounded();
                Verse.Text.Anchor = (TextAnchor)anchor;
                Widgets.Label(rect2, Helpers.TryTranslate(label, tKey));
                Verse.Text.Anchor = TextAnchor.UpperLeft;
                if (percent)
                    Widgets.TextFieldPercent(rect3, ref f, ref buf, min, max);
                else
                {
                    if (decimals == 0)
                        Widgets.TextFieldNumeric(rect3, ref i, ref buf, min, max);
                    else
                        Widgets.TextFieldNumeric(rect3, ref f, ref buf, min, max);
                }
            }
            else
            {
                if (percent)
                    Widgets.TextFieldPercent(inRect, ref f, ref buf, min, max);
                else
                {
                    if (decimals == 0)
                        Widgets.TextFieldNumeric(inRect, ref i, ref buf, min, max);
                    else
                        Widgets.TextFieldNumeric(inRect, ref f, ref buf, min, max);
                }
            }
            try
            {
                f = (float)Math.Round(double.Parse(buf), decimals);
            }
            catch { }
            SettingsManager.SetSetting(selectedMod, key, f.ToString());
        }
    }
}