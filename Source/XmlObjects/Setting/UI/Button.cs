﻿using System.Collections.Generic;
using UnityEngine;
using Verse;
using XmlExtensions.Action;

namespace XmlExtensions.Setting
{
    public class Button : SettingContainer
    {
        public string label;
        public string tKey;
        protected bool confirm = false;
        public float height = 30;
        public List<ActionContainer> actions;
        public string message;
        public string tKeyMessage;
        public string tKeyTip;
        public string tooltip;

        protected override bool Init()
        {
            if (label == null)
            {
                label = "Apply";
                tKey ??= "XmlExtensions_Apply";
            }
            if (message == null)
            {
                message = "Are you sure?";
                tKeyMessage ??= "XmlExtensions_Confirmation";
            }
            return InitializeContainers(menuDef, actions);
        }

        protected override float CalculateHeight(float width)
        {
            return height;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            if (!tooltip.NullOrEmpty())
            {
                string tooltipLabel = Helpers.TryTranslate(tooltip, tKeyTip);
                tooltipLabel = Helpers.SubstituteVariable(tooltipLabel, "key", SettingsManager.GetSetting(modId, key), "{}");
                TooltipHandler.TipRegion(inRect, tooltipLabel);
            }
            string buttonLabel = Helpers.TryTranslate(label, tKey);
            buttonLabel = Helpers.SubstituteVariable(buttonLabel, "key", SettingsManager.GetSetting(modId, key), "{}");
            if (Widgets.ButtonText(inRect, buttonLabel))
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