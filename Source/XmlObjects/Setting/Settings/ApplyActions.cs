using System.Collections.Generic;
using UnityEngine;
using Verse;
using XmlExtensions.Action;

namespace XmlExtensions.Setting
{
    public class ApplyActions : SettingContainer
    {
        public string label;
        public string tKey;
        public List<ActionContainer> actions;

        protected override bool Init(string selectedMod)
        {
            if (label == null)
            {
                label = "Apply";
                tKey = "XmlExtensions_Apply";
            }
            return true;
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 30 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            if (Widgets.ButtonText(inRect, Helpers.TryTranslate(label, tKey)))
            {
                if (actions != null)
                {
                    int c = 0;
                    foreach (ActionContainer action in actions)
                    {
                        c++;
                        if (!action.DoAction())
                        {
                            Error("Error in the action at position=" + c.ToString());
                            break;
                        }
                    }
                }
            }
        }
    }
}