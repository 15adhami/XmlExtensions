using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Linq;
using Verse;

namespace XmlExtensions
{
    internal static class PatchManager
    {
        public static XmlDocument xmlDoc;
        public static XmlDocument defaultDoc;
        public static Stopwatch watch;
        public static Stopwatch watch2;
        public static int PatchCount = 0;
        public static int FailedPatchCount = 0;

        public static bool applyingPatches = false;
        public static bool loadingPatches = false;
        public static int rangeCount = 1;
        public static bool context = false;
        public static Dictionary<Type, Delegate> patchConstructors;
        public static Dictionary<string, XmlDocument> XmlDocs;
        public static Dictionary<string, Dictionary<XmlNode, XmlNode>> nodeMap;
        public static ModContentPack ActiveMod;
        public static Dictionary<PatchOperation, ModContentPack> PatchModDict;
        public static Dictionary<string, HashSet<ModContentPackContainer>> DefModDict;
        public static HashSet<string> PatchedDefSet;
        public static List<PatchOperationExtended> delayedPatches;

        public static List<Type> PatchedPatchOperations = new List<Type> { typeof(Verse.PatchOperationFindMod), typeof(Verse.PatchOperationSequence), typeof(Verse.PatchOperationAttributeAdd), typeof(Verse.PatchOperationAttributeRemove), typeof(Verse.PatchOperationAttributeSet), typeof(Verse.PatchOperationConditional),
             typeof(Verse.PatchOperationSetName),  };

        static PatchManager()
        {
            PatchedDefSet = new();
            DefModDict = new();
            delayedPatches = new List<PatchOperationExtended>();
            PatchModDict = new Dictionary<PatchOperation, ModContentPack>();
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
            }
        }

        public static void ModPatchedDef(string name, ModContentPack pack, Type type)
        {
            pack ??= ActiveMod;
            if (!DefModDict.ContainsKey(name))
            {
                DefModDict.Add(name, new HashSet<ModContentPackContainer>());
            }
            if (!DefModDict[name].Any(c => c.Pack == pack))
            {
                DefModDict[name].Add(new ModContentPackContainer(pack, type));
            }
            else
            {
                ModContentPackContainer container = DefModDict[name].Single(p => p.Pack == pack);
                if (!container.OperationTypes.Contains(type))
                {
                    container.OperationTypes.Add(type);
                }
            }
            if (type != null)
            {
                if (!PatchedDefSet.Contains(name))
                {
                    PatchedDefSet.Add(name);
                }
            }
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