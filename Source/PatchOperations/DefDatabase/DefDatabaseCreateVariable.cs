using System.Collections.Generic;
using System.Xml;

namespace XmlExtensions
{
    internal class DefDatabaseCreateVariable : DefDatabaseOperationPathed
    {
        public DefDatabaseCreateVariable()
        {
            isValue = true;
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
            vals.Add(obj.ToString());
            return true;
        }
    }
}