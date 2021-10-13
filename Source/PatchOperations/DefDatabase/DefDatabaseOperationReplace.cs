using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace XmlExtensions
{
    public class DefDatabaseOperationReplace : DefDatabaseOperation
    {
        public string defType;
        public string defName;
        public string objPath;
        public XmlContainer value;

        protected override void SetException()
        {
            CreateExceptions(defType, "defType", defName, "defName");
        }

        protected override bool DoPatch()
        {
            object def = GetDef(defType, defName);
            object obj = FindObject(def, objPath);
            if (obj == null)
            {
                Error("Failed to find an object with the given path");
                return false;
            }
            if (parentObj.GetType().HasGenericDefinition(typeof(List<>)))
            {
                PropertyInfo indexer = AccessTools.Property(parentObj.GetType(), "Item");
                indexer.SetValue(parentObj, NodeToObject(value.node.FirstChild, obj.GetType()), new object[] { listIndex });
            }
            else
            {
                List<string> list = CreateComponents(objPath);
                AccessTools.Field(parentObj.GetType(), list[list.Count - 1]).SetValue(parentObj, NodeToObject(value.node.FirstChild, obj.GetType()));
            }
            return true;
        }
    }
}