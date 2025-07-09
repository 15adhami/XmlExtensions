using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(LoadedModManager), "CombineIntoUnifiedXML")]
    internal static class CombineIntoUnifiedXML_Patch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>();
            codes.Add(new CodeInstruction(OpCodes.Ldarg_0));
            codes.Add(new CodeInstruction(OpCodes.Ldarg_1));
            codes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomXmlLoader), "CombineIntoUnifiedXMLMirror")));
            codes.Add(new CodeInstruction(OpCodes.Ret));
            return codes;
        }
    }
}