using HarmonyLib;
using System;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(ModsConfig), "TrySortMods")]
    static class TrySortMods_Patch
    {
        static Exception Finalizer(Exception __exception)
        {
            if (__exception != null)
            {
                Verse.Log.Error("XML EXTENSIONS DETECTED A FATAL ERROR: READ ALL OTHER WARNINGS AND ERRORS\n(Could maybe be an XML parsing error?)\n" + __exception);
            }
            return null;
        }
    }
}