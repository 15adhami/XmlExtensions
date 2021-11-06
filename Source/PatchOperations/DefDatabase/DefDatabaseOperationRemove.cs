using HarmonyLib;
using System.Collections.Generic;
using Verse;

namespace XmlExtensions
{
    internal class DefDatabaseOperationRemove : DefDatabaseOperation
    {
        public string defType;
        public string defName;
        public string objPath;

        protected override bool DoPatch()
        {
            List<ObjectContainer> objects = SelectObjects(defType != null ? defType + "/[defName=\"" + defName + "\"]/" + objPath : objPath);
            if (objects.Count == 0)
            {
                Error("Failed to find an object with the given path");
                return false;
            }
            foreach (ObjectContainer obj in objects)
            {
                if (obj.parent.value.GetType().HasGenericDefinition(typeof(List<>)))
                {
                    AccessTools.Method(obj.parent.value.GetType(), "Remove").Invoke(obj.parent.value, new object[] { obj.value });
                }
                else
                {
                    Error("You can only Remove from lists");
                    return false;
                }
            }
            return true;
        }
    }
}