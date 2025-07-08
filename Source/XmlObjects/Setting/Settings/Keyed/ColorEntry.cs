using System;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class ColorEntry : KeyedSettingContainer
    {
        private int r, g, b;
        private string rBuf = "", gBuf = "", bBuf = "";

        protected override float CalculateHeight(float width, string selectedMod)
        {
            float boxHeight = 24;
            float spacing = 6f;
            return 3 * boxHeight + 2 * spacing + GetDefaultSpacing();
        }

        internal override bool PreOpen(string selectedMod)
        {
            try
            {
                Color color = ParseHelper.FromString<Color>(SettingsManager.GetSetting(selectedMod, key));
                r = Mathf.RoundToInt(color.r * 255f);
                g = Mathf.RoundToInt(color.g * 255f);
                b = Mathf.RoundToInt(color.b * 255f);
                rBuf = r.ToString();
                gBuf = g.ToString();
                bBuf = b.ToString();
            }
            catch
            {
                return false;
            }
            return true;
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            float boxHeight = 24;
            float spacing = 6f;

            Color c = ParseHelper.FromString<Color>(defaultValue);
            r = Mathf.RoundToInt(c.r * 255f);
            g = Mathf.RoundToInt(c.g * 255f);
            b = Mathf.RoundToInt(c.b * 255f);

            Rect rRect = new Rect(inRect.x, inRect.y, inRect.width, boxHeight);
            Rect gRect = new Rect(inRect.x, rRect.yMax + spacing, inRect.width, boxHeight);
            Rect bRect = new Rect(inRect.x, gRect.yMax + spacing, inRect.width, boxHeight);

            int rPrev = r;
            int gPrev = g;
            int bPrev = b;

            GUI.SetNextControlName("box1");
            Widgets.TextFieldNumeric(rRect, ref r, ref rBuf, 0, 255);
            GUI.SetNextControlName("box2");
            Widgets.TextFieldNumeric(gRect, ref g, ref gBuf, 0, 255);
            GUI.SetNextControlName("box3");
            Widgets.TextFieldNumeric(bRect, ref b, ref bBuf, 0, 255);

            if (rPrev != r || gPrev != g || bPrev != b)
            {
                Color newColor;
                try
                {
                    newColor = new Color(int.Parse(rBuf) / 255f, int.Parse(gBuf) / 255f, int.Parse(bBuf) / 255f);
                }
                catch
                {
                    newColor = ParseHelper.FromString<Color>(defaultValue);
                }
                SettingsManager.SetSetting(selectedMod, key, newColor.ToString());
            }
        }
    }
}
