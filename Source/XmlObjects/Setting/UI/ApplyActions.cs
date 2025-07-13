using System.Collections.Generic;
using UnityEngine;
using Verse;
using XmlExtensions.Action;

namespace XmlExtensions.Setting
{
    internal class ApplyActions : SettingContainer
    {
        public string label = "Apply";
        public string tKey;
        protected bool confirm = false;
        public List<ActionContainer> actions;
        public string message;
        public string tKeyMessage;
        public string tKeyTip;
        public string tooltip;

        protected override bool Init()
        {
            return InitializeContainers(modId, actions);
        }

        protected override float CalculateHeight(float width)
        {
            return 30;
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