﻿using HarmonyLib;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class DefDatabaseOperationAdd : DefDatabaseOperation
    {
        public string defType;
        public string defName;
        public string path;
        public XmlContainer value;
        private Order order = Order.Append;

        private enum Order
        {
            Append,
            Prepend
        }

        protected override void SetException()
        {
            CreateExceptions(defType, "defType", defName, "defName");
        }

        protected override bool DoPatch()
        {
            object def = GetDef(defType, defName);
            object obj = FindObject(def, path);
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
            return true;
        }
    }
}