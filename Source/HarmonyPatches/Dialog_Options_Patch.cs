using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(Dialog_Options), "DoCategoryRow")]
    internal static class Dialog_Options_Patch
    {
        private static void Prefix(ref Rect r, OptionCategoryDef optionCategory)
        {
            if (optionCategory == OptionCategoryDefOf.Mods && XmlMod.allSettings.showSettingsButton)
            {
                Rect r_right = r.RightPart(0.15f);
                Rect r_right_button = r_right.RightPartPixels(r_right.width-4);
                r = r.LeftPart(0.85f);

                Widgets.DrawOptionBackground(r_right_button, false);
                if (Widgets.ButtonInvisible(r_right_button))
                {
                    Find.WindowStack.Add(new XmlExtensionsMenuModSettings());
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }
                Rect label_position = new Rect(r_right.x + 10f, r_right.y + (r_right.height - 20f) / 2f, 20f, 20f);
                Widgets.Label(label_position, "+");
            }
        }
    }
}