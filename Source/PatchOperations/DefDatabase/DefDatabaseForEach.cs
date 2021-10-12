using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace XmlExtensions
{
    public class DefDatabaseForEach : DefDatabaseOperation
    {
        public string defType;
        public string brackets = "{}";
        public string storeIn;
        public XmlContainer apply;

        protected override bool DoPatch()
        {
            Type t = GetDefType(defType);
            if (t == null)
            {
                Error("Failed to find the given defType");
                return false;
            }
            object list = t.GetProperty("AllDefsListForReading").GetValue(null);
            PropertyInfo indexer = AccessTools.Property(list.GetType(), "Item");
            int count = (int)list.GetType().GetProperty("Count").GetValue(list);
            for (int i = 0; i < count; i++)
            {
                object def = indexer.GetValue(list, new object[] { i });
                string defName = (string)Traverse.Create(def).Field("defName").GetValue();
                XmlContainer newContainer = Helpers.SubstituteVariableXmlContainer(apply, storeIn, defName, brackets);
                if (!RunPatches(newContainer, null))
                {
                    return false;
                }
            }
            return true;
        }
    }
}