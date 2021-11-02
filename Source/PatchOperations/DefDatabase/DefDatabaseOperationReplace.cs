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
            List<ObjectContainer> objects = SelectObjects(defType != null ? defType + "/[defName=\"" + defName + "\"]/" + objPath : objPath);
            if (objects.Count == 0)
            {
                Error("Failed to find an object with the given path");
                return false;
            }
            foreach (ObjectContainer obj in objects)
            {
                if (obj.parent.GetType().HasGenericDefinition(typeof(List<>)))
                {
                    int index = (int)AccessTools.Method(obj.parent.GetType(), "IndexOf").Invoke(obj.parent, new object[] { obj.child });
                    PropertyInfo indexer = AccessTools.Property(obj.parent.GetType(), "Item");
                    indexer.SetValue(obj.parent, NodeToObject(value.node.FirstChild, obj.child.GetType()), new object[] { index });
                }
                else
                {
                    List<string> list = CreateComponents(objPath);
                    AccessTools.Field(obj.parent.GetType(), list[list.Count - 1]).SetValue(obj.parent, NodeToObject(value.node.FirstChild, obj.child.GetType()));
                }
            }
            return true;
        }
    }
}