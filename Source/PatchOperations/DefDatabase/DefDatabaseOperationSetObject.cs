using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class DefDatabaseOperationSetObject : DefDatabaseOperationPathed
    {
        public string defType2;
        public string defName2;
        public string objPath2;

        protected override bool DoPatch()
        {
            List<string> list = CreateComponents(objPath);
            AccessTools.Field(parentObj.GetType(), list[list.Count - 1]).SetValue(parentObj, FindObject(GetDef(defType2, defName2), objPath2));
            return true;
        }
    }
}