using HarmonyLib;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal static class DefDatabaseSearcher
    {
        public static List<ObjectContainer> SelectObjects(string objPath)
        {
            List<ObjectContainer> list = [];
            try
            {
                if (objPath == null)
                {
                    return list;
                }
                List<string> components = CreateComponents(objPath);
                Type genericType = null;
                genericType = GetDefType(components[0]);
                if (genericType == null)
                {
                    return list;
                }
                if (components.Count == 1)
                {
                    return list;
                }
                int startIndex = 1;
                if (components[1].StartsWith("[defName="))
                {
                    startIndex = 2;
                    ObjectContainer tempContainer = new ObjectContainer(GetDef(components[0], StripQuotes(components[1])));
                    tempContainer.objPath = components[0] + "/" + components[1];
                    list.Add(tempContainer);
                }
                else
                {
                    List<object> tempList = new();
                    object listObj = genericType.GetProperty("AllDefsListForReading").GetValue(null);
                    int listLength = (int)listObj.GetType().GetProperty("Count").GetValue(listObj);
                    PropertyInfo indexer = AccessTools.Property(listObj.GetType(), "Item");
                    for (int i = 0; i < listLength; i++)
                    {
                        tempList.Add(indexer.GetValue(listObj, new object[] { i }));
                    }
                    if (tempList.Count > 0)
                    {
                        ObjectContainer tempContainer = new ObjectContainer(tempList);
                        tempContainer.objPath = components[0];
                        list.Add(tempContainer);
                    }
                    else
                    {
                        return list;
                    }
                }
                FieldInfo fieldInfo = null;
                for (int i = startIndex; i < components.Count; i++)
                {
                    List<ObjectContainer> list2 = new();
                    string component = components[i];
                    foreach (ObjectContainer objC in list)
                    {
                        if (objC.value == null)
                        {
                            continue;
                        }
                        object obj = objC.value;
                        Type tempType = obj.GetType();
                        List<ObjectContainer> objectsToAdd = new();
                        if (tempType.HasGenericDefinition(typeof(List<>)))
                        {
                            List<ObjectContainer> objects = SearchList(objC, component);
                            foreach (ObjectContainer tempObj in objects)
                            {
                                objectsToAdd.Add(tempObj);
                            }
                        }
                        else
                        {
                            fieldInfo = AccessTools.Field(tempType, component);
                            if (fieldInfo == null)
                            {
                                continue;
                            }
                            ObjectContainer tempContainer = new ObjectContainer(fieldInfo.GetValue(obj), objC);
                            tempContainer.objPath = tempContainer.parent.objPath + "/" + fieldInfo.Name;
                            objectsToAdd.Add(tempContainer);
                        }
                        foreach (ObjectContainer objToAdd in objectsToAdd)
                        {
                            list2.Add(objToAdd);
                        }
                    }
                    list = list2;
                }
                return list;
            }
            catch (Exception e)
            {
                Verse.Log.Error(e.Message);
                return [];
            }
        }

        public static List<ObjectContainer> SelectObjects(ObjectContainer objContainer, string objPath)
        {
            List<ObjectContainer> list = new();
            try
            {
                if (objPath == null)
                {
                    return list;
                }
                List<string> components = CreateComponents(objPath);
                list.Add(objContainer);
                FieldInfo fieldInfo = null;
                for (int i = 0; i < components.Count; i++)
                {
                    List<ObjectContainer> list2 = new();
                    string component = components[i];
                    foreach (ObjectContainer objC in list)
                    {
                        if (objC.value == null)
                        {
                            continue;
                        }
                        object obj = objC.value;
                        Type tempType = obj.GetType();
                        List<ObjectContainer> objectsToAdd = new();
                        if (tempType.HasGenericDefinition(typeof(List<>)))
                        {
                            List<ObjectContainer> objects = SearchList(objC, component);
                            foreach (ObjectContainer tempObj in objects)
                            {
                                objectsToAdd.Add(tempObj);
                            }
                        }
                        else
                        {
                            fieldInfo = AccessTools.Field(tempType, component);
                            if (fieldInfo == null)
                            {
                                continue;
                            }
                            if (fieldInfo.GetValue(obj) == null && i + 1 < components.Count)
                            {
                                continue;
                            }
                            ObjectContainer tempContainer = new(fieldInfo.GetValue(obj), objC);
                            tempContainer.objPath = tempContainer.parent.objPath + "/" + fieldInfo.Name;
                            objectsToAdd.Add(tempContainer);
                        }
                        foreach (ObjectContainer objToAdd in objectsToAdd)
                        {
                            list2.Add(objToAdd);
                        }
                    }
                    list = list2;
                }
                return list;
            }
            catch(Exception e)
            {
                Verse.Log.Error(e.Message);
                return new List<ObjectContainer>();
            }
        }

        public static List<string> CreateComponents(string path)
        {
            List<string> components = new List<string>();
            components.Add("");
            int depth = 0;
            foreach (char c in RemoveSpaces(path))
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

        private static string RemoveSpaces(string str)
        {
            string result = "";
            bool removeSpaces = true;
            foreach (char c in str)
            {
                if (c == '\"')
                {
                    removeSpaces = !removeSpaces;
                }
                if (c == ' ')
                {
                    if (!removeSpaces)
                    {
                        result += c;
                    }
                }
                else
                {
                    result += c;
                }
            }
            return result;
        }

        private static string RemoveBrackets(string str)
        {
            string temp = str.Substring(1);
            temp = temp.Substring(0, temp.Length - 1);
            return temp;
        }

        public static Type GetDefType(string defType)
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
        private static List<ObjectContainer> SearchList(ObjectContainer list, string component)
        {
            List<ObjectContainer> listObjects = new();
            ICollection col = list.value as ICollection;
            int count = col.Count;
            string classTemp = StripQuotes(component);
            PropertyInfo indexer = AccessTools.Property(list.value.GetType(), "Item");
            int index = 0;
            if (component == "*")
            {
                for (int i = 0; i < count; i++)
                {
                    object tempItem = indexer.GetValue(list.value, new object[] { i });
                    ObjectContainer tempContainer = new(tempItem, list);
                    tempContainer.objPath = tempContainer.parent.objPath + "/[" + (i + 1).ToString() + "]";
                    listObjects.Add(tempContainer);
                }
                return listObjects;
            }
            string noBrackets = RemoveBrackets(component);
            if (int.TryParse(noBrackets, out index))
            {
                object tempItem = null;
                try
                {
                    tempItem = indexer.GetValue(list.value, new object[] { index - 1 });
                }
                catch
                {
                    return listObjects;
                }
                ObjectContainer tempContainer = new(tempItem, list);
                tempContainer.objPath = tempContainer.parent.objPath + "/[" + index.ToString() + "]";
                listObjects.Add(tempContainer);
                return listObjects;
            }
            else if (indexer == null)
            {
                return listObjects;
            }
            else if (noBrackets.StartsWith("@Class"))
            {
                Type classType = GenTypes.GetTypeInAnyAssembly(classTemp, "RimWorld");
                if (classType == null)
                {
                    classType = GenTypes.GetTypeInAnyAssembly(classTemp, "Verse");
                    if (classType == null)
                    {
                        return listObjects;
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    object tempItem = indexer.GetValue(list.value, new object[] { i });
                    if (tempItem.GetType() == classType)
                    {
                        ObjectContainer tempContainer = new(tempItem, list);
                        tempContainer.objPath = tempContainer.parent.objPath + "/[" + (i + 1).ToString() + "]";
                        listObjects.Add(tempContainer);
                    }
                }
                return listObjects;
            }
            else if (noBrackets.StartsWith("@InheritsFrom"))
            {
                Type classType = GenTypes.GetTypeInAnyAssembly(classTemp, "RimWorld");
                if (classType == null)
                {
                    classType = GenTypes.GetTypeInAnyAssembly(classTemp, "Verse");
                    if (classType == null)
                    {
                        return listObjects;
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    object tempItem = indexer.GetValue(list.value, new object[] { i });
                    if (classType.IsAssignableFrom(tempItem.GetType()))
                    {
                        ObjectContainer tempContainer = new(tempItem, list);
                        tempContainer.objPath = tempContainer.parent.objPath + "/[" + (i + 1).ToString() + "]";
                        listObjects.Add(tempContainer);
                    }
                }
                return listObjects;
            }
            else
            {
                List<string> pair = SplitEquals(noBrackets);

                for (int i = 0; i < count; i++)
                {
                    object tempItem = indexer.GetValue(list.value, new object[] { i });
                    ObjectContainer tempContainer = new(tempItem, list);
                    tempContainer.objPath = tempContainer.parent.objPath + "/[" + (i + 1).ToString() + "]";
                    if (pair.Count > 1)
                    {
                        List<ObjectContainer> left = SelectObjects(tempContainer, pair[0]);
                        if (left.Count > 0)
                        {
                            object right = AccessTools.Method(typeof(ParseHelper), "FromString", new Type[] { typeof(string) }).MakeGenericMethod(new Type[] { left[0].value.GetType() }).Invoke(null, new object[] { StripQuotes(pair[1]) });
                            if (left[0].value.Equals(right))
                            {
                                listObjects.Add(tempContainer);
                            }
                        }
                    }
                    else
                    {
                        List<ObjectContainer> left = SelectObjects(tempContainer, pair[0]);
                        if (left.Count > 0)
                        {
                            listObjects.Add(tempContainer);
                        }
                    }
                }
                return listObjects;
            }
        }

        private static List<string> SplitEquals(string str)
        {
            int count = 0;
            List<string> list = new List<string>();
            list.Add("");
            foreach (char c in str)
            {
                if (c == '[')
                {
                    count++;
                    list[list.Count - 1] += c;
                }
                else if (c == ']')
                {
                    count--;
                    list[list.Count - 1] += c;
                }
                else if (c == '=')
                {
                    if (count == 0)
                    {
                        list.Add("");
                    }
                    else
                    {
                        list[list.Count - 1] += c;
                    }
                }
                else
                {
                    list[list.Count - 1] += c;
                }
            }
            return list;
        }

        private static string StripQuotes(string component)
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

        public static object NodeToObject(XmlNode node, Type type)
        {
            return typeof(DirectXmlToObject).GetMethod("ObjectFromXml").MakeGenericMethod(new Type[] { type }).Invoke(null, new object[] { node, false });
        }

        private static object GetDef(string defType, string defName)
        {
            return GetDefType(defType).GetMethod("GetNamed").Invoke(null, new object[] { defName, false });
        }
    }
}