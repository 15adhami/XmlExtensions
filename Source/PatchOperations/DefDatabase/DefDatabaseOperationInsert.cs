using HarmonyLib;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class DefDatabaseOperationInsert : DefDatabaseOperation
    {
        public string objPath;
        public string objPath2;
        public XmlContainer value;
        private Order order = Order.Prepend;

        private enum Order
        {
            Append,
            Prepend
        }

        protected override bool DoPatch()
        {
            List<ObjectContainer> objects = DefDatabaseSearcher.SelectObjects(objPath);
            if (objects.Count == 0)
            {
                XPathError("objPath");
                return false;
            }
            List<object> objectsToAdd = new();
            if (value != null)
            {
                XmlNodeList nodes = value.node.ChildNodes;
                foreach (XmlNode node in nodes)
                {
                    objectsToAdd.Add(DefDatabaseSearcher.NodeToObject(node, objects[0].value.GetType().GetGenericArguments()[0]));
                }
            }
            else if (objPath2 != null)
            {
                List<ObjectContainer> objs = DefDatabaseSearcher.SelectObjects(objPath2);
                if (objs.Count == 0)
                {
                    XPathError("objPath2");
                    return false;
                }
                foreach (ObjectContainer obj in objs)
                {
                    objectsToAdd.Add(obj.value);
                }
            }
            foreach (ObjectContainer obj in objects)
            {
                object parentObj = obj.parent;
                if (parentObj.GetType().HasGenericDefinition(typeof(List<>)))
                {
                    if (order == Order.Append)
                    {
                        foreach(object objToAdd in objectsToAdd)
                        {
                            int index = (int)AccessTools.Method(parentObj.GetType(), "IndexOf").Invoke(parentObj, new object[] { obj.value });
                            AccessTools.Method(parentObj.GetType(), "Insert").Invoke(parentObj, new object[] { index + 1, objToAdd });
                        }
                    }
                    else
                    {
                        for (int i = objectsToAdd.Count - 1; i >= 0; i--)
                        {
                            int index = (int)AccessTools.Method(parentObj.GetType(), "IndexOf").Invoke(parentObj, new object[] { obj.value });
                            AccessTools.Method(parentObj.GetType(), "Insert").Invoke(parentObj, new object[] { index, objectsToAdd[i] });
                        }
                    }
                }
                else if (obj.value.GetType().HasGenericDefinition(typeof(HashSet<>)))
                {
                    foreach (object objToAdd in objectsToAdd)
                    {
                        AccessTools.Method(obj.value.GetType(), "Add").Invoke(obj.value, new object[] { objToAdd });
                    }
                }
                else
                {
                    Error("You can only Insert to lists");
                    return false;
                }
            }
            
            return true;
        }
    }
}