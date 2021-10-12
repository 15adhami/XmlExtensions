using HarmonyLib;
using Verse;

namespace XmlExtensions
{
    public class DefDatabaseCreateVariable : DefDatabaseOperation
    {
        public string defType;
        public string defName;
        public string path;
        public string storeIn;
        public XmlContainer apply;
        public string brackets = "{}";

        protected override bool DoPatch()
        {
            object def = GetDef(defType, defName);
            object obj = FindObject(def, path);
            if (obj == null)
            {
                Error("Failed to find an object with the given path");
                return false;
            }
            string str = (string)Traverse.Create(obj).Method("ToString", new object[] { }).GetValue();
            return RunPatches(Helpers.SubstituteVariableXmlContainer(apply, storeIn, str, brackets), null);
        }
    }
}