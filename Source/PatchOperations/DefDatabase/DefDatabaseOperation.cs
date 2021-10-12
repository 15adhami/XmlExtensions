using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public abstract class DefDatabaseOperation : PatchOperationExtended
    {
        public object parentObj;
        protected int listIndex = -1;

        protected object GetDef(string defType, string defName)
        {
            return GetDefType(defType).GetMethod("GetNamed").Invoke(null, new object[] { defName, false });
        }

        protected sealed override bool Patch(XmlDocument xml)
        {
            if (PatchManager.applyingPatches)
            {
                PatchManager.delayedPatches.Add(this);
                return true;
            }
            else
            {
                try
                {
                    return DoPatch();
                }
                catch (Exception e)
                {
                    Error(e.Message);
                    return false;
                }
            }
        }

        protected List<string> CreateComponents(string path)
        {
            List<string> components = new List<string>();
            components.Add("");
            int depth = 0;
            foreach (char c in path)
            {
                if (c == '[')
                {
                    components[components.Count - 1] += c;
                    depth++;
                }
                else if (c == ']')
                {
                    components[components.Count - 1] += c;
                    depth--;
                }
                else if (c == '/' && depth == 0)
                {
                    components.Add("");
                }
                else if (!(c == ' ' && depth == 0))
                {
                    components[components.Count - 1] += c;
                }
            }
            return components;
        }

        private string GetClass(string component)
        {
            string str = "";
            int count = 0;
            foreach (char c in component)
            {
                if (c == '\"')
                {
                    count++;
                }
                if (c != '\"' && count == 1)
                {
                    str += c;
                }
            }
            return str;
        }

        private string RemoveBrackets(string str)
        {
            string temp = "";
            foreach (char c in str)
            {
                if (c != '[' && c != ']')
                {
                    temp += c;
                }
            }
            return temp;
        }

        protected Type GetDefType(string defType)
        {
            Type tempType = GenTypes.GetTypeInAnyAssembly(defType, "Verse");
            if (tempType == null)
            {
                tempType = GenTypes.GetTypeInAnyAssembly(defType, "RimWorld");
                if (tempType == null)
                {
                    return null;
                }
            }
            Type genericType = typeof(DefDatabase<>).MakeGenericType(new Type[] { tempType });
            return genericType;
        }

        // TODO: Add SearchDict

        protected object SearchList(object list, string component)
        {
            object item = null;
            ICollection col = list as ICollection;
            int count = col.Count;
            string classTemp = GetClass(component);
            PropertyInfo indexer = AccessTools.Property(list.GetType(), "Item");
            if (indexer == null)
            {
                return null;
            }
            if (classTemp.Length > 1)
            {
                Type classType = GenTypes.GetTypeInAnyAssembly(classTemp, "RimWorld");
                if (classType == null)
                {
                    return null;
                }
                for (int i = 0; i < count; i++)
                {
                    object tempItem = indexer.GetValue(list, new object[] { i });
                    if (tempItem.GetType() == classType)
                    {
                        item = tempItem;
                        listIndex = i;
                        break;
                    }
                }
            }
            else
            {
                listIndex = int.Parse(RemoveBrackets(component)) - 1;
                item = indexer.GetValue(list, new object[] { listIndex });
            }
            return item;
        }

        // TODO: Try only movespeed
        protected object FindObject(object parent, string path)
        {
            if (parent == null)
            {
                return null;
            }
            List<string> components = CreateComponents(path);
            FieldInfo fieldInfo = null;
            object obj = parent;
            Type tempType = obj.GetType();
            int count = 0;
            foreach (string component in components)
            {
                if (count == components.Count - 1)
                {
                    parentObj = obj;
                }
                count++;
                if (tempType.HasGenericDefinition(typeof(List<>)))
                {
                    obj = SearchList(obj, component);
                    if (obj == null)
                    {
                        return null;
                    }
                    tempType = obj.GetType();
                }
                else
                {
                    fieldInfo = AccessTools.Field(tempType, component);
                    if (fieldInfo == null)
                    {
                        return null;
                    }
                    obj = fieldInfo.GetValue(obj);
                    if (obj == null)
                    {
                        return null;
                    }
                    tempType = obj.GetType();
                    if (tempType == null)
                    {
                        return null;
                    }
                }
            }
            return obj;
        }

        protected object NodeToObject(XmlNode node, Type type)
        {
            return typeof(DirectXmlToObject).GetMethod("ObjectFromXml").MakeGenericMethod(new Type[] { type }).Invoke(null, new object[] { node, false });
        }

        protected abstract bool DoPatch();
    }
}