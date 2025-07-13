using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using Verse;
using XmlExtensions.Action;

namespace XmlExtensions.Setting
{
    internal class ApplyActions : SettingContainer
    {
        public string label;
        public string tKey;
        public List<ActionContainer> actions;

        protected override bool Init()
        {
            if (label == null)
            {
                label = "Apply";
                tKey = "XmlExtensions_Apply";
            }
            return InitializeContainers(modId, actions);
        }

        protected override float CalculateHeight(float width)
        {
            return 30;
        }

        protected override void DrawSettingContents(Rect inRect)
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

        internal override bool PreOpen()
        {
            return PreOpenContainers(actions);
        }

        internal override bool PostClose()
        {
            return PostCloseContainers(actions);
        }
    }
}