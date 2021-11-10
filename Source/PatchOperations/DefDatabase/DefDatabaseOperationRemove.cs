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
            List<ObjectContainer> objects = DefDatabaseSearcher.SelectObjects(defType != null ? defType + "/[defName=\"" + defName + "\"]/" + objPath : objPath);
            if (objects.Count == 0)
            {
                XPathError("objPath");
                return false;
            }
            foreach (ObjectContainer obj in objects)
            {
                if (obj.parent.value.GetType().HasGenericDefinition(typeof(List<>)) || obj.parent.value.GetType().HasGenericDefinition(typeof(HashSet<>)))
                {
                    AccessTools.Method(obj.parent.value.GetType(), "Remove").Invoke(obj.parent.value, new object[] { obj.value });
                }
                else
                {
                    Error("You can only Remove from Lists or HashSets");
                    return false;
                }
            }
            return true;
        }
    }
}