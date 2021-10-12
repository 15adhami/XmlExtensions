using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace XmlExtensions
{
    public class DefDatabaseOperationAdd : DefDatabaseOperation
    {
        public string defType;
        public string defName;
        public string path;
        public XmlContainer value;

        protected override void SetException()
        {
            CreateExceptions(defType, "defType", defName, "defName");
        }

        protected override bool DoPatch()
        {
            object def = GetDef(defType, defName);
            object obj = FindObject(def, path);
            if (obj == null)
            {
                Error("Failed to find an object with the given path");
                return false;
            }
            if (obj.GetType().HasGenericDefinition(typeof(List<>)))
            {
                AccessTools.Method(obj.GetType(), "Add").Invoke(obj, new object[] { NodeToObject(value.node.FirstChild, obj.GetType().GetGenericArguments()[0]) });
            }
            else
            {
                Error("You can only Add to lists");
                return false;
            }
            return true;
        }
    }
}