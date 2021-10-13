using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class DefDatabaseCreateVariable : DefDatabaseOperation
    {
        public string defType;
        public string defName;
        public string objPath;

        public DefDatabaseCreateVariable()
        {
            isValue = true;
        }

        protected override void SetException()
        {
            CreateExceptions(defType, "defType", defName, "defName", objPath, "path");
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            object def = GetDef(defType, defName);
            if (def == null)
            {
                Error("Failed to find the given Def");
                return false;
            }
            object obj = FindObject(def, RemoveSpaces(objPath));
            if (obj == null)
            {
                Error("Failed to find an object with the given path");
                return false;
            }
            string str = (string)Traverse.Create(obj).Method("ToString", new object[] { }).GetValue();
            vals.Add(str);
            return true;
        }
    }
}