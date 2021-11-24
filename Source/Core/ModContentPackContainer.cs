using System;
using System.Collections.Generic;
using Verse;

namespace XmlExtensions
{
    internal class ModContentPackContainer : IComparable
    {
        public ModContentPack Pack;
        public List<Type> OperationTypes;

        public ModContentPackContainer(ModContentPack pack, Type type)
        {
            Pack = pack;
            OperationTypes ??= new();
            if (!OperationTypes.Contains(type))
            {
                OperationTypes.Add(type);
            }
        }

        public int CompareTo(object obj)
        {
            return Pack.Name.CompareTo(((ModContentPackContainer)obj).Pack.Name);
        }
    }
}