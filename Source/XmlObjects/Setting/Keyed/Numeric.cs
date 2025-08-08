using UnityEngine;
using Verse;
using System;

namespace XmlExtensions.Setting
{
    internal class Numeric : KeyedSettingContainer
    {
        public float min = -1000000000;
        public float max = 1000000000;
        public int decimals = 0;
        public bool percent = false;
        public Anchor anchor = Anchor.Left;

        private string buf = null;
        private float cachedValue;

        public enum Anchor
        {
            Left = TextAnchor.MiddleLeft,
            Middle = TextAnchor.MiddleCenter,
            Right = TextAnchor.MiddleRight
        }

        protected override bool PreOpen()
        {
            cachedValue = float.Parse(SettingsManager.GetSetting(modId, key));
            buf = Math.Round(cachedValue, decimals).ToString();
            if (buf == "" || buf == null)
                buf = ((float)Math.Round(double.Parse(defaultValue), decimals)).ToString();
            return true;
        }

        protected override float CalculateHeight(float width)
        {
            return 22;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            float f = (float)Math.Round(double.Parse(defaultValue), decimals);
            float currentValue = float.Parse(SettingsManager.GetSetting(modId, key));
            if (cachedValue != currentValue)
            {
                cachedValue = currentValue;
                f = cachedValue;
                buf = f.ToString();
            }
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
            SettingsManager.SetSetting(modId, key, f.ToString());
        }
    }
}