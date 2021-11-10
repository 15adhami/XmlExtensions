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
            List<ObjectContainer> objects = DefDatabaseSearcher.SelectObjects(objPath);
            if (objects.Count == 0)
            {
                XPathError("objPath");
                return false;
            }
            vals.Add(objects[0].value.ToString());
            return true;
        }
    }
}