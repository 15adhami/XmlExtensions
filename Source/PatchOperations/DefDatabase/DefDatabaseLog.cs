using System;
using System.Collections.Generic;

namespace XmlExtensions
{
    internal class DefDatabaseLog : DefDatabaseOperation
    {
        string objPath;
        protected override bool DoPatch()
        {
            List<object> list = SelectObjects(objPath);
            if (list.Count == 0)
            {
                Error("Failed to find an object with the given objPath2");
                return false;
            }
            foreach (object obj in list)
            {
                Verse.Log.Message(obj.ToString());
            }
            return true;
        }
    }
}