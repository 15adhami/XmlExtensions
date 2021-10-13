using HarmonyLib;
using System.Collections.Generic;
using Verse;

namespace XmlExtensions
{
    public class DefDatabaseOperationRemove : DefDatabaseOperationPathed
    {
        protected override bool DoPatch()
        {
            object obj = FindObject(def, objPath);
            if (obj == null)
            {
                Error("Failed to find an object with the given path");
                return false;
            }
            if (parentObj.GetType().HasGenericDefinition(typeof(List<>)))
            {
                AccessTools.Method(parentObj.GetType(), "Remove").Invoke(parentObj, new object[] { obj });
            }
            else
            {
                Error("You can only Remove from lists");
                return false;
            }
            return true;
        }
    }
}