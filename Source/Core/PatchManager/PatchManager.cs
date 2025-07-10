using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Verse;
using XmlExtensions.Source.Core;

namespace XmlExtensions
{
    internal static class PatchManager
    {
        internal static PatchCoordinator Coordinator = new();
        internal static XmlDocumentManager XmlDocs = new();
        internal static PatchProfiler Profiler = new();

        internal static void SetActivePatchingMod(ModContentPack mod) => Coordinator.SetActiveMod(mod);

        internal static bool CheckType(Type t) => PatchFilter.IsValidPatchOperation(t);

        internal static bool CheckTypePathed(Type t) => PatchFilter.IsValidPathedOperation(t);

    }
}