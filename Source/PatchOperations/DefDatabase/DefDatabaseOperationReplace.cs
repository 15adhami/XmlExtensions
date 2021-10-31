using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace XmlExtensions
{
    internal class DefDatabaseOperationReplace : DefDatabaseOperation
    {
        public string defType;
        public string defName;
        public string objPath;
        public XmlContainer value;

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
                    int index = (int)AccessTools.Method(parentObjDict[obj].GetType(), "IndexOf").Invoke(parentObjDict[obj], new object[] { obj });
                    PropertyInfo indexer = AccessTools.Property(parentObjDict[obj].GetType(), "Item");
                    indexer.SetValue(parentObjDict[obj], NodeToObject(value.node.FirstChild, obj.GetType()), new object[] { index });
                }
                else
                {
                    List<string> list = CreateComponents(objPath);
                    AccessTools.Field(parentObjDict[obj].GetType(), list[list.Count - 1]).SetValue(parentObjDict[obj], NodeToObject(value.node.FirstChild, obj.GetType()));
                }
            }
            return true;
        }
    }
}