using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(Dialog_Options), "DoCategoryRow")]
    internal static class Dialog_Options_Patch
    {// TODO: Need to fix settings menu
        private static void Prefix(ref Rect r, OptionCategoryDef optionCategory)
        {
            if (optionCategory == OptionCategoryDefOf.Mods)
            {
                Rect r_right = r.RightPart(0.15f);
                Rect r_right_button = r_right.RightPartPixels(r_right.width-4);
                r = r.LeftPart(0.85f);

                Widgets.DrawOptionBackground(r_right_button, false);
                TooltipHandler.TipRegion(r_right_button, Helpers.TryTranslate("More Mod Settings", "XmlExtensions_MoreModSettings"));
                if (Widgets.ButtonInvisible(r_right_button))
                {
                    Find.WindowStack.Add(new XmlExtensions_MenuModSettings());
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }
                Rect label_position = new Rect(r_right.x + 10f, r_right.y + (r_right.height - 20f) / 2f, 20f, 20f);
                Widgets.Label(label_position, "+");
            }
        }
        /*
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            var translate = AccessTools.Method(typeof(Helpers), "TryTranslate");
            var foundModSettings = false;
            var startIndex = -1;
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldstr)
                {
                    var strOperand = codes[i].operand as string;
                    if (strOperand == "ModSettings")
                    {
                        startIndex = i + 10;
                        foundModSettings = true;
                        break;
                    }
                }
            }

            if (foundModSettings)
            {
                Label label = gen.DefineLabel();
                codes[startIndex].labels.Add(label);
                codes.Insert(startIndex, new CodeInstruction(OpCodes.Ldloc, 1));
                codes.Insert(startIndex + 1, new CodeInstruction(OpCodes.Ldstr, "More Mod Settings"));
                codes.Insert(startIndex + 2, new CodeInstruction(OpCodes.Ldstr, "XmlExtensions_MoreModSettings"));
                codes.Insert(startIndex + 3, new CodeInstruction(OpCodes.Call, translate));
                codes.Insert(startIndex + 4, new CodeInstruction(OpCodes.Ldnull));
                codes.Insert(startIndex + 5, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Listing_Standard), "ButtonText", new Type[] { typeof(string), typeof(string) })));
                codes.Insert(startIndex + 6, new CodeInstruction(OpCodes.Brfalse_S, label));
                codes.Insert(startIndex + 7, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Find), "get_WindowStack")));
                codes.Insert(startIndex + 8, new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(XmlExtensions_MenuModSettings))));
                codes.Insert(startIndex + 9, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(WindowStack), "Add", new[] { typeof(Window) })));
            }

            return codes.AsEnumerable();
        }
        */
    }
}