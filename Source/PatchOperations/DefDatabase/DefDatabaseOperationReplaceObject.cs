using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class DefDatabaseOperationReplaceObject : DefDatabaseOperationPathed
    {
        public string defType2;
        public string defName2;
        public string objPath2;

        protected override bool DoPatch()
        {
            object obj = FindObject(def, objPath);
            object parent = parentObj;
            if (obj == null)
            {
                Error("Failed to find an object with the given objPath");
                return false;
            }
            List<string> list = CreateComponents(objPath);
            FieldInfo field = AccessTools.Field(parent.GetType(), list[list.Count - 1]);
            if (field == null)
            {
                Error("Failed to find an object referenced by <objPath>");
                return false;
            }
            object def2 = GetDef(defType2, defName2);
            if (def2 == null)
            {
                Error("Failed to find a Def with the given defName2");
                return false;
            }
            object newObj = FindObject(def2, objPath2);
            if (newObj == null)
            {
                Error("Failed to find an object referenced by <objPath2>");
                return false;
            }
            field.SetValue(parent, newObj);
            return true;
        }
    }
}