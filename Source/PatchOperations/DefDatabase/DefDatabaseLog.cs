using HarmonyLib;

namespace XmlExtensions
{
    public class DefDatabaseLog : DefDatabaseOperation
    {
        public string defType;
        public string defName;
        public string objPath;

        protected override bool DoPatch()
        {
            object def = GetDef(defType, defName);
            object obj = FindObject(def, objPath);
            if (obj == null)
            {
                Error("Failed to find an object with the given path");
                return false;
            }
            Verse.Log.Message((string)Traverse.Create(obj).Method("ToString", new object[] {  }).GetValue());
            return true;
        }
    }
}