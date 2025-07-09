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
        // For debugging
        public static Stopwatch watch;
        public static Stopwatch watch2;
        public static int PatchCount = 0;
        public static int FailedPatchCount = 0;
        public static bool applyingPatches = false;

        // Required to fix a vanilla bug with range sliders
        public static int rangeCount = 1;

        // Used for xmldoc patch operations
        public static XmlDocument xmlDoc;
        public static XmlDocument defaultDoc;
        public static Dictionary<string, XmlDocument> XmlDocs;
        public static Dictionary<string, Dictionary<XmlNode, XmlNode>> nodeMap;

        // For stack traces
        public static ModContentPack ActiveMod;
        public static Dictionary<PatchOperation, ModContentPack> PatchModDict;

        // For DefDatabase operations
        public static List<PatchOperationExtended> delayedPatches;

        

        static PatchManager()
        {
            delayedPatches = new List<PatchOperationExtended>();
            PatchModDict = new Dictionary<PatchOperation, ModContentPack>();
            nodeMap = new Dictionary<string, Dictionary<XmlNode, XmlNode>>();
            XmlDocs = new Dictionary<string, XmlDocument>();
            watch = new Stopwatch();
            watch2 = new Stopwatch();
            xmlDoc = new XmlDocument();
            defaultDoc = new XmlDocument();
        }

        internal static PatchCoordinator Coordinator = new();
        internal static XmlDocumentManager XmlDocs = new();
        internal static PatchProfiler Profiler = new();

        internal static void SetActivePatchingMod(ModContentPack mod) => Coordinator.SetActiveMod(mod);

        internal static bool CheckType(Type t) => PatchFilter.IsValidPatchOperation(t);

        internal static bool CheckTypePathed(Type t) => PatchFilter.IsValidPathedOperation(t);
    }
}