using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using HarmonyLib;
using Verse;

namespace XmlExtensions
{
    /*
    [HarmonyPatch(typeof(ModContentPack))]
    [HarmonyPatch("ClearPatchesCache")]
    static class ClearPatchesCache_Patch
    {
        static void Postfix(List<PatchOperation> ___patches)
        {
            if(PatchManager.loadingPatches)
            {
                ___patches = new List<PatchOperation>();
            }
        }
    }*/

    [HarmonyPatch(typeof(ModsConfig))]
    [HarmonyPatch("TrySortMods")]
    static class PatchOperationLoads_Patch
    {
        static Exception Finalizer(Exception __exception)
        {
            if (__exception != null)
            {
                Verse.Log.Error("FATAL ERROR: READ WARNINGS AND OTHER ERRORS");
            }
            return null;
        }

    }

    /*
    [HarmonyPatch(typeof(LoadedModManager))]
    [HarmonyPatch("ErrorCheckPatches")]
    static class PatchOperationLoad_Patch
    {
        static Exception Finalizer(Exception __exception, List<ModContentPack> ___runningMods)
        {
            if (__exception != null)
            {
                foreach (ModContentPack modContentPack in ___runningMods)
                {
                    modContentPack.ClearPatchesCache();
                }
            }
            return null;
        }

    }*/

    [HarmonyPatch(typeof(PatchOperationAdd))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationAdd_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("PatchOperationAdd(xpath=" + ___xpath + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }
        
        static void Postfix(bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationAdd(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    PatchManager.errors.Add("PatchOperationAdd(xpath=" + ___xpath + "): Error");
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationAddModExtension))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationAddModExtension_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("PatchOperationAddModExtension(xpath=" + ___xpath + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationAddModExtension(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    PatchManager.errors.Add("PatchOperationAddModExtension(xpath=" + ___xpath + "): Error");
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationAttributeAdd))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationAttributeAdd_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, ref string ___attribute, ref string ___value, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("PatchOperationAttributeAdd(xpath=" + ___xpath + ", attribute=" + ___attribute + ", value=" + ___value + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref string ___xpath, ref string ___attribute, ref string ___value, XmlDocument xml)
        {
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationAttributeAdd(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    PatchManager.errors.Add("PatchOperationAttributeAdd(xpath=" + ___xpath + ", attribute=" + ___attribute + ", value=" + ___value + "): Error");
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationAttributeRemove))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationAttributeRemove_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, ref string ___attribute, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("PatchOperationAttributeRemove(xpath=" + ___xpath + ", attribute=" + ___attribute + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref string ___xpath, ref string ___attribute, XmlDocument xml)
        {
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationAttributeRemove(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    PatchManager.errors.Add("PatchOperationAttributeRemove(xpath=" + ___xpath + ", attribute=" + ___attribute + "): Error ");
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationAttributeSet))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationAttributeSet_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, ref string ___value, ref string ___attribute, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("PatchOperationAttributeSet(xpath=" + ___xpath + ", attribute=" + ___attribute + ", value=" + ___value + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref string ___xpath, ref string ___attribute, ref string ___value, XmlDocument xml)
        {
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationAttributeSet(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    PatchManager.errors.Add("PatchOperationAttributeSet(xpath=" + ___xpath + ", attribute=" + ___attribute + ", value=" + ___value + "): Error");
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationConditional))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationConditional_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("PatchOperationConditional(xpath=" + ___xpath + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (!__result)
            {
                PatchManager.errors.Add("PatchOperationConditional(xpath=" + ___xpath + "): Error");
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationFindMod))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationFindMod_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("PatchOperationFindMod: " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(List<string> ___mods, ref bool __result, XmlDocument xml)
        {
            int c = 0;
            if (!__result)
            {
                string str = "nomatch";
                for (int i = 0; i < ___mods.Count; i++)
                {
                    if (ModLister.HasActiveModWithName(___mods[i]))
                    {
                        c = i;
                        str = "match";
                        break;
                    }
                }
                PatchManager.errors.Add("PatchOperationFindMod("+___mods[c]+"): Error in <" + str + ">");
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationInsert))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationInsert_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("PatchOperationInsert(xpath=" + ___xpath + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationInsert(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    PatchManager.errors.Add("PatchOperationInsert(xpath=" + ___xpath + "): Error");
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationRemove))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationRemove_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("PatchOperationRemove(xpath=" + ___xpath + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationRemove(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    PatchManager.errors.Add("PatchOperationRemove(xpath=" + ___xpath + "): Error");
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationReplace))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationReplace_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("PatchOperationReplace(xpath = " + ___xpath + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationReplace(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    PatchManager.errors.Add("PatchOperationReplace(xpath=" + ___xpath + "): Error");
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationSequence))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationSequence_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("PatchOperationSequence: " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref PatchOperation ___lastFailedOperation, ref List<PatchOperation> ___operations, XmlDocument xml)
        {
            if (!__result)
            {
                int num = 0;
                int c = 0;
                foreach(PatchOperation operation in ___operations)
                {
                    c++;
                    if (operation == ___lastFailedOperation && ___lastFailedOperation.GetType() != typeof(PatchOperationTest))
                    {
                        num = c;
                    }                    
                }
                if (___operations != null && ___operations.Count > 0 && num != 0)
                    PatchManager.errors.Add("PatchOperationSequence: Error in the operation at position=" + num.ToString());
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationSetName))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationSetName_Patch
    {
        static Exception Finalizer(Exception __exception, ref bool __result, ref string ___xpath, ref string ___name, XmlDocument xml)
        {
            if (__exception != null)
            {
                PatchManager.errors.Add("PatchOperationSetName(xpath=" + ___xpath + ", name=" + ___name + "): " + __exception.Message);
                __result = false;
            }
            return null;
        }

        static void Postfix(ref bool __result, ref string ___xpath, ref string ___name, XmlDocument xml)
        {
            if (!__result)
            {
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationSetName(xpath=" + ___xpath + "): Failed to find a node with the given xpath");
                else
                    PatchManager.errors.Add("PatchOperationSetName(xpath=" + ___xpath + ", name=" + ___name + "): Error");
            }
        }
    }
}
