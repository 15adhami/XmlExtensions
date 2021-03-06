// Verse.DirectXmlToObject
using HarmonyLib;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal static class CustomXmlLoader
    {
        public static HashSet<string> defaultNamespaces = new();

        private struct FieldAliasCache : IEquatable<FieldAliasCache>
        {
            public Type type;

            public string fieldName;

            public FieldAliasCache(Type type, string fieldName)
            {
                this.type = type;
                this.fieldName = fieldName.ToLower();
            }

            public bool Equals(FieldAliasCache other)
            {
                if (type == other.type)
                {
                    return string.Equals(fieldName, other.fieldName);
                }
                return false;
            }
        }

        public static Stack<Type> currentlyInstantiatingObjectOfType = new Stack<Type>();

        public const string DictionaryKeyName = "key";

        public const string DictionaryValueName = "value";

        public const string LoadDataFromXmlCustomMethodName = "LoadDataFromXmlCustom";

        public const string PostLoadMethodName = "PostLoad";

        public const string ObjectFromXmlMethodName = "ObjectFromXmlReflection";

        public const string ListFromXmlMethodName = "ListFromXmlReflection";

        public const string DictionaryFromXmlMethodName = "DictionaryFromXmlReflection";

        private static Dictionary<Type, Func<XmlNode, XmlNode, string, object>> listFromXmlMethods = new Dictionary<Type, Func<XmlNode, XmlNode, string, object>>();

        private static Dictionary<Type, Func<XmlNode, object>> dictionaryFromXmlMethods = new Dictionary<Type, Func<XmlNode, object>>();

        private static readonly Type[] tmpOneTypeArray = new Type[1];

        private static readonly Dictionary<Type, Func<XmlNode, bool, object>> objectFromXmlMethods = new Dictionary<Type, Func<XmlNode, bool, object>>();

        private static readonly Dictionary<Type, Func<XmlNode, XmlNode, string, bool, object>> objectFromXmlMethodsRecursive = new Dictionary<Type, Func<XmlNode, XmlNode, string, bool, object>>();

        private static Dictionary<FieldAliasCache, FieldInfo> fieldAliases = new Dictionary<FieldAliasCache, FieldInfo>(EqualityComparer<FieldAliasCache>.Default);

        private static Dictionary<Type, Dictionary<string, FieldInfo>> fieldInfoLookup = new Dictionary<Type, Dictionary<string, FieldInfo>>();

        public static Func<XmlNode, bool, object> GetObjectFromXmlMethod(Type type)
        {
            if (!objectFromXmlMethods.TryGetValue(type, out var value))
            {
                MethodInfo method = typeof(CustomXmlLoader).GetMethod("ObjectFromXmlReflection", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                tmpOneTypeArray[0] = type;
                value = (Func<XmlNode, bool, object>)Delegate.CreateDelegate(typeof(Func<XmlNode, bool, object>), method.MakeGenericMethod(tmpOneTypeArray));
                objectFromXmlMethods.Add(type, value);
            }
            return value;
        }

        public static Func<XmlNode, XmlNode, string, bool, object> GetObjectFromXmlMethodRecursive(Type type)
        {
            if (!objectFromXmlMethodsRecursive.TryGetValue(type, out var value))
            {
                MethodInfo method = typeof(CustomXmlLoader).GetMethod("ObjectFromXmlReflectionRecursive", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                tmpOneTypeArray[0] = type;
                value = (Func<XmlNode, XmlNode, string, bool, object>)Delegate.CreateDelegate(typeof(Func<XmlNode, XmlNode, string, bool, object>), method.MakeGenericMethod(tmpOneTypeArray));
                objectFromXmlMethodsRecursive.Add(type, value);
            }
            return value;
        }

        private static object ObjectFromXmlReflection<T>(XmlNode xmlRoot, bool doPostLoad)
        {
            return ObjectFromXml<T>(xmlRoot, doPostLoad);
        }

        private static object ObjectFromXmlReflectionRecursive<T>(XmlNode xmlRoot, XmlNode realRoot, string nameOfDef, bool doPostLoad)
        {
            return ObjectFromXmlRecursive<T>(xmlRoot, realRoot, nameOfDef, doPostLoad);
        }

        public static T ObjectFromXml<T>(XmlNode xmlRoot, bool doPostLoad)
        {
            if (xmlRoot.Attributes["Disabled"]?.Value.ToUpperInvariant() == "TRUE")
            {
                if (typeof(PatchOperation).IsAssignableFrom(typeof(T)))
                {
                    return (T)(object)new Nop();
                }
            }

            XmlAttribute xmlAttribute = xmlRoot.Attributes["IsNull"];
            if (xmlAttribute != null && xmlAttribute.Value.ToUpperInvariant() == "TRUE")
            {
                return default;
            }
            MethodInfo methodInfo = CustomDataLoadMethodOf(typeof(T));
            if (methodInfo != null)
            {
                xmlRoot = XmlInheritance.GetResolvedNodeFor(xmlRoot);
                Type type = ClassTypeOf<T>(xmlRoot);
                currentlyInstantiatingObjectOfType.Push(type);
                T val;
                try
                {
                    val = (T)Activator.CreateInstance(type);
                }
                finally
                {
                    currentlyInstantiatingObjectOfType.Pop();
                }
                try
                {
                    methodInfo.Invoke(val, new object[1] { xmlRoot });
                }
                catch (Exception ex)
                {
                    Verse.Log.Error(string.Concat("Exception in custom XML loader for ", typeof(T), ". Node is:\n ", xmlRoot.OuterXml, "\n\nException is:\n ", ex.ToString()));
                    val = default(T);
                }
                if (doPostLoad)
                {
                    TryDoPostLoad(val);
                }
                return val;
            }
            if (typeof(ISlateRef).IsAssignableFrom(typeof(T)))
            {
                try
                {
                    return ParseHelper.FromString<T>(InnerTextWithReplacedNewlinesOrXML(xmlRoot));
                }
                catch (Exception ex2)
                {
                    Verse.Log.Error(string.Concat("Exception parsing ", xmlRoot.OuterXml, " to type ", typeof(T), ": ", ex2));
                }
                return default(T);
            }
            if (xmlRoot.ChildNodes.Count == 1 && xmlRoot.FirstChild.NodeType == XmlNodeType.CDATA)
            {
                if (typeof(T) != typeof(string))
                {
                    Verse.Log.Error("CDATA can only be used for strings. Bad xml: " + xmlRoot.OuterXml);
                    return default(T);
                }
                return (T)(object)xmlRoot.FirstChild.Value;
            }
            if (xmlRoot.ChildNodes.Count == 1 && xmlRoot.FirstChild.NodeType == XmlNodeType.Text)
            {
                try
                {
                    return ParseHelper.FromString<T>(xmlRoot.InnerText);
                }
                catch (Exception ex3)
                {
                    Verse.Log.Error(string.Concat("Exception parsing ", xmlRoot.OuterXml, " to type ", typeof(T), ": ", ex3));
                }
                return default(T);
            }
            if (Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
            {
                List<T> list = ListFromXml<T>(xmlRoot, xmlRoot, XmlMod.allSettings.advancedDebugging ? Helpers.GetDefNameFromNode(xmlRoot) : null);
                int num = 0;
                foreach (T item in list)
                {
                    int num2 = (int)(object)item;
                    num |= num2;
                }
                return (T)(object)num;
            }
            if (typeof(T).HasGenericDefinition(typeof(List<>)))
            {
                Func<XmlNode, XmlNode, string, object> value = null;
                if (!listFromXmlMethods.TryGetValue(typeof(T), out value))
                {
                    MethodInfo method = typeof(CustomXmlLoader).GetMethod("ListFromXmlReflection", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    Type[] genericArguments = typeof(T).GetGenericArguments();
                    value = (Func<XmlNode, XmlNode, string, object>)Delegate.CreateDelegate(typeof(Func<XmlNode, XmlNode, string, object>), method.MakeGenericMethod(genericArguments));
                    listFromXmlMethods.Add(typeof(T), value);
                }
                return (T)value(xmlRoot, xmlRoot, XmlMod.allSettings.advancedDebugging ? Helpers.GetDefNameFromNode(xmlRoot) : null);
            }
            if (typeof(T).HasGenericDefinition(typeof(Dictionary<,>)))
            {
                Func<XmlNode, object> value2 = null;
                if (!dictionaryFromXmlMethods.TryGetValue(typeof(T), out value2))
                {
                    MethodInfo method2 = typeof(CustomXmlLoader).GetMethod("DictionaryFromXmlReflection", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    Type[] genericArguments2 = typeof(T).GetGenericArguments();
                    value2 = (Func<XmlNode, object>)Delegate.CreateDelegate(typeof(Func<XmlNode, object>), method2.MakeGenericMethod(genericArguments2));
                    dictionaryFromXmlMethods.Add(typeof(T), value2);
                }
                return (T)value2(xmlRoot);
            }
            if (!xmlRoot.HasChildNodes)
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)"";
                }
                XmlAttribute xmlAttribute2 = xmlRoot.Attributes["IsNull"];
                if (xmlAttribute2 != null && xmlAttribute2.Value.ToUpperInvariant() == "TRUE")
                {
                    return default(T);
                }
                if (typeof(T).IsGenericType)
                {
                    Type genericTypeDefinition = typeof(T).GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(List<>) || genericTypeDefinition == typeof(HashSet<>) || genericTypeDefinition == typeof(Dictionary<,>))
                    {
                        return Activator.CreateInstance<T>();
                    }
                }
            }
            xmlRoot = XmlInheritance.GetResolvedNodeFor(xmlRoot);
            Type type2 = ClassTypeOf<T>(xmlRoot);
            Type type3 = Nullable.GetUnderlyingType(type2) ?? type2;
            currentlyInstantiatingObjectOfType.Push(type3);
            T val2;
            try
            {
                val2 = (T)Activator.CreateInstance(type3);
            }
            finally
            {
                currentlyInstantiatingObjectOfType.Pop();
            }
            HashSet<string> hashSet = null;
            if (xmlRoot.ChildNodes.Count > 1)
            {
                hashSet = new HashSet<string>();
            }
            for (int i = 0; i < xmlRoot.ChildNodes.Count; i++)
            {
                XmlNode xmlNode = xmlRoot.ChildNodes[i];
                if (xmlNode is XmlComment)
                {
                    continue;
                }
                if (xmlRoot.ChildNodes.Count > 1)
                {
                    if (hashSet.Contains(xmlNode.Name))
                    {
                        if (XmlMod.allSettings.advancedDebugging && typeof(Def).IsAssignableFrom(typeof(T)))
                        {
                            Verse.Log.Error(string.Concat("XML error: The node <", xmlNode.Name, "> appeared twice within <", xmlRoot.Name, "> ", Helpers.GetNameFromName(Helpers.GetDefNameFromNode(xmlRoot)), "\nWhole XML: ", xmlRoot.OuterXml));
                            ErrorManager.PrintSusMods(xmlRoot);
                        }
                        else
                        {
                            Verse.Log.Error(string.Concat("XML ", typeof(T), " defines the same field twice: ", xmlNode.Name, ".\n\nField contents: ", xmlNode.InnerText, ".\n\nWhole XML:\n\n", xmlRoot.OuterXml));
                        }
                    }
                    else
                    {
                        hashSet.Add(xmlNode.Name);
                    }
                }
                FieldInfo value3 = null;
                DeepProfiler.Start("GetFieldInfoForType");
                try
                {
                    value3 = GetFieldInfoForType(val2.GetType(), xmlNode.Name, xmlRoot);
                }
                finally
                {
                    DeepProfiler.End();
                }
                if (value3 == null)
                {
                    DeepProfiler.Start("Field search");
                    try
                    {
                        FieldAliasCache key = new FieldAliasCache(val2.GetType(), xmlNode.Name);
                        if (!fieldAliases.TryGetValue(key, out value3))
                        {
                            FieldInfo[] fields = val2.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            foreach (FieldInfo fieldInfo in fields)
                            {
                                object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(LoadAliasAttribute), inherit: true);
                                for (int k = 0; k < customAttributes.Length; k++)
                                {
                                    if (((LoadAliasAttribute)customAttributes[k]).alias.EqualsIgnoreCase(xmlNode.Name))
                                    {
                                        value3 = fieldInfo;
                                        break;
                                    }
                                }
                                if (value3 != null)
                                {
                                    break;
                                }
                            }
                            fieldAliases.Add(key, value3);
                        }
                    }
                    finally
                    {
                        DeepProfiler.End();
                    }
                }
                if (value3 != null && value3.TryGetAttribute<UnsavedAttribute>() != null && !value3.TryGetAttribute<UnsavedAttribute>().allowLoading)
                {
                    Verse.Log.Error("XML error: " + xmlNode.OuterXml + " corresponds to a field in type " + val2.GetType().Name + " which has an Unsaved attribute. Context: " + xmlRoot.OuterXml);
                    if (XmlMod.allSettings.advancedDebugging && typeof(Def).IsAssignableFrom(typeof(T)))
                    {
                        ErrorManager.PrintSusMods(xmlRoot);
                    }
                }
                else if (value3 == null)
                {
                    DeepProfiler.Start("Field search 2");
                    try
                    {
                        bool flag = false;
                        XmlAttribute xmlAttribute3 = xmlNode.Attributes?["IgnoreIfNoMatchingField"];
                        if (xmlAttribute3 != null && xmlAttribute3.Value.ToUpperInvariant() == "TRUE")
                        {
                            flag = true;
                        }
                        else
                        {
                            object[] customAttributes = val2.GetType().GetCustomAttributes(typeof(IgnoreSavedElementAttribute), inherit: true);
                            for (int j = 0; j < customAttributes.Length; j++)
                            {
                                if (string.Equals(((IgnoreSavedElementAttribute)customAttributes[j]).elementToIgnore, xmlNode.Name, StringComparison.OrdinalIgnoreCase))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        if (!flag)
                        {
                            if (XmlMod.allSettings.advancedDebugging && typeof(Def).IsAssignableFrom(typeof(T)))
                            {
                                Verse.Log.Error("XML error: " + xmlNode.OuterXml + " does not belong in <" + xmlRoot.Name + "> " + Helpers.GetNameFromName(Helpers.GetDefNameFromNode(xmlRoot)) + "\nWhole XML: " + xmlRoot.OuterXml);
                                ErrorManager.PrintSusMods(xmlRoot);
                            }
                            else
                            {
                                Verse.Log.Error("XML error: " + xmlNode.OuterXml + " doesn't correspond to any field in type " + val2.GetType().Name + ". Context: " + xmlRoot.OuterXml);
                            }
                        }
                    }
                    finally
                    {
                        DeepProfiler.End();
                    }
                }
                else if (typeof(Def).IsAssignableFrom(value3.FieldType))
                {
                    if (xmlNode.InnerText.NullOrEmpty())
                    {
                        value3.SetValue(val2, null);
                        continue;
                    }
                    XmlAttribute xmlAttribute4 = xmlNode.Attributes["MayRequire"];
                    DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(val2, value3, xmlNode.InnerText, xmlAttribute4?.Value.ToLower());
                }
                else
                {
                    object obj = null;
                    try
                    {
                        if (XmlMod.allSettings.advancedDebugging && typeof(Def).IsAssignableFrom(typeof(T)))
                        {
                            obj = GetObjectFromXmlMethodRecursive(value3.FieldType)(xmlNode, xmlRoot, Helpers.GetDefNameFromNode(xmlRoot), doPostLoad);
                        }
                        else
                        {
                            obj = GetObjectFromXmlMethod(value3.FieldType)(xmlNode, doPostLoad);
                        }
                    }
                    catch (Exception ex4)
                    {
                        Verse.Log.Error("Exception loading the node:\n" + xmlRoot.ToString() + "\n\nException info: " + ex4.ToString());
                        if (XmlMod.allSettings.advancedDebugging && typeof(Def).IsAssignableFrom(typeof(T)))
                        {
                            ErrorManager.PrintSusMods(xmlRoot);
                        }
                        continue;
                    }
                    if (!typeof(T).IsValueType)
                    {
                        value3.SetValue(val2, obj);
                        continue;
                    }
                    object obj2 = val2;
                    value3.SetValue(obj2, obj);
                    val2 = (T)obj2;
                }
            }
            if (doPostLoad)
            {
                TryDoPostLoad(val2);
            }
            return val2;
        }

        public static T ObjectFromXmlRecursive<T>(XmlNode xmlRoot, XmlNode fullRoot, string nameOfDef, bool doPostLoad)
        {
            if (xmlRoot.Attributes["Disabled"]?.Value.ToUpperInvariant() == "TRUE")
            {
                if (typeof(PatchOperation).IsAssignableFrom(typeof(T)))
                {
                    return (T)(object)new Nop();
                }
            }

            XmlAttribute xmlAttribute = xmlRoot.Attributes["IsNull"];
            if (xmlAttribute != null && xmlAttribute.Value.ToUpperInvariant() == "TRUE")
            {
                return default;
            }
            MethodInfo methodInfo = CustomDataLoadMethodOf(typeof(T));
            if (methodInfo != null)
            {
                xmlRoot = XmlInheritance.GetResolvedNodeFor(xmlRoot);
                Type type = ClassTypeOf<T>(xmlRoot);
                currentlyInstantiatingObjectOfType.Push(type);
                T val;
                try
                {
                    val = (T)Activator.CreateInstance(type);
                }
                finally
                {
                    currentlyInstantiatingObjectOfType.Pop();
                }
                try
                {
                    methodInfo.Invoke(val, new object[1] { xmlRoot });
                }
                catch (Exception ex)
                {
                    Verse.Log.Error(string.Concat("Exception in custom XML loader for ", typeof(T), ". Node is:\n ", xmlRoot.OuterXml, "\n\nException is:\n ", ex.ToString()));
                    val = default(T);
                }
                if (doPostLoad)
                {
                    TryDoPostLoad(val);
                }
                return val;
            }
            if (typeof(ISlateRef).IsAssignableFrom(typeof(T)))
            {
                try
                {
                    return ParseHelper.FromString<T>(InnerTextWithReplacedNewlinesOrXML(xmlRoot));
                }
                catch (Exception ex2)
                {
                    Verse.Log.Error(string.Concat("Exception parsing ", xmlRoot.OuterXml, " to type ", typeof(T), ": ", ex2));
                }
                return default(T);
            }
            if (xmlRoot.ChildNodes.Count == 1 && xmlRoot.FirstChild.NodeType == XmlNodeType.CDATA)
            {
                if (typeof(T) != typeof(string))
                {
                    Verse.Log.Error("CDATA can only be used for strings. Bad xml: " + xmlRoot.OuterXml);
                    return default(T);
                }
                return (T)(object)xmlRoot.FirstChild.Value;
            }
            if (xmlRoot.ChildNodes.Count == 1 && xmlRoot.FirstChild.NodeType == XmlNodeType.Text)
            {
                try
                {
                    return ParseHelper.FromString<T>(xmlRoot.InnerText);
                }
                catch (Exception ex3)
                {
                    Verse.Log.Error(string.Concat("Exception parsing ", xmlRoot.OuterXml, " to type ", typeof(T), ": ", ex3));
                }
                return default(T);
            }
            if (Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
            {
                List<T> list = ListFromXml<T>(xmlRoot, fullRoot, nameOfDef);
                int num = 0;
                foreach (T item in list)
                {
                    int num2 = (int)(object)item;
                    num |= num2;
                }
                return (T)(object)num;
            }
            if (typeof(T).HasGenericDefinition(typeof(List<>)))
            {
                Func<XmlNode, XmlNode, string, object> value = null;
                if (!listFromXmlMethods.TryGetValue(typeof(T), out value))
                {
                    MethodInfo method = typeof(CustomXmlLoader).GetMethod("ListFromXmlReflection", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    Type[] genericArguments = typeof(T).GetGenericArguments();
                    value = (Func<XmlNode, XmlNode, string, object>)Delegate.CreateDelegate(typeof(Func<XmlNode, XmlNode, string, object>), method.MakeGenericMethod(genericArguments));
                    listFromXmlMethods.Add(typeof(T), value);
                }
                return (T)value(xmlRoot, fullRoot, nameOfDef);
            }
            if (typeof(T).HasGenericDefinition(typeof(Dictionary<,>)))
            {
                Func<XmlNode, object> value2 = null;
                if (!dictionaryFromXmlMethods.TryGetValue(typeof(T), out value2))
                {
                    MethodInfo method2 = typeof(CustomXmlLoader).GetMethod("DictionaryFromXmlReflection", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    Type[] genericArguments2 = typeof(T).GetGenericArguments();
                    value2 = (Func<XmlNode, object>)Delegate.CreateDelegate(typeof(Func<XmlNode, object>), method2.MakeGenericMethod(genericArguments2));
                    dictionaryFromXmlMethods.Add(typeof(T), value2);
                }
                return (T)value2(xmlRoot);
            }
            if (!xmlRoot.HasChildNodes)
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)"";
                }
                XmlAttribute xmlAttribute2 = xmlRoot.Attributes["IsNull"];
                if (xmlAttribute2 != null && xmlAttribute2.Value.ToUpperInvariant() == "TRUE")
                {
                    return default(T);
                }
                if (typeof(T).IsGenericType)
                {
                    Type genericTypeDefinition = typeof(T).GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(List<>) || genericTypeDefinition == typeof(HashSet<>) || genericTypeDefinition == typeof(Dictionary<,>))
                    {
                        return Activator.CreateInstance<T>();
                    }
                }
            }
            xmlRoot = XmlInheritance.GetResolvedNodeFor(xmlRoot);
            Type type2 = ClassTypeOf<T>(xmlRoot);
            Type type3 = Nullable.GetUnderlyingType(type2) ?? type2;
            currentlyInstantiatingObjectOfType.Push(type3);
            T val2;
            try
            {
                val2 = (T)Activator.CreateInstance(type3);
            }
            finally
            {
                currentlyInstantiatingObjectOfType.Pop();
            }
            HashSet<string> hashSet = null;
            if (xmlRoot.ChildNodes.Count > 1)
            {
                hashSet = new HashSet<string>();
            }
            for (int i = 0; i < xmlRoot.ChildNodes.Count; i++)
            {
                XmlNode xmlNode = xmlRoot.ChildNodes[i];
                if (xmlNode is XmlComment)
                {
                    continue;
                }
                if (xmlRoot.ChildNodes.Count > 1)
                {
                    if (hashSet.Contains(xmlNode.Name))
                    {
                        if (nameOfDef != null)
                        {
                            Verse.Log.Error(string.Concat("XML error: The node <", xmlNode.Name, "> appeared twice within <", xmlRoot.Name, "> " + Helpers.GetNameFromName(nameOfDef) + "\nWhole XML: ", fullRoot.OuterXml));
                            ErrorManager.PrintSusMods(fullRoot);
                        }
                        else
                        {
                            Verse.Log.Error(string.Concat("XML error: The node <", xmlNode.Name, "> appeared twice within <", xmlRoot.Name, ">.\nWhole XML: ", fullRoot.OuterXml));
                        }
                    }
                    else
                    {
                        hashSet.Add(xmlNode.Name);
                    }
                }
                FieldInfo value3 = null;
                DeepProfiler.Start("GetFieldInfoForType");
                try
                {
                    value3 = GetFieldInfoForType(val2.GetType(), xmlNode.Name, xmlRoot);
                }
                finally
                {
                    DeepProfiler.End();
                }
                if (value3 == null)
                {
                    DeepProfiler.Start("Field search");
                    try
                    {
                        FieldAliasCache key = new FieldAliasCache(val2.GetType(), xmlNode.Name);
                        if (!fieldAliases.TryGetValue(key, out value3))
                        {
                            FieldInfo[] fields = val2.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            foreach (FieldInfo fieldInfo in fields)
                            {
                                object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(LoadAliasAttribute), inherit: true);
                                for (int k = 0; k < customAttributes.Length; k++)
                                {
                                    if (((LoadAliasAttribute)customAttributes[k]).alias.EqualsIgnoreCase(xmlNode.Name))
                                    {
                                        value3 = fieldInfo;
                                        break;
                                    }
                                }
                                if (value3 != null)
                                {
                                    break;
                                }
                            }
                            fieldAliases.Add(key, value3);
                        }
                    }
                    finally
                    {
                        DeepProfiler.End();
                    }
                }
                if (value3 != null && value3.TryGetAttribute<UnsavedAttribute>() != null && !value3.TryGetAttribute<UnsavedAttribute>().allowLoading)
                {
                    Verse.Log.Error("XML error: " + xmlNode.OuterXml + " corresponds to a field in type " + val2.GetType().Name + " which has an Unsaved attribute. Context: " + xmlRoot.OuterXml);
                    if (XmlMod.allSettings.advancedDebugging && nameOfDef != null)
                    {
                        ErrorManager.PrintSusMods(fullRoot);
                    }
                }
                else if (value3 == null)
                {
                    DeepProfiler.Start("Field search 2");
                    try
                    {
                        bool flag = false;
                        XmlAttribute xmlAttribute3 = xmlNode.Attributes?["IgnoreIfNoMatchingField"];
                        if (xmlAttribute3 != null && xmlAttribute3.Value.ToUpperInvariant() == "TRUE")
                        {
                            flag = true;
                        }
                        else
                        {
                            object[] customAttributes = val2.GetType().GetCustomAttributes(typeof(IgnoreSavedElementAttribute), inherit: true);
                            for (int j = 0; j < customAttributes.Length; j++)
                            {
                                if (string.Equals(((IgnoreSavedElementAttribute)customAttributes[j]).elementToIgnore, xmlNode.Name, StringComparison.OrdinalIgnoreCase))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        if (!flag)
                        {
                            if (nameOfDef != null)
                            {
                                Verse.Log.Error("XML error: <" + xmlNode.Name + "> does not belong in <" + xmlRoot.Name + "> (Type=" + val2.GetType().ToString() + ") " + Helpers.GetNameFromName(nameOfDef) + "\nXML Block: " + xmlRoot.OuterXml + "\nWhole XML: " + fullRoot.OuterXml);
                                ErrorManager.PrintSusMods(fullRoot);
                            }
                            else
                            {
                                Verse.Log.Error("XML error: <" + xmlNode.Name + "> does not belong in <" + xmlRoot.Name + "> (Type=" + val2.GetType().ToString() + ").\nXML Block: " + xmlRoot.OuterXml + "\nWhole XML: " + fullRoot.OuterXml);
                            }
                        }
                    }
                    finally
                    {
                        DeepProfiler.End();
                    }
                }
                else if (typeof(Def).IsAssignableFrom(value3.FieldType))
                {
                    if (xmlNode.InnerText.NullOrEmpty())
                    {
                        value3.SetValue(val2, null);
                        continue;
                    }
                    XmlAttribute xmlAttribute4 = xmlNode.Attributes["MayRequire"];
                    DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(val2, value3, xmlNode.InnerText, xmlAttribute4?.Value.ToLower());
                }
                else
                {
                    object obj = null;
                    try
                    {
                        obj = GetObjectFromXmlMethodRecursive(value3.FieldType)(xmlNode, fullRoot, nameOfDef, doPostLoad);
                    }
                    catch (Exception ex4)
                    {
                        Verse.Log.Error("Exception loading the node:\n" + xmlRoot.ToString() + "\n\nException info: " + ex4.ToString());
                        if (XmlMod.allSettings.advancedDebugging && nameOfDef != null)
                        {
                            ErrorManager.PrintSusMods(fullRoot);
                        }
                        continue;
                    }
                    if (!typeof(T).IsValueType)
                    {
                        value3.SetValue(val2, obj);
                        continue;
                    }
                    object obj2 = val2;
                    value3.SetValue(obj2, obj);
                    val2 = (T)obj2;
                }
            }
            if (doPostLoad)
            {
                TryDoPostLoad(val2);
            }
            return val2;
        }

        private static Type ClassTypeOf<T>(XmlNode xmlRoot)
        {
            XmlAttribute xmlAttribute = xmlRoot.Attributes["Class"];
            if (xmlAttribute != null)
            {
                Type typeInAnyAssembly = null;
                foreach (string t in defaultNamespaces)
                {
                    typeInAnyAssembly = GenTypes.GetTypeInAnyAssembly(t + "." + xmlAttribute.Value, t);
                    if (typeInAnyAssembly != null)
                    {
                        break;
                    }
                }
                if (typeInAnyAssembly == null)
                {
                    typeInAnyAssembly = GenTypes.GetTypeInAnyAssembly(xmlAttribute.Value, typeof(T).Namespace);
                    if (typeInAnyAssembly == null)
                    {
                        Verse.Log.Error("Could not find type named " + xmlAttribute.Value + " from node " + xmlRoot.OuterXml);
                        if (XmlMod.allSettings.advancedDebugging && typeof(Def).IsAssignableFrom(typeof(T)))
                        {
                            ErrorManager.PrintSusMods(Helpers.GetDefNode(xmlRoot));
                        }
                        return typeof(T);
                    }
                }
                return typeInAnyAssembly;
            }
            return typeof(T);
        }

        private static void TryDoPostLoad(object obj)
        {
            DeepProfiler.Start("TryDoPostLoad");
            try
            {
                MethodInfo method = obj.GetType().GetMethod("PostLoad");
                if (method != null)
                {
                    method.Invoke(obj, null);
                }
            }
            catch (Exception ex)
            {
                Verse.Log.Error("Exception while executing PostLoad on " + obj.ToStringSafe() + ": " + ex);
            }
            finally
            {
                DeepProfiler.End();
            }
        }

        private static object ListFromXmlReflection<T>(XmlNode listRootNode, XmlNode fullRoot, string nameOfDef)
        {
            return ListFromXml<T>(listRootNode, fullRoot, nameOfDef);
        }

        private static List<T> ListFromXml<T>(XmlNode listRootNode, XmlNode fullRoot, string nameOfDef)
        {
            List<T> list = new List<T>();
            try
            {
                bool flag = typeof(Def).IsAssignableFrom(typeof(T));
                foreach (XmlNode childNode in listRootNode.ChildNodes)
                {
                    if (!ValidateListNode(childNode, listRootNode, typeof(T)))
                    {
                        continue;
                    }
                    XmlAttribute xmlAttribute = childNode.Attributes["MayRequire"];
                    if (flag)
                    {
                        DirectXmlCrossRefLoader.RegisterListWantsCrossRef(list, childNode.InnerText, listRootNode.Name, xmlAttribute?.Value);
                    }
                    else if (xmlAttribute == null || xmlAttribute.Value.NullOrEmpty() || ModsConfig.AreAllActive(xmlAttribute.Value))
                    {
                        try
                        {
                            if (XmlMod.allSettings.advancedDebugging)
                            {
                                list.Add(ObjectFromXmlRecursive<T>(childNode, fullRoot, nameOfDef, true));
                            }
                            else
                            {
                                list.Add(ObjectFromXml<T>(childNode, true));
                            }
                        }
                        catch (Exception ex)
                        {
                            Verse.Log.Error(string.Concat("Exception loading list element from XML: ", ex, "\nXML:\n", listRootNode.OuterXml));
                            if (XmlMod.allSettings.advancedDebugging && nameOfDef != null)
                            {
                                ErrorManager.PrintSusMods(fullRoot);
                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception ex2)
            {
                Verse.Log.Error(string.Concat("Exception loading list from XML: ", ex2, "\nXML:\n", listRootNode.OuterXml));
                if (XmlMod.allSettings.advancedDebugging && nameOfDef != null)
                {
                    ErrorManager.PrintSusMods(fullRoot);
                }
                return list;
            }
        }

        private static object DictionaryFromXmlReflection<K, V>(XmlNode dictRootNode)
        {
            return DictionaryFromXml<K, V>(dictRootNode);
        }

        private static Dictionary<K, V> DictionaryFromXml<K, V>(XmlNode dictRootNode)
        {
            Dictionary<K, V> dictionary = new Dictionary<K, V>();
            try
            {
                bool num = typeof(Def).IsAssignableFrom(typeof(K));
                bool flag = typeof(Def).IsAssignableFrom(typeof(V));
                if (!num && !flag)
                {
                    foreach (XmlNode childNode in dictRootNode.ChildNodes)
                    {
                        if (ValidateListNode(childNode, dictRootNode, typeof(KeyValuePair<K, V>)))
                        {
                            K key = ObjectFromXml<K>(childNode["key"], doPostLoad: true);
                            V value = ObjectFromXml<V>(childNode["value"], doPostLoad: true);
                            dictionary.Add(key, value);
                        }
                    }
                    return dictionary;
                }
                foreach (XmlNode childNode2 in dictRootNode.ChildNodes)
                {
                    if (ValidateListNode(childNode2, dictRootNode, typeof(KeyValuePair<K, V>)))
                    {
                        DirectXmlCrossRefLoader.RegisterDictionaryWantsCrossRef(dictionary, childNode2, dictRootNode.Name);
                    }
                }
                return dictionary;
            }
            catch (Exception ex)
            {
                Verse.Log.Error("Malformed dictionary XML. Node: " + dictRootNode.OuterXml + ".\n\nException: " + ex);
                return dictionary;
            }
        }

        private static MethodInfo CustomDataLoadMethodOf(Type type)
        {
            return type.GetMethod("LoadDataFromXmlCustom", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private static bool ValidateListNode(XmlNode listEntryNode, XmlNode listRootNode, Type listItemType)
        {
            if (listEntryNode is XmlComment)
            {
                return false;
            }
            if (listEntryNode is XmlText)
            {
                Verse.Log.Error("XML format error: Raw text found inside a list element. Did you mean to surround it with list item <li> tags? " + listRootNode.OuterXml);
                return false;
            }
            if (listEntryNode.Name != "li" && CustomDataLoadMethodOf(listItemType) == null)
            {
                Verse.Log.Error("XML format error: List item found with name that is not <li>, and which does not have a custom XML loader method, in " + listRootNode.OuterXml);
                return false;
            }
            return true;
        }

        private static FieldInfo GetFieldInfoForType(Type type, string token, XmlNode debugXmlNode)
        {
            Dictionary<string, FieldInfo> dictionary = fieldInfoLookup.TryGetValue(type);
            if (dictionary == null)
            {
                dictionary = new Dictionary<string, FieldInfo>();
                fieldInfoLookup[type] = dictionary;
            }
            FieldInfo fieldInfo = dictionary.TryGetValue(token);
            if (fieldInfo == null && !dictionary.ContainsKey(token))
            {
                fieldInfo = SearchTypeHierarchy(type, token, BindingFlags.Default);
                if (fieldInfo == null)
                {
                    fieldInfo = SearchTypeHierarchy(type, token, BindingFlags.IgnoreCase);
                    if (fieldInfo != null && !type.HasAttribute<CaseInsensitiveXMLParsing>())
                    {
                        string text = $"Attempt to use string {token} to refer to field {fieldInfo.Name} in type {type}; xml tags are now case-sensitive";
                        if (debugXmlNode != null)
                        {
                            text = text + ". XML: " + debugXmlNode.OuterXml;
                        }
                        Verse.Log.Error(text);
                    }
                }
                dictionary[token] = fieldInfo;
            }
            return fieldInfo;
        }

        private static FieldInfo SearchTypeHierarchy(Type type, string token, BindingFlags extraFlags)
        {
            FieldInfo fieldInfo = null;
            while (true)
            {
                fieldInfo = type.GetField(token, extraFlags | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (!(fieldInfo == null) || !(type.BaseType != typeof(object)))
                {
                    break;
                }
                type = type.BaseType;
            }
            return fieldInfo;
        }

        public static string InnerTextWithReplacedNewlinesOrXML(XmlNode xmlNode)
        {
            if (xmlNode.ChildNodes.Count == 1 && xmlNode.FirstChild.NodeType == XmlNodeType.Text)
            {
                return xmlNode.InnerText.Replace("\\n", "\n");
            }
            return xmlNode.InnerXml;
        }

        public static MethodInfo GetObjectFromXmlReflection()
        {
            return AccessTools.Method(typeof(CustomXmlLoader), "ObjectFromXmlReflection");
        }

        public static XmlDocument CombineIntoUnifiedXMLMirror(List<LoadableXmlAsset> xmls, Dictionary<XmlNode, LoadableXmlAsset> assetlookup)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.AppendChild(xmlDocument.CreateElement("Defs"));
            foreach (LoadableXmlAsset xml in xmls)
            {
                if (xml.xmlDoc == null || xml.xmlDoc.DocumentElement == null)
                {
                    Verse.Log.Error(string.Format("{0}: unknown parse failure", xml.fullFolderPath + "/" + xml.name));
                    continue;
                }
                if (xml.xmlDoc.DocumentElement.Name != "Defs")
                {
                    Verse.Log.Error(string.Format("{0}: root element named {1}; should be named Defs", xml.fullFolderPath + "/" + xml.name, xml.xmlDoc.DocumentElement.Name));
                }
                foreach (XmlNode childNode in xml.xmlDoc.DocumentElement.ChildNodes)
                {
                    XmlNode xmlNode = xmlDocument.ImportNode(childNode, deep: true);
                    assetlookup[xmlNode] = xml;
                    xmlDocument.DocumentElement.AppendChild(xmlNode);
                    if (XmlMod.allSettings.advancedDebugging)
                    {
                        string name = Helpers.GetDefNameFromNode(xmlNode);
                        if (name != null && xml.mod != null)
                        {
                            PatchManager.ModPatchedDef(name, xml.mod, null);
                        }
                    }
                }
            }
            return xmlDocument;
        }
    }
}