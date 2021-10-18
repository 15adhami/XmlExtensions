using HarmonyLib;

namespace XmlExtensions
{
    public class DefDatabaseLog : DefDatabaseOperationPathed
    {
        protected override bool DoPatch()
        {
            object obj = FindObject(def, objPath);
            if (obj == null)
            {
                Error("Failed to find an object with the given objPath");
                return false;
            }
            Verse.Log.Message(obj.ToString());
            return true;
        }
    }
}