using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal static class PatchManager
    {
        public static XmlDocument xmlDoc;
        public static XmlDocument defaultDoc;
        public static Stopwatch watch;
        public static Stopwatch watch2;

        public static bool applyingPatches = false;
        public static bool loadingPatches = false;
        public static int rangeCount = 1;
        public static bool context = false;
        public static Dictionary<Type, Delegate> patchConstructors;
        public static Dictionary<string, XmlDocument> XmlDocs;
        public static Dictionary<string, Dictionary<XmlNode, XmlNode>> nodeMap;
        public static ModContentPack ActiveMod;
        public static Dictionary<PatchOperation, ModContentPack> ModPatchDict;
        public static List<DefDatabaseOperation> delayedPatches;

        public static List<Type> PatchedClasses = new List<Type> { typeof(Verse.PatchOperationFindMod), typeof(Verse.PatchOperationSequence), typeof(Verse.PatchOperationAttributeAdd), typeof(Verse.PatchOperationAttributeRemove), typeof(Verse.PatchOperationAttributeSet), typeof(Verse.PatchOperationConditional),
             typeof(Verse.PatchOperationSetName),  };

        static PatchManager()
        {
            delayedPatches = new List<DefDatabaseOperation>();
            ModPatchDict = new Dictionary<PatchOperation, ModContentPack>();
            patchConstructors = new Dictionary<Type, Delegate>();
            nodeMap = new Dictionary<string, Dictionary<XmlNode, XmlNode>>();
            XmlDocs = new Dictionary<string, XmlDocument>();
            watch = new Stopwatch();
            watch2 = new Stopwatch();
            xmlDoc = new XmlDocument();
            defaultDoc = new XmlDocument();
        }

        public static void SetActiveMod(ModContentPack mod)
        {
            if (mod != null)
            {
                ActiveMod = mod;
                Verse.Log.Message(mod.PackageId);
            }
            else
            {
                Verse.Log.Message("null");
            }
        }

        public static bool CheckType(Type T)
        {
            if (typeof(PatchOperationExtended).IsAssignableFrom(T)) { return false; }
            if (T.Namespace == "XmlExtensions") { return false; }
            if (typeof(PatchOperationPathed).IsAssignableFrom(T)) { return false; }
            if (PatchedClasses.Contains(T)) { return false; }
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
            if (PatchedClasses.Contains(T)) { return false; }
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