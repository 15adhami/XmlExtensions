using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(Dialog_Options))]
    [HarmonyPatch("DoWindowContents")]
    static class Dialog_Options_patch
    {
        
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
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
                CodeInstruction item = new CodeInstruction(OpCodes.Ldloc, 1);
                codes.Insert(startIndex, item);
                item = new CodeInstruction(OpCodes.Ldstr, "More Mod Settings");
                codes.Insert(startIndex + 1, item);
                item = new CodeInstruction(OpCodes.Ldstr, "XmlExtensions_MoreModSettings");
                codes.Insert(startIndex + 2, item);
                item = new CodeInstruction(OpCodes.Call, translate);
                codes.Insert(startIndex + 3, item);
                item = new CodeInstruction(OpCodes.Ldnull);
                codes.Insert(startIndex + 4, item);
                item = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Listing_Standard), "ButtonText", new Type[] { typeof(string), typeof(string)}));
                codes.Insert(startIndex + 5, item);
                item = new CodeInstruction(OpCodes.Brfalse_S, label);
                codes.Insert(startIndex + 6, item);
                item = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Find), "get_WindowStack"));
                codes.Insert(startIndex + 7, item);
                item = new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(XmlExtensions_SettingsMenu)));
                codes.Insert(startIndex + 8, item);
                item = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(WindowStack), "Add", new[] { typeof(Window)}));
                codes.Insert(startIndex + 9, item);
            }

            return codes.AsEnumerable();
        }
    }
}
