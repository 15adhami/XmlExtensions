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
                XPathError("objPath");
                return false;
            }
            foreach (ObjectContainer obj in objects)
            {
                if (obj.parent.value.GetType().HasGenericDefinition(typeof(List<>)))
                {
                    int index = (int)AccessTools.Method(obj.parent.value.GetType(), "IndexOf").Invoke(obj.parent.value, new object[] { obj.value });
                    PropertyInfo indexer = AccessTools.Property(obj.parent.value.GetType(), "Item");
                    indexer.SetValue(obj.parent.value, NodeToObject(value.node.FirstChild, obj.value.GetType()), new object[] { index });
                }
                else
                {
                    List<string> list = CreateComponents(objPath);
                    AccessTools.Field(obj.parent.value.GetType(), list[list.Count - 1]).SetValue(obj.parent.value, NodeToObject(value.node.FirstChild, obj.value.GetType()));
                }
            }
            return true;
        }
    }
}