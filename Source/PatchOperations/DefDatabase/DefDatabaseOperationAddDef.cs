using HarmonyLib;
using System;
using System.Reflection;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class DefDatabaseOperationAddDef : DefDatabaseOperation
    {
        public XmlContainer value;

        protected override bool DoPatch()
        {
            foreach (XmlNode node in value.node.ChildNodes)
            {
                Type type = GetDefType(node.Name);
                Type tempType = GenTypes.GetTypeInAnyAssembly(node.Name, "Verse");
                if (tempType == null)
                {
                    tempType = GenTypes.GetTypeInAnyAssembly(node.Name, "RimWorld");
                    if (tempType == null)
                    {
                        return false;
                    }
                }
                MethodInfo genericMethod = typeof(DirectXmlToObject).GetMethod("ObjectFromXml").MakeGenericMethod(tempType);
                type.GetMethod("Add", new Type[] { tempType }).Invoke(null, new object[] { genericMethod.Invoke(null, new object[] { node, false }) });
            }
            return true;
        }
    }
}