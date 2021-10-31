using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace XmlExtensions
{
    internal class DefDatabaseOperationSetReference : DefDatabaseOperation
    {
        public string objPath1;
        public string objPath2;

        protected override bool DoPatch()
        {
            List<object> objects2 = SelectObjects(objPath2);
            if (objects2.Count == 0)
            {
                Error("Failed to find an object referenced by <objPath2>");
                return false;
            }
            List<object> objects1 = SelectObjects(objPath1);
            if (objects1.Count == 0)
            {
                Error("Failed to find an object referenced by <objPath1>");
                return false;
            }
            foreach (object obj in objects1)
            {
                object parentObj = parentObjDict[obj];
                List<string> list = CreateComponents(objPath1);
                FieldInfo field = AccessTools.Field(parentObj.GetType(), list[list.Count - 1]);
                field.SetValue(parentObj, objects2[0]);
            }
            return true;
        }
    }
}