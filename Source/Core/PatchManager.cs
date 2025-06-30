using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Verse;

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

        // Required to fix a vanilla bug with range sldiers
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

        public static List<Type> PatchedPatchOperations = new List<Type> { typeof(Verse.PatchOperationFindMod), typeof(Verse.PatchOperationSequence), typeof(Verse.PatchOperationAttributeAdd), typeof(Verse.PatchOperationAttributeRemove), typeof(Verse.PatchOperationAttributeSet), typeof(Verse.PatchOperationConditional),
             typeof(Verse.PatchOperationSetName),  };

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

        public static void SetActiveMod(ModContentPack mod)
        {
            ActiveMod = mod;
        }

        public static bool CheckType(Type T)
        {
            if (typeof(PatchOperationExtended).IsAssignableFrom(T)) { return false; }
            if (T.Namespace == "XmlExtensions") { return false; }
            if (typeof(PatchOperationPathed).IsAssignableFrom(T)) { return false; }
            if (PatchedPatchOperations.Contains(T)) { return false; }
            try
            {
                if (AccessTools.Method(T, "ApplyWorker") == null)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool CheckTypePathed(Type T)
        {
            if (typeof(PatchOperationExtended).IsAssignableFrom(T)) { return false; }
            if (T.Namespace == "XmlExtensions") { return false; }
            if (PatchedPatchOperations.Contains(T)) { return false; }
            try
            {
                if (AccessTools.Method(T, "ApplyWorker") == null)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}