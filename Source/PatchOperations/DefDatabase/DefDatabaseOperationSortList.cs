using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class DefDatabaseOperationSortList : DefDatabaseOperation
    {
        public string objPath;
        public string objPathLocal = "*";
        public bool reverse = false;
        public bool nonNumeric = false;

        protected override bool DoPatch()
        {
            List<ObjectContainer> lists = DefDatabaseSearcher.SelectObjects(objPath);
            if (lists.Count == 0)
            {
                XPathError("objPath");
                return false;
            }
            for (int i = 0; i < lists.Count; i++)
            {
                ObjectContainer list = lists[i];
                List<ObjectContainer> objList = DefDatabaseSearcher.SelectObjects(list, "*");
                objList.Sort(delegate (ObjectContainer obj1, ObjectContainer obj2)
                {
                    if (nonNumeric)
                    {
                        object tempObj1 = DefDatabaseSearcher.SelectObjects(obj1, objPathLocal)[0].value;
                        object tempObj2 = DefDatabaseSearcher.SelectObjects(obj2, objPathLocal)[0].value;
                        return tempObj1.ToString().CompareTo(tempObj2.ToString());
                    }
                    else
                    {
                        float tempObj1 = float.Parse(DefDatabaseSearcher.SelectObjects(obj1, objPathLocal)[0].value.ToString());
                        float tempObj2 = float.Parse(DefDatabaseSearcher.SelectObjects(obj2, objPathLocal)[0].value.ToString());
                        if (tempObj1 < tempObj2)
                        {
                            return -1;
                        }
                        else if (float.Parse(tempObj1.ToString()) == float.Parse(tempObj2.ToString()))
                        {
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
                );
                if (reverse)
                {
                    objList.Reverse();
                }
                object newList = Activator.CreateInstance(list.value.GetType());
                foreach(ObjectContainer temp in objList)
                {
                    AccessTools.Method(newList.GetType(), "Add").Invoke(newList, new object[] { temp.value });
                }
                List<string> components = DefDatabaseSearcher.CreateComponents(list.objPath);
                FieldInfo fieldInfo = AccessTools.Field(list.parent.value.GetType(), components[components.Count - 1]);
                fieldInfo.SetValue(list.parent.value, newList);
            }
            return true;
        }

        protected override void SetException()
        {
            CreateExceptions(objPath, "objPath", objPathLocal, "objPathLocal");
        }
    }
}