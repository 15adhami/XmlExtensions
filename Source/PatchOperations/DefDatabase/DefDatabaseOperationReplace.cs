﻿using HarmonyLib;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class DefDatabaseOperationReplace : DefDatabaseOperation
    {
        public string defType;
        public string defName;
        public string objPath;
        public string objPath2;
        public XmlContainer value;

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
                    objectsToAdd.Add(DefDatabaseSearcher.NodeToObject(node, objects[0].value.GetType()));
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
                if (obj.parent.value.GetType().HasGenericDefinition(typeof(List<>)))
                {
                    System.Reflection.MethodInfo method = AccessTools.Method(obj.parent.value.GetType(), "IndexOf", new System.Type[] { obj.parent.value.GetType().GenericTypeArguments[0] });
                    int index = (int)method.Invoke(obj.parent.value, new object[] { obj.value });
                    AccessTools.Method(obj.parent.value.GetType(), "RemoveAt").Invoke(obj.parent.value, new object[] { index });
                    foreach (object objToAdd in objectsToAdd)
                    {
                        AccessTools.Method(obj.parent.value.GetType(), "Insert").Invoke(obj.parent.value, new object[] { index, objToAdd });
                    }
                }
                else if (obj.parent.value.GetType().HasGenericDefinition(typeof(HashSet<>)))
                {
                    AccessTools.Method(obj.parent.value.GetType(), "Remove").Invoke(obj.parent.value, new object[] { obj.value });
                    foreach (object objToAdd in objectsToAdd)
                    {
                        AccessTools.Method(obj.parent.value.GetType(), "Add").Invoke(obj.parent.value, new object[] { objToAdd });
                    }
                }
                else
                {
                    List<string> list = DefDatabaseSearcher.CreateComponents(objPath);
                    AccessTools.Field(obj.parent.value.GetType(), list[list.Count - 1]).SetValue(obj.parent.value, objectsToAdd[0]);
                }
            }
            return true;
        }
    }
}