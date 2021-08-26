using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HarmonyLib;
using Verse;

namespace XmlExtensions
{
    [HarmonyPatch(typeof(PatchOperationAdd))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationAdd_Patch
    {
        static void Postfix(bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (!__result)
            {
                PatchManager.errors.Add("Error in PatchOperationAdd with the xpath: " + ___xpath);
            }
            else
            {
                PatchManager.errors.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationAddModExtension))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationAddModExtension_Patch
    {
        static void Postfix(ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (!__result)
            {
                PatchManager.errors.Add("Error in PatchOperationAddModExtension with the xpath: " + ___xpath);
            }
            else
            {
                PatchManager.errors.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationAttributeAdd))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationAttributeAdd_Patch
    {
        static void Postfix(ref bool __result, ref string ___xpath, ref string ___attribute, ref string ___value, XmlDocument xml)
        {
            if (!__result)
            {
                PatchManager.errors.Add("Error in PatchOperationAttributeAdd with the attribute: " + ___attribute + ", the value: " + ___value + ", and the xpath: " + ___xpath);
            }
            else
            {
                PatchManager.errors.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationAttributeRemove))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationAttributeRemove_Patch
    {
        static void Postfix(ref bool __result, ref string ___xpath, ref string ___attribute, XmlDocument xml)
        {
            if (!__result)
            {
                PatchManager.errors.Add("Error in PatchOperationAttributeRemove with the attribute: " + ___attribute + ", and the xpath: " + ___xpath);
            }
            else
            {
                PatchManager.errors.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationAttributeSet))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationAttributeSet_Patch
    {
        static void Postfix(ref bool __result, ref string ___xpath, ref string ___attribute, ref string ___value, XmlDocument xml)
        {
            if (!__result)
            {
                PatchManager.errors.Add("Error in PatchOperationAttributeSet with the attribute: " + ___attribute + ", the value: " + ___value + ", and the xpath: " + ___xpath);
            }
            else
            {
                PatchManager.errors.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationConditional))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationConditional_Patch
    {
        static void Postfix(ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (!__result)
            {
                PatchManager.errors.Add("Error in PatchOperationConditional with the xpath: " + ___xpath);
            }
            else
            {
                PatchManager.errors.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationFindMod))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationFindMod_Patch
    {
        static void Postfix(ref bool __result, XmlDocument xml)
        {
            if (!__result)
            {
                PatchManager.errors.Add("Error in PatchOperationFindMod");
            }
            else
            {
                PatchManager.errors.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationInsert))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationInsert_Patch
    {
        static void Postfix(ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (!__result)
            {
                PatchManager.errors.Add("Error in PatchOperationInsert with the xpath: " + ___xpath);
            }
            else
            {
                PatchManager.errors.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationRemove))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationRemove_Patch
    {
        static void Postfix(ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (!__result)
            {
                PatchManager.errors.Add("Error in PatchOperationRemove with the xpath: " + ___xpath);
            }
            else
            {
                PatchManager.errors.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationReplace))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationReplace_Patch
    {
        static void Postfix(ref bool __result, ref string ___xpath, XmlDocument xml)
        {
            if (!__result)
            {
                PatchManager.errors.Add("Error in PatchOperationReplace with the xpath: " + ___xpath);
            }
            else
            {
                PatchManager.errors.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationSequence))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationSequence_Patch
    {
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
                    PatchManager.errors.Add("Error in PatchOperationSequence in the operation with index: " + num.ToString());
            }
            else
            {
                PatchManager.errors.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationSetName))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationSetName_Patch
    {
        static void Postfix(ref bool __result, ref string ___xpath, ref string ___name, XmlDocument xml)
        {
            if (!__result)
            {
                PatchManager.errors.Add("Error in PatchOperationSetName with the name: " + ___name + ", and the xpath: " + ___xpath);
            }
            else
            {
                PatchManager.errors.Clear();
            }
        }
    }
}
