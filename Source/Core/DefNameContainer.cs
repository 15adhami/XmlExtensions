using System;
using System.Collections.Generic;

namespace XmlExtensions
{
    internal class DefNameContainer : IComparable
    {
        public string Name;
        public List<Type> OperationTypes;

        public DefNameContainer(string name, Type type)
        {
            Name = name;
            OperationTypes ??= new();
            if (type != null && !OperationTypes.Contains(type))
            {
                OperationTypes.Add(type);
            }            
        }

        public int CompareTo(object obj)
        {
            string name1 = Name.Split(';')[1];
            // name1 = name1.StartsWith("@") ? Name.Substring(1) : Name;
            string name2 = ((DefNameContainer)obj).Name.Split(';')[1];
            // name2 = name2.StartsWith("@") ? Name.Substring(1) : Name;
            return name1.CompareTo(name2);
        }
    }
}