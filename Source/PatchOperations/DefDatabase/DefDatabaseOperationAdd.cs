﻿using HarmonyLib;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    // TODO: Eventually get rid of the extra tags
    internal class DefDatabaseOperationAdd : DefDatabaseOperation
    {
        public string defType;
        public string defName;
        public string objPath;
        public string objPath2;
        public XmlContainer value;
        private Order order = Order.Append;

        private enum Order
        {
            Append,
            Prepend
        }

        protected override bool DoPatch()
        {
            List<ObjectContainer> objects = DefDatabaseSearcher.SelectObjects(defType != null ? defType + "/[defName=\"" + defName + "\"]/" + objPath : objPath);
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
                if (obj.value.GetType().HasGenericDefinition(typeof(List<>)) || obj.value.GetType().HasGenericDefinition(typeof(HashSet<>)))
                {
                    
                    if (order == Order.Append || obj.value.GetType().HasGenericDefinition(typeof(HashSet<>)))
                    {
                        foreach (object objToAdd in objectsToAdd)
                        {
                            AccessTools.Method(obj.value.GetType(), "Add").Invoke(obj.value, new object[] { objToAdd });
                        }
                    }
                    else
                    {
                        for (int i = objectsToAdd.Count - 1; i >= 0; i--)
                        {
                            AccessTools.Method(obj.value.GetType(), "Insert").Invoke(obj.value, new object[] { 0, objectsToAdd[i] });
                        }
                    }
                }
                else
                {
                    Error("You can only Add to Lists or HashSets");
                    return false;
                }
            }
            return true;
        }
    }
}