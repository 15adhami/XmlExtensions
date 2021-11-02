using HarmonyLib;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class DefDatabaseOperationInsert : DefDatabaseOperation
    {
        public string objPath;
        public XmlContainer value;
        private Order order = Order.Prepend;

        private enum Order
        {
            Append,
            Prepend
        }

        protected override bool DoPatch()
        {
            List<ObjectContainer> objects = SelectObjects(objPath);
            if (objects.Count == 0)
            {
                Error("Failed to find an object with the given path");
                return false;
            }
            foreach (ObjectContainer obj in objects)
            {
                object parentObj = obj.parent;
                if (parentObj.GetType().HasGenericDefinition(typeof(List<>)))
                {
                    XmlNodeList nodes = value.node.ChildNodes;
                    if (order == Order.Append)
                    {
                        for (int i = 0; i < nodes.Count; i++)
                        {
                            int index = (int)AccessTools.Method(parentObj.GetType(), "IndexOf").Invoke(parentObj, new object[] { obj.child });
                            AccessTools.Method(parentObj.GetType(), "Insert").Invoke(parentObj, new object[] { index + 1, NodeToObject(nodes[i], parentObj.GetType().GetGenericArguments()[0]) });
                        }
                    }
                    else
                    {
                        for (int i = nodes.Count - 1; i >= 0; i--)
                        {
                            int index = (int)AccessTools.Method(parentObj.GetType(), "IndexOf").Invoke(parentObj, new object[] { obj.child });
                            AccessTools.Method(parentObj.GetType(), "Insert").Invoke(parentObj, new object[] { index, NodeToObject(nodes[i], parentObj.GetType().GetGenericArguments()[0]) });
                        }
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