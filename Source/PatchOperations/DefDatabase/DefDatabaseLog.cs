using System;
using System.Collections.Generic;

namespace XmlExtensions
{
    internal class DefDatabaseLog : DefDatabaseOperation
    {
        string objPath;
        protected override bool DoPatch()
        {
            List<ObjectContainer> list = SelectObjects(objPath);
            if (list.Count == 0)
            {
                Error("Failed to find an object with the given objPath2");
                return false;
            }
            foreach (ObjectContainer obj in list)
            {
                Verse.Log.Message(obj.child.ToString());
            }
            return true;
        }
    }
}