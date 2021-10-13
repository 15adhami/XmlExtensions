using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public abstract class DefDatabaseOperation : PatchOperationValue
    {
        public object parentObj;
        protected int listIndex = -1;

        protected bool isValue = false;

        public DefDatabaseOperation()
        {
            isDefDatabaseOperation = true;
        }

        protected override bool PreCheck(XmlDocument xml)
        {
            return true;
        }

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
                if (!isValue)
                {
                    try
                    {
                        if (!PreCheck(xml))
                        {
                            return false;
                        }
                        return DoPatch();
                    }
                    catch (Exception e)
                    {
                        Error(e.Message);
                        return false;
                    }
                }
                else
                {
                    return base.Patch(xml);
                }
            }
        }

        protected List<string> CreateComponents(string path)
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

        protected string RemoveSpaces(string str)
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

        private string RemoveBrackets(string str)
        {
            string temp = str.Substring(1);
            temp = temp.Substring(0, temp.Length - 1);
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
            ICollection col = list as ICollection;
            int count = col.Count;
            string classTemp = StripQuotes(component);
            PropertyInfo indexer = AccessTools.Property(list.GetType(), "Item");
            string noBrackets = RemoveBrackets(component);
            int index = 0;
            if (int.TryParse(noBrackets, out index))
            {
                return indexer.GetValue(list, new object[] { index - 1 });
            }
            if (indexer == null)
            {
                return null;
            }
            if (noBrackets.StartsWith("Class"))
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
                        listIndex = i;
                        return tempItem;
                    }
                }
            }
            else
            {
                List<string> pair = SplitEquals(noBrackets);

                for (int i = 0; i < count; i++)
                {
                    object tempItem = indexer.GetValue(list, new object[] { i });
                    if (pair.Count > 1)
                    {
                        object left = FindObject(tempItem, pair[0]);
                        if (left != null)
                        {
                            object right = AccessTools.Method(typeof(ParseHelper), "FromString", new Type[] { typeof(string) }).MakeGenericMethod(new Type[] { left.GetType() }).Invoke(null, new object[] { StripQuotes(pair[1]) });
                            if (left.Equals(right))
                            {
                                return tempItem;
                            }
                        }
                    }
                    else
                    {
                        object obj = FindObject(tempItem, pair[0]);
                        if (obj != null)
                        {
                            return tempItem;
                        }
                    }
                }
            }
            return null;
        }

        private List<string> SplitEquals(string str)
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

        private string StripQuotes(string component)
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

        protected object FindObject(object parent, string path)
        {
            try
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
            catch
            {
                return null;
            }
        }

        protected object NodeToObject(XmlNode node, Type type)
        {
            return typeof(DirectXmlToObject).GetMethod("ObjectFromXml").MakeGenericMethod(new Type[] { type }).Invoke(null, new object[] { node, false });
        }

        protected virtual bool DoPatch()
        {
            return true;
        }
    }
}