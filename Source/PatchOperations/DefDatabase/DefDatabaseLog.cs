using System;
using System.Collections.Generic;

namespace XmlExtensions
{
    internal class DefDatabaseLog : DefDatabaseOperation
    {
        protected string text;
        protected string warning;
        protected string error;
        protected string objPath;

        protected override bool DoPatch()
        {
            if (text == null && objPath == null && error == null && warning == null)
            {
                Verse.Log.Message("XmlExtensions.DefDatabaseLog");
            }
            if (text != null)
                Verse.Log.Message(text);
            if (warning != null)
                Verse.Log.Warning(warning);
            if (error != null)
                Verse.Log.Error(error);
            List<ObjectContainer> list = SelectObjects(objPath);
            if (list.Count == 0)
            {
                XPathError("objPath");
                return false;
            }
            foreach (ObjectContainer obj in list)
            {
                Verse.Log.Message(obj.value.ToString());
            }
            return true;
        }
    }
}