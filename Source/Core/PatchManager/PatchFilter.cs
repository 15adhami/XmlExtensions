using HarmonyLib;
using System;
using System.Collections.Generic;
using Verse;

namespace XmlExtensions
{
    internal static class PatchFilter
    {
        internal static List<Type> PatchedPatchOperations = new List<Type> { typeof(Verse.PatchOperationFindMod), typeof(Verse.PatchOperationSequence), typeof(Verse.PatchOperationAttributeAdd), typeof(Verse.PatchOperationAttributeRemove), typeof(Verse.PatchOperationAttributeSet), typeof(Verse.PatchOperationConditional),
             typeof(Verse.PatchOperationSetName),  };
        internal static bool IsValidPatchOperation(Type T) 
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
        internal static bool IsValidPathedOperation(Type T)
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
