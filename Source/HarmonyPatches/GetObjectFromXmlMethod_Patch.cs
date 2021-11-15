using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(DirectXmlToObject), "GetObjectFromXmlMethod")]
    internal static class GetObjectFromXmlMethod_Patch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            var foundMethod = false;
            var startIndex = -1;
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stloc_1)
                {
                    startIndex = i;
                    foundMethod = true;
                    break;
                }
            }

            if (foundMethod)
            {
                codes.Insert(startIndex, new CodeInstruction(OpCodes.Pop));
                codes.Insert(startIndex + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomXmlLoader), "GetObjectFromXmlReflection")));
            }

            return codes.AsEnumerable();
        }
    }
}