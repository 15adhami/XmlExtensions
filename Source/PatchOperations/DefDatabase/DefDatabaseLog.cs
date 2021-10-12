using HarmonyLib;

namespace XmlExtensions
{
    public class DefDatabaseLog : DefDatabaseOperation
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
            Verse.Log.Message((string)Traverse.Create(obj).Method("ToString", new object[] {  }).GetValue());
            return true;
        }
    }
}