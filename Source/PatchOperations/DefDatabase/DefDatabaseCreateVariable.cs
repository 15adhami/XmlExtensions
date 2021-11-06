using System.Collections.Generic;
using System.Xml;

namespace XmlExtensions
{
    internal class DefDatabaseCreateVariable : DefDatabaseOperation
    {
        public string objPath;

        public DefDatabaseCreateVariable()
        {
            isValue = true;
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            List<ObjectContainer> objects = SelectObjects(objPath);
            if (objects.Count == 0)
            {
                Error("Failed to find an object with the given path");
                return false;
            }
            vals.Add(objects[0].value.ToString());
            return true;
        }
    }
}