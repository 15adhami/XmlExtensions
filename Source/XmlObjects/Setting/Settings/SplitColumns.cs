using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class SplitColumns : SettingContainer
    {
        public List<float> splits = new List<float>() { 0.5f };
        public List<List<SettingContainer>> settings;
        public bool drawLine = false;
        public int gapSize = 6;

        protected override bool Init(string selectedMod)
        {
            int c = 0;
            foreach (List<SettingContainer> list in settings)
            {
                c++;
                if (!InitializeSettingsList(selectedMod, list, c.ToString()))
                {
                    return false;
                }
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            float offset = 0;
            int c = 0;
            float h = 0;
            foreach (List<SettingContainer> list in settings)
            {
                float tempWidth = 0;
                if (c == splits.Count)
                {
                    tempWidth = width - offset - gapSize / 2;
                }
                else if (c > splits.Count)
                {
                    tempWidth = 0;
                }
                else
                {
                    tempWidth = width * splits[c] - gapSize / 2;
                }
                h = Math.Max(GetHeightSettingsList(tempWidth, selectedMod, list), h);
                offset += tempWidth + gapSize;
                c++;
            }
            return h;
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            float offset = 0;
            int c = 0;
            float width = inRect.width;
            Rect currRect = inRect;
            Rect tempRect = new Rect(inRect);
            foreach (List<SettingContainer> list in settings)
            {
                float tempWidth = 0;
                
                if (c == splits.Count)
                {
                    tempWidth = width - offset - gapSize / 2;
                    tempRect = currRect;
                }
                else if (c > splits.Count)
                {
                    tempWidth = 0;
                    tempRect = currRect.LeftPartPixels(tempWidth);
                }
                else
                {
                    tempWidth = width * splits[c] - gapSize / 2;
                    tempRect = currRect.LeftPartPixels(tempWidth);
                }
                DrawSettingsList(tempRect, selectedMod, list);
                if (drawLine)
                {
                    Color color = GUI.color;
                    GUI.color = color * new Color(1f, 1f, 1f, 0.4f);
                    GUI.DrawTexture(new Rect(currRect.x + tempWidth + gapSize / 2, currRect.yMin, 1f, currRect.height), BaseContent.WhiteTex);
                    GUI.color = color;
                }
                currRect = currRect.RightPartPixels(currRect.width - (tempWidth +  gapSize));
                offset += tempWidth + gapSize;
                c++;
            }
        }
    }
}
