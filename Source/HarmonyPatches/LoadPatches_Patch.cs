using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace XmlExtensions
{
    // This patch replaces the DirectXmlToObject.ObjectFromXml<PatchOperation>() call with CustomXmlLoader.ObjectFromXml<PatchOperation>()

    [HarmonyPatch(typeof(ModContentPack), "LoadPatches")]
    internal static class LoadPatches_Patch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            var translate = AccessTools.Method(typeof(Helpers), "TryTranslate");
            var foundMethod = false;
            var startIndex = -1;
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call)
                {
                    var method = codes[i].operand as MethodBase;
                    if (method.Name == "ObjectFromXml")
                    {
                        startIndex = i;
                        foundMethod = true;
                        break;
                    }
                }
            }

            if (foundMethod)
            {
                codes[startIndex].operand = AccessTools.Method(typeof(CustomXmlLoader), "ObjectFromXmlReflection").MakeGenericMethod(typeof(PatchOperation));
            }

            return codes.AsEnumerable();
        }
    }
}