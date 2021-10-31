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
            List<object> objects = SelectObjects(defType != null ? defType + "/[defName=\"" + defName + "\"]/" + objPath : objPath);
            if (objects.Count == 0)
            {
                Error("Failed to find an object with the given path");
                return false;
            }
            foreach (object obj in objects)
            {
                if (parentObjDict[obj].GetType().HasGenericDefinition(typeof(List<>)))
                {
                    AccessTools.Method(parentObjDict[obj].GetType(), "Remove").Invoke(parentObjDict[obj], new object[] { obj });
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