using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using XmlExtensions.Action;

namespace XmlExtensions.Setting
{
    [Obsolete]
    internal class ApplyActions : SettingContainer
    {
        protected bool confirm = false;
        public float height = 30;
        public List<ActionContainer> actions;
        public string message;
        public string tKeyMessage;

        protected override bool Init()
        {
            if (label == null)
            {
                label = "Apply";
            }
            WarnUsingObselete([typeof(Button)]);
            return InitializeContainers(actions);
        }

        protected override float CalculateHeight(float width)
        {
            return height;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(inRect, Helpers.TryTranslate(tooltip, tKeyTip));
            }
            if (Widgets.ButtonText(inRect, Helpers.TryTranslate(label, tKey)))
            {
                if (actions != null)
                {
                    if (!confirm)
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
                    else
                    {
                        Find.WindowStack.Add(new Dialog_MessageBox(Helpers.TryTranslate(message, tKeyMessage), "Yes".Translate(), delegate ()
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
                        }, "No".Translate(), null, null, false, null, null));
                    }
                }
            }
        }
    }
}