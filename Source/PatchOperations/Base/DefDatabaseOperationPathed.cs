using System;
using System.Xml;

namespace XmlExtensions
{/*
    internal abstract class DefDatabaseOperationPathed : DefDatabaseOperation
    {
        public string defType;
        public string defName;
        public string objPath;

        protected object def;

        protected override void SetException()
        {
            CreateExceptions(defType, "defType", defName, "defName", objPath, "objPath");
        }

        protected override bool PreCheck(XmlDocument xml)
        {
            Type t = GetDefType(defType);
            if (t == null)
            {
                Error("No such defType exists");
                return false;
            }
            def = t.GetMethod("GetNamed").Invoke(null, new object[] { defName, false });
            if (def == null)
            {
                Error("Failed to find a(n) " + defType + " with the given defName");
                return false;
            }
            return true;
        }
    }*/
}