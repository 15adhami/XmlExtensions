using HarmonyLib;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(PatchOperation), "Apply")]
    internal static class PatchOperation_Patch
    {
        private static void Prefix(PatchOperation __instance, ref XmlDocument xml)
        {
            try
            {
                if (ErrorManager.depth == 0)
                {
                    ModContentPack pack = PatchManager.ModPatchDict[__instance];
                    if (pack != null && PatchManager.ActiveMod != pack)
                    {
                        PatchManager.ActiveMod = pack;
                    }
                }
            }
            catch
            {

            }
            ErrorManager.depth += 1;
            PatchManager.PatchCount++;
        }

        private static void Postfix(PatchOperation __instance, ref bool __result, XmlDocument xml)
        {
            ErrorManager.depth -= 1;
            if (ErrorManager.depth == 0 && ErrorManager.ErrorCount() > 0 && !__result)
            {
                if (PatchManager.ModPatchDict.ContainsKey(__instance))
                {
                    ErrorManager.PrintErrors(__instance.sourceFile, PatchManager.ModPatchDict[__instance]);
                }
                else
                {
                    ErrorManager.PrintErrors();
                }
            }
            else if (__result)
            {
                ErrorManager.ClearErrors();
            }
        }
    }
}