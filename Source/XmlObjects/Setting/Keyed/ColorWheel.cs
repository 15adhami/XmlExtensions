using System;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class ColorWheel : KeyedSettingContainer
    {
        public float wheelSize = 128;
        public float brightnessWidth = 12;
        public float gapSize = 16;
        public Location brightnessLocation = Location.Left;
        public Anchor anchor = Anchor.Middle;

        public enum Location
        {
            Left,
            Right,
            Top,
            Bottom
        }

        public enum Anchor
        {
            Left,
            Right,
            Middle
        }

        private bool currentlyDraggingWheel = false;
        private bool currentlyDraggingBar = false;

        protected override float CalculateHeight(float width, string selectedMod)
        {
            float size = wheelSize;
            if (brightnessLocation == Location.Top || brightnessLocation == Location.Bottom)
            {
                size += brightnessWidth + gapSize;
            }
            return size + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            Color color_temp = GUI.color;
            Color color = ParseHelper.FromString<Color>(SettingsManager.GetSetting(selectedMod, key));

            Rect rectWheel, rectBrightness;

            float totalWidth = inRect.width;
            float totalHeight = inRect.height;

            float layoutWidth = (brightnessLocation == Location.Left || brightnessLocation == Location.Right)
                ? wheelSize + brightnessWidth + gapSize
                : wheelSize;
            float layoutHeight = (brightnessLocation == Location.Top || brightnessLocation == Location.Bottom)
                ? wheelSize + brightnessWidth + gapSize
                : wheelSize;

            float offsetX = anchor switch
            {
                Anchor.Left => 0f,
                Anchor.Right => totalWidth - layoutWidth,
                Anchor.Middle => (totalWidth - layoutWidth) / 2f,
                _ => 0f
            };

            float offsetY = (totalHeight - layoutHeight) / 2f;

            Rect layoutRect = new Rect(inRect.x + offsetX, inRect.y + offsetY, layoutWidth, layoutHeight);

            switch (brightnessLocation)
            {
                case Location.Left:
                    rectBrightness = new Rect(layoutRect.x, layoutRect.y, brightnessWidth, wheelSize);
                    rectWheel = new Rect(rectBrightness.xMax + gapSize, layoutRect.y, wheelSize, wheelSize);
                    break;
                case Location.Right:
                    rectWheel = new Rect(layoutRect.x, layoutRect.y, wheelSize, wheelSize);
                    rectBrightness = new Rect(rectWheel.xMax + gapSize, layoutRect.y, brightnessWidth, wheelSize);
                    break;
                case Location.Top:
                    rectBrightness = new Rect(layoutRect.x, layoutRect.y, wheelSize, brightnessWidth);
                    rectWheel = new Rect(layoutRect.x, rectBrightness.yMax + gapSize, wheelSize, wheelSize);
                    break;
                case Location.Bottom:
                default:
                    rectWheel = new Rect(layoutRect.x, layoutRect.y, wheelSize, wheelSize);
                    rectBrightness = new Rect(layoutRect.x, rectWheel.yMax + gapSize, wheelSize, brightnessWidth);
                    break;
            }

            Widgets.HSVColorWheel(rectWheel, ref color, ref currentlyDraggingWheel);

            if (brightnessLocation == Location.Left || brightnessLocation == Location.Right)
                BrightnessBarVertical(rectBrightness, ref color, ref currentlyDraggingBar);
            else
                BrightnessBar(rectBrightness, ref color, ref currentlyDraggingBar);

            SettingsManager.SetSetting(selectedMod, key, color.ToString());
            GUI.color = color_temp;
        }


        private static void BrightnessBar(Rect rect, ref Color color, ref bool dragging)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            if (!dragging)
            {
                Verse.Sound.MouseoverSounds.DoRegion(rect);
            }
            if (Event.current.button == 0)
            {
                if (dragging && Event.current.type == EventType.MouseUp)
                {
                    dragging = false;
                }
                else if ((Mouse.IsOver(rect) && Event.current.type == EventType.MouseDown) || (dragging && UnityGUIBugsFixer.MouseDrag()))
                {
                    dragging = true;
                    if (Event.current.type == EventType.MouseDrag)
                    {
                        Event.current.Use();
                    }

                    float fraction = Mathf.Clamp01((Event.current.mousePosition.x - rect.xMin) / rect.width);
                    v = fraction;
                    color = Color.HSVToRGB(h, s, v);
                }
            }

            if (Event.current.type == EventType.Repaint)
            {
                Texture2D tex = SolidColorMaterials.NewSolidColorTexture(Color.white);
                for (int i = 0; i < rect.width; i++)
                {
                    float value = i / rect.width;
                    Color c = Color.HSVToRGB(h, s, value);
                    GUI.color = c;
                    GUI.DrawTexture(new Rect(rect.x + i, rect.y, 1f, rect.height), tex);
                }
            }

            float xPos = rect.x + rect.width * v;
            Color markerColor = v < 0.5f ? Color.white : Color.black;
            GUI.color = markerColor;

            Widgets.DrawLineVertical(xPos - 2f, rect.y, rect.height);
            Widgets.DrawLineVertical(xPos + 2f, rect.y, rect.height);
        }


        private static void BrightnessBarVertical(Rect rect, ref Color color, ref bool dragging)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            if (!dragging)
            {
                Verse.Sound.MouseoverSounds.DoRegion(rect);
            }

            if (Event.current.button == 0)
            {
                if (dragging && Event.current.type == EventType.MouseUp)
                {
                    dragging = false;
                }
                else if ((Mouse.IsOver(rect) && Event.current.type == EventType.MouseDown) || (dragging && UnityGUIBugsFixer.MouseDrag()))
                {
                    dragging = true;
                    if (Event.current.type == EventType.MouseDrag)
                    {
                        Event.current.Use();
                    }

                    float fraction = Mathf.Clamp01((Event.current.mousePosition.y - rect.yMin) / rect.height);
                    v = 1f - fraction;
                    color = Color.HSVToRGB(h, s, v);
                }
            }

            if (Event.current.type == EventType.Repaint)
            {
                Texture2D tex = SolidColorMaterials.NewSolidColorTexture(Color.white);
                for (int i = 0; i < rect.height; i++)
                {
                    float value = 1f - (i / rect.height);
                    Color c = Color.HSVToRGB(h, s, value);
                    GUI.color = c;
                    GUI.DrawTexture(new Rect(rect.x, rect.y + i, rect.width, 1f), tex);
                }
            }
            float yPos = rect.y + rect.height * (1f - v);
            Color markerColor = v < 0.5f ? new Color(175, 175, 175) : Color.black;
            GUI.color = markerColor;

            Widgets.DrawLineHorizontal(rect.x, yPos - 2f, rect.width, markerColor);
            Widgets.DrawLineHorizontal(rect.x, yPos + 2f, rect.width, markerColor);
        }
    }
}
