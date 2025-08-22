using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Verse;
using XmlExtensions.Action;

namespace XmlExtensions.Setting
{
    public class Button : SettingContainer
    {
        protected bool confirm = false;
        public float height = 30;
        public Style style = Style.UIButton;
        public List<ActionContainer> actions;
        public string message;
        public string tKeyMessage;

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
            return InitializeContainers(actions);
        }

        protected override float CalculateHeight(float width)
        {
            return style == Style.OptionButton ? height + 6f : height;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            if (!tooltip.NullOrEmpty())
            {
                string tooltipLabel = tooltip.TryTKey(tKeyTip);
                tooltipLabel = tooltipLabel.SubstituteVariable("key", SettingsManager.GetSetting(modId, key));
                TooltipHandler.TipRegion(inRect, tooltipLabel);
            }
            string buttonLabel = label.TryTKey(tKey);
            buttonLabel = buttonLabel.SubstituteVariable("key", SettingsManager.GetSetting(modId, key));
            Rect drawRect = inRect;
            if (style == Style.OptionButton)
            { // Add extra space for this style
                drawRect = inRect.ContractedBy(0f, 3f);
            }
            if (DrawButton(drawRect, buttonLabel))
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
                        Find.WindowStack.Add(new Dialog_MessageBox(message.TryTKey(tKeyMessage), "Yes".Translate(), delegate ()
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

        private bool DrawButton(Rect rect, string label)
        {
            bool clicked = false;
            if (style == Style.UIButton)
            {
                clicked = Widgets.ButtonText(rect, label);
            }
            else if (style == Style.OptionButton)
            {
                Rect drawRect = rect.MiddlePartPixels(rect.width - 6f, rect.height);
                clicked = Widgets.ButtonInvisible(drawRect);
                Widgets.DrawOptionBackground(drawRect, Mouse.IsOver(drawRect) && UnityEngine.Input.GetMouseButton(0));
                Verse.Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect, label);
                Verse.Text.Anchor = TextAnchor.UpperLeft;
            }
            else if (style == Style.MainButton)
            {
                clicked = Widgets.ButtonTextSubtle(rect, null, 0f, -1f, SoundDefOf.Mouseover_Category);
                Verse.Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect, label);
                Verse.Text.Anchor = TextAnchor.UpperLeft;
                if (Mouse.IsOver(rect) && UnityEngine.Input.GetMouseButton(0))
                {
                    Color colorTemp = GUI.color;
                    GUI.color = Color.grey;
                    Widgets.DrawBox(rect, 2);
                    GUI.color = colorTemp;
                }
            }
            return clicked;
        }
    }
}