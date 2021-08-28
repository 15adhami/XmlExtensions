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
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationAdd: Error in finding a node with <xpath>=" + ___xpath);
                else
                    PatchManager.errors.Add("PatchOperationAdd: Error (<xpath>=" + ___xpath + ")");
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
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationAddModExtension: Error in finding a node with <xpath>=" + ___xpath);
                else
                    PatchManager.errors.Add("PatchOperationAddModExtension: Error (<xpath>=" + ___xpath+")");
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
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationAttributeAdd: Error in finding a node with <xpath>=" + ___xpath);
                else
                    PatchManager.errors.Add("PatchOperationAttributeAdd: Error (<attribute>=" + ___attribute + ", <value>=" + ___value + ", <xpath>=" + ___xpath+")");
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
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationAttributeRemove: Error in finding a node with <xpath>=" + ___xpath);
                else
                    PatchManager.errors.Add("PatchOperationAttributeRemove: Error (<attribute>=" + ___attribute + ", <xpath>=" + ___xpath+")");
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
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationAttributeSet: Error in finding a node with <xpath>=" + ___xpath);
                else
                    PatchManager.errors.Add("PatchOperationAttributeSet: Error (<attribute=" + ___attribute + ", <value>=" + ___value + ", <xpath>=" + ___xpath+")");
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
                PatchManager.errors.Add("PatchOperationConditional: Error (<xpath>=" + ___xpath+")");
            }
        }
    }

    [HarmonyPatch(typeof(PatchOperationFindMod))]
    [HarmonyPatch("ApplyWorker")]
    static class PatchOperationFindMod_Patch
    {
        static void Postfix(List<string> ___mods, ref bool __result, XmlDocument xml)
        {
            if (!__result)
            {
                string str = "nomatch";
                for (int i = 0; i < ___mods.Count; i++)
                {
                    if (ModLister.HasActiveModWithName(___mods[i]))
                    {
                        str = "match";
                        break;
                    }
                }
                PatchManager.errors.Add("PatchOperationFindMod: Error in <" + str + ">");
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
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationInsert: Error in finding a node with <xpath>=" + ___xpath);
                else
                    PatchManager.errors.Add("PatchOperationInsert: Error (<xpath>=" + ___xpath+")");
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
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationRemove: Error in finding a node with <xpath>=" + ___xpath);
                else
                    PatchManager.errors.Add("PatchOperationRemove: Error (<xpath>=" + ___xpath+")");
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
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationReplace: Error in finding a node with <xpath>=" + ___xpath);
                else
                    PatchManager.errors.Add("PatchOperationReplace: Error (<xpath>=" + ___xpath+")");
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
                    PatchManager.errors.Add("PatchOperationSequence: Error in the operation at position=" + num.ToString());
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
                if (xml.SelectSingleNode(___xpath) == null)
                    PatchManager.errors.Add("PatchOperationSetName: Error in finding a node with <xpath>=" + ___xpath);
                else
                    PatchManager.errors.Add("PatchOperationSetName: Error (<name>=" + ___name + ", <xpath>=" + ___xpath+")");
            }
        }
    }
}
