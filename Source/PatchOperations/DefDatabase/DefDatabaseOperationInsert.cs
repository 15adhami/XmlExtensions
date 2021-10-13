using HarmonyLib;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class DefDatabaseOperationInsert : DefDatabaseOperationPathed
    {
        public XmlContainer value;
        private Order order = Order.Append;

        private enum Order
        {
            Append,
            Prepend
        }

        protected override bool DoPatch()
        {
            object obj = FindObject(def, objPath);
            if (obj == null)
            {
                Error("Failed to find an object with the given path");
                return false;
            }
            if (obj.GetType().HasGenericDefinition(typeof(List<>)))
            {
                XmlNodeList nodes = value.node.ChildNodes;
                if (order == Order.Append)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        AccessTools.Method(obj.GetType(), "Insert").Invoke(obj, new object[] { listIndex + 1, NodeToObject(nodes[i], obj.GetType().GetGenericArguments()[0]) });
                    }
                }
                else
                {
                    for (int i = nodes.Count - 1; i >= 0; i--)
                    {
                        AccessTools.Method(obj.GetType(), "Insert").Invoke(obj, new object[] { listIndex, NodeToObject(nodes[i], obj.GetType().GetGenericArguments()[0]) });
                    }
                }
            }
            else
            {
                Error("You can only Insert to lists");
                return false;
            }
            return true;
        }
    }
}