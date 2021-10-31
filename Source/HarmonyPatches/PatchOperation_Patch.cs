using HarmonyLib;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(PatchOperation), "Apply")]
    internal static class PatchOperation_Patch
    {
        private static void Prefix(ref XmlDocument xml)
        {
            ErrorManager.depth += 1;
        }

        private static void Postfix(PatchOperation __instance, ref bool __result, XmlDocument xml)
        {
            ErrorManager.depth -= 1;
            if (ErrorManager.depth == 0 && ErrorManager.ErrorCount() > 0 && !__result)
            {
                ErrorManager.PrintErrors(__instance.sourceFile, PatchManager.ModPatchDict[__instance]);
            }
            else if (__result)
            {
                ErrorManager.ClearErrors();
            }
        }
    }
}