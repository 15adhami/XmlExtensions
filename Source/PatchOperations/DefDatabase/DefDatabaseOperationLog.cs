using HarmonyLib;

namespace XmlExtensions
{
    public class DefDatabaseOperationLog : DefDatabaseOperation
    {
        public string defType;
        public string defName;
        public string path;

        protected override bool DoPatch()
        {
            object def = GetDef(defType, defName);
            object obj = FindObject(def, path);
            if (obj == null)
            {
                Error("Failed to find an object with the given path");
                return false;
            }
            return true;
        }
    }
}