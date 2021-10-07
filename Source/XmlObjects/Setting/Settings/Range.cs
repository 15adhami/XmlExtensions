using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace XmlExtensions.Setting
{
    public class Range : KeyedSettingContainer
    {
        public int min;
        public int max;
        public string key2;
        public int id = 0;
        public int decimals = 0;

        private Color cColor;

        protected override bool Init()
        {
            // Used to avoid a vanilla bug in how ranges are dragged
            id = PatchManager.rangeCount;
            PatchManager.rangeCount++;
            return true;
        }

        protected override bool SetDefaultValue(string modId)
        {
            if (key == null)
            {
                ThrowError("<key> is null");
                return false;
            }
            if (key2 == null)
            {
                SettingsManager.SetDefaultValue(modId, key, defaultValue);
            }
            else
            {
                SettingsManager.SetDefaultValue(modId, key, defaultValue.Split('~')[0]);
                SettingsManager.SetDefaultValue(modId, key2, defaultValue.Split('~')[1]);
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 28 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            cColor = GUI.color;
            Rect rect = inRect.TopPartPixels(28f);
            if (key2 == null)
            {
                FloatRange range = Verse.FloatRange.FromString(SettingsManager.GetSetting(selectedMod, key));
                FloatRange(rect, id, ref range, min, max, null);
                SettingsManager.SetSetting(selectedMod, key, Math.Round(range.min, decimals).ToString() + "~" + Math.Round(range.max, decimals).ToString());
            }
            else
            {
                FloatRange range = Verse.FloatRange.FromString(SettingsManager.GetSetting(selectedMod, key) + "~" + SettingsManager.GetSetting(selectedMod, key2));
                FloatRange(rect, id, ref range, min, max, null);
                SettingsManager.SetSetting(selectedMod, key, Math.Round(range.min, decimals).ToString());
                SettingsManager.SetSetting(selectedMod, key2, Math.Round(range.max, decimals).ToString());
            }
            GUI.color = cColor;
        }

        private enum RangeEnd : byte
        {
            None,
            Min,
            Max
        }

        private RangeEnd curDragEnd;

        private int draggingId = 0;
        private float lastDragSliderSoundTime = 0;

        private void FloatRange(Rect rect, int id, ref FloatRange range, float min = 0f, float max = 1f, string labelKey = null)
        {
            Rect rect2 = rect;
            rect2.xMin += 8f;
            rect2.xMax -= 8f;
            GUI.color = new Color(0.6f, 0.6f, 0.6f) * cColor;
            string text = Math.Round(range.min, decimals).ToString() + " - " + Math.Round(range.max, decimals).ToString();
            if (labelKey != null)
            {
                text = labelKey.Translate(text);
            }
            GameFont font = Verse.Text.Font;
            Verse.Text.Font = GameFont.Tiny;
            Verse.Text.Anchor = TextAnchor.UpperCenter;
            Rect rect3 = rect2;
            rect3.yMin -= 2f;
            rect3.height = Mathf.Max(rect3.height, Verse.Text.CalcHeight(text, rect3.width));
            Widgets.Label(rect3, text);
            Verse.Text.Anchor = TextAnchor.UpperLeft;
            Rect position = new Rect(rect2.x, rect2.yMax - 8f - 1f, rect2.width, 2f);
            GUI.DrawTexture(position, BaseContent.WhiteTex);
            GUI.color = Color.white * cColor;
            float num = rect2.x + rect2.width * Mathf.InverseLerp(min, max, range.min);
            float num2 = rect2.x + rect2.width * Mathf.InverseLerp(min, max, range.max);
            Rect position2 = new Rect(num - 16f, position.center.y - 8f, 16f, 16f);
            Texture2D FloatRangeSliderTex = ContentFinder<Texture2D>.Get("UI/Widgets/RangeSlider");
            GUI.DrawTexture(position2, FloatRangeSliderTex);
            Rect position3 = new Rect(num2 + 16f, position.center.y - 8f, -16f, 16f);
            GUI.DrawTexture(position3, FloatRangeSliderTex);
            if (curDragEnd != 0 && (Event.current.type == EventType.MouseUp || Event.current.rawType == EventType.MouseDown))
            {
                draggingId = 0;
                curDragEnd = RangeEnd.None;
                SoundDefOf.DragSlider.PlayOneShotOnCamera();
            }
            bool flag = false;
            if (Mouse.IsOver(rect) || draggingId == id)
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && id != draggingId)
                {
                    draggingId = id;
                    float x = Event.current.mousePosition.x;
                    if (x < position2.xMax)
                    {
                        curDragEnd = RangeEnd.Min;
                    }
                    else if (x > position3.xMin)
                    {
                        curDragEnd = RangeEnd.Max;
                    }
                    else
                    {
                        float num3 = Mathf.Abs(x - position2.xMax);
                        float num4 = Mathf.Abs(x - (position3.x - 16f));
                        curDragEnd = ((num3 < num4) ? RangeEnd.Min : RangeEnd.Max);
                    }
                    flag = true;
                    Event.current.Use();
                    SoundDefOf.DragSlider.PlayOneShotOnCamera();
                }
                if (flag || (curDragEnd != 0 && Event.current.type == EventType.MouseDrag))
                {
                    float value = (Event.current.mousePosition.x - rect2.x) / rect2.width * (max - min) + min;
                    value = Mathf.Clamp(value, min, max);
                    if (curDragEnd == RangeEnd.Min)
                    {
                        if (value != range.min)
                        {
                            range.min = value;
                            if (range.max < range.min)
                            {
                                range.max = range.min;
                            }
                            if (Time.realtimeSinceStartup > lastDragSliderSoundTime + 0.075f)
                            {
                                SoundDefOf.DragSlider.PlayOneShotOnCamera();
                                lastDragSliderSoundTime = Time.realtimeSinceStartup;
                            }
                        }
                    }
                    else if (curDragEnd == RangeEnd.Max && value != range.max)
                    {
                        range.max = value;
                        if (range.min > range.max)
                        {
                            range.min = range.max;
                        }
                        if (Time.realtimeSinceStartup > lastDragSliderSoundTime + 0.075f)
                        {
                            SoundDefOf.DragSlider.PlayOneShotOnCamera();
                            lastDragSliderSoundTime = Time.realtimeSinceStartup;
                        }
                    }
                    Event.current.Use();
                }
            }
            Verse.Text.Font = font;
        }
    }
}