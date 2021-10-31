using HarmonyLib;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class DefDatabaseOperationAdd : DefDatabaseOperation
    {
        public string defType;
        public string defName;
        public string objPath;
        public XmlContainer value;
        private Order order = Order.Append;

        private enum Order
        {
            Append,
            Prepend
        }

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
                if (obj.GetType().HasGenericDefinition(typeof(List<>)))
                {
                    XmlNodeList nodes = value.node.ChildNodes;
                    if (order == Order.Append)
                    {
                        foreach (XmlNode node in nodes)
                        {
                            AccessTools.Method(obj.GetType(), "Add").Invoke(obj, new object[] { NodeToObject(node, obj.GetType().GetGenericArguments()[0]) });
                        }
                    }
                    else
                    {
                        for (int i = nodes.Count - 1; i >= 0; i--)
                        {
                            AccessTools.Method(obj.GetType(), "Insert").Invoke(obj, new object[] { 0, NodeToObject(nodes[i], obj.GetType().GetGenericArguments()[0]) });
                        }
                    }
                }
                else
                {
                    Error("You can only Add to lists");
                    return false;
                }
            }
            return true;
        }
    }
}