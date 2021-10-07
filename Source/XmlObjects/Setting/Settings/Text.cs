using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class Text : SettingContainer
    {
        public string text;
        public GameFont font = GameFont.Small;
        public string anchor = "Left";
        public string tooltip = null;
        public string tKey = null;
        public string tKeyTip = null;
        public List<string> keys;
        public string xpath;

        protected override float CalculateHeight(float width, string selectedMod)
        {
            Verse.Text.Font = font;
            TextAnchor t = TextAnchor.UpperLeft;
            if (anchor == "Middle")
            {
                t = TextAnchor.UpperCenter;
            }
            else if (anchor == "Right")
            {
                t = TextAnchor.UpperRight;
            }
            Verse.Text.Anchor = t;
            string str = Helpers.TryTranslate(text, tKey);
            int h = (int)Math.Ceiling(Verse.Text.CalcHeight(str, width));
            Verse.Text.Font = GameFont.Small;
            Verse.Text.Anchor = TextAnchor.UpperLeft;
            return h + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {//M: 29 S: 22 T:18
            Verse.Text.Font = font;
            TextAnchor t = TextAnchor.UpperLeft;
            if(anchor == "Middle")
            {
                t = TextAnchor.UpperCenter;
            }
            else if (anchor == "Right")
            {
                t = TextAnchor.UpperRight;
            }
            Verse.Text.Anchor = t;
            int h = 18;
            if (font == GameFont.Small)
            {
                h = 22;
            }
            else if (font == GameFont.Medium)
            {
                h = 29;
            }
            h += 1;
            string str = Helpers.TryTranslate(text, tKey);
            if (keys != null)
            {
                foreach (string key in keys)
                {
                    str = Helpers.SubstituteVariable(str, key, XmlMod.allSettings.dataDict[selectedMod + ";" + key], "{}");
                }
            }
            // TODO: Add tooltip
            Widgets.Label(inRect, str);                       
            Verse.Text.Font = GameFont.Small;
            Verse.Text.Anchor = TextAnchor.UpperLeft;
        }

        protected override bool Init()
        {
            if (xpath != null)
            {
                text = PatchManager.defaultDoc.SelectSingleNode(xpath).InnerText;
            }
            return true;
        }
    }
}
