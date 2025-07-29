using RimWorld;
using System.Collections.Generic;
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
        public Style style = Style.UIButton;
        public List<ActionContainer> actions;
        public string message;
        public string tKeyMessage;
        public string tKeyTip;
        public string tooltip;

        public enum Style
        {
            UIButton,
            OptionButton,
            MainButton
        }

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
            if (DrawButton(inRect, buttonLabel))
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

        private bool DrawButton(Rect rect, string label)
        {
            bool clicked = false;
            if (style == Style.UIButton)
            {
                clicked = Widgets.ButtonText(rect, label);
            }
            else if (style == Style.OptionButton)
            {
                Widgets.DrawOptionBackground(rect, false);
                Verse.Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect, label);
                Verse.Text.Anchor = TextAnchor.UpperLeft;
                clicked = Widgets.ButtonInvisible(rect);
            }
            else if (style == Style.MainButton)
            {
                clicked = Widgets.ButtonTextSubtle(rect, null, 0f, -1f, SoundDefOf.Mouseover_Category);
                Verse.Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect, label);
                Verse.Text.Anchor = TextAnchor.UpperLeft;
            }
            return clicked;
        }
    }
}