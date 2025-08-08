using System;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class ColorEntry : KeyedSettingContainer
    {
        public float spacing = 2;
        public float textGap = 12;
        public bool colorLabels = false;

        private int r, g, b;
        private string rBuf = "", gBuf = "", bBuf = "";

        protected override float CalculateHeight(float width)
        {
            float boxHeight = 24;
            return 3 * boxHeight + 2 * spacing;
        }

        protected override bool PreOpen()
        {
            try
            {
                Color color = ParseHelper.FromString<Color>(SettingsManager.GetSetting(modId, key));
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

        protected override void DrawSettingContents(Rect inRect)
        {
            float boxHeight = 24;

            // Convert default value to color
            Color c = ParseHelper.FromString<Color>(defaultValue);
            r = Mathf.RoundToInt(c.r * 255f);
            g = Mathf.RoundToInt(c.g * 255f);
            b = Mathf.RoundToInt(c.b * 255f);

            // Label width
            float labelWidth = Verse.Text.CalcSize("R:").x;

            // Define rects
            Rect rRow = new(inRect.x, inRect.y, inRect.width, boxHeight);
            Rect gRow = new(inRect.x, rRow.yMax + spacing, inRect.width, boxHeight);
            Rect bRow = new(inRect.x, gRow.yMax + spacing, inRect.width, boxHeight);

            Rect rLabel = new(rRow.x, rRow.y + 2, labelWidth, boxHeight);
            Rect gLabel = new(gRow.x, gRow.y + 2, labelWidth, boxHeight);
            Rect bLabel = new(bRow.x, bRow.y + 2, labelWidth, boxHeight);

            Rect rField = new(rLabel.xMax + textGap, rRow.y, inRect.width - (labelWidth + textGap), boxHeight);
            Rect gField = new(gLabel.xMax + textGap, gRow.y, inRect.width - (labelWidth + textGap), boxHeight);
            Rect bField = new(bLabel.xMax + textGap, bRow.y, inRect.width - (labelWidth + textGap), boxHeight);

            int rPrev = r, gPrev = g, bPrev = b;

            // Labels
            if (colorLabels)
            {
                Color temp = GUI.color;
                GUI.color = Color.red;
                Widgets.Label(rLabel, Helpers.TryTranslate("R", "XmlExtensions_R") + ":");
                GUI.color = Color.green;
                Widgets.Label(gLabel, Helpers.TryTranslate("G", "XmlExtensions_G") + ":");
                GUI.color = Color.blue;
                Widgets.Label(bLabel, Helpers.TryTranslate("B", "XmlExtensions_B") + ":");
                GUI.color = temp;
            }
            else
            {
                Widgets.Label(rLabel, Helpers.TryTranslate("R", "XmlExtensions_R") + ":");
                Widgets.Label(gLabel, Helpers.TryTranslate("G", "XmlExtensions_G") + ":");
                Widgets.Label(bLabel, Helpers.TryTranslate("B", "XmlExtensions_B") + ":");
            }

            // Fields
            GUI.SetNextControlName("box1");
            Widgets.TextFieldNumeric(rField, ref r, ref rBuf, 0, 255);
            GUI.SetNextControlName("box2");
            Widgets.TextFieldNumeric(gField, ref g, ref gBuf, 0, 255);
            GUI.SetNextControlName("box3");
            Widgets.TextFieldNumeric(bField, ref b, ref bBuf, 0, 255);

            // Update setting if changed
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
                SettingsManager.SetSetting(modId, key, newColor.ToString());
            }

            // Sync buffers in case external value was updated
            Color color = ParseHelper.FromString<Color>(SettingsManager.GetSetting(modId, key));
            r = Mathf.RoundToInt(color.r * 255f);
            g = Mathf.RoundToInt(color.g * 255f);
            b = Mathf.RoundToInt(color.b * 255f);
            rBuf = r.ToString();
            gBuf = g.ToString();
            bBuf = b.ToString();
        }
    }
}
