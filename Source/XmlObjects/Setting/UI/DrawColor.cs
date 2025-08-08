using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using XmlExtensions.Action;

namespace XmlExtensions.Setting
{
    internal class DrawColor : SettingContainer
    {
        public new Color color = new();
        public int colorSize = 22;
        public int border = 2;
        public Anchor anchor = Anchor.Middle;
        public bool drawSelection = false;
        public List<ActionContainer> actions;

        public enum Anchor
        {
            Left,
            Middle,
            Right
        }

        protected override bool Init()
        {
            return InitializeContainers(actions);
        }

        protected override float CalculateHeight(float width)
        {
            return colorSize + border * 2;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            Color drawColor = color;
            if (!string.IsNullOrEmpty(key))
            {
                string settingValue = SettingsManager.GetSetting(modId, key);
                if (!string.IsNullOrEmpty(settingValue))
                {
                    try
                    {
                        drawColor = ParseHelper.FromString<Color>(settingValue);
                    }
                    catch
                    {
                        // Parsing failed; keep default color
                    }
                }
            }

            float boxSize = colorSize + border * 2;
            Rect rect = inRect.TopPartPixels(boxSize);

            Rect alignedRect = anchor switch
            {
                Anchor.Left => new Rect(rect.x, rect.y, boxSize, boxSize),
                Anchor.Middle => new Rect(rect.x + (rect.width - boxSize) / 2f, rect.y, boxSize, boxSize),
                Anchor.Right => new Rect(rect.xMax - boxSize, rect.y, boxSize, boxSize),
                _ => rect
            };

            Widgets.DrawLightHighlight(alignedRect);
            Color selectColor = new();
            if (color != null)
            {
                selectColor = color;
            }
            else if (key != null)
            {
                selectColor = drawColor;
            }/*
            if (drawSelection && drawColor.IndistinguishableFrom(selectColor))
            {
                Widgets.DrawBox(alignedRect);
            }*/

            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(alignedRect, Helpers.TryTranslate(tooltip, tKeyTip));
            }

            Rect inner = new(alignedRect.x + border, alignedRect.y + border, colorSize, colorSize);
            if (actions != null)
            {
                Widgets.DrawHighlightIfMouseover(alignedRect);
                if (Widgets.ButtonInvisible(alignedRect))
                {
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                    int i = 0;
                    foreach (ActionContainer action in actions)
                    {
                        i++;
                        if (!action.DoAction())
                        {
                            Error("Failed action at index=" + i.ToString());
                            ErrorManager.PrintErrors();
                            break;
                        }
                    }
                }
            }

            Widgets.DrawBoxSolid(inner, drawColor);
        }
    }
}
