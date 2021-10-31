using Verse;

namespace XmlExtensions
{
    internal class DefDatabaseConditional : DefDatabaseOperationPathed
    {
        XmlContainer caseFalse;
        XmlContainer caseTrue;

        protected override bool DoPatch()
        {
            object obj = FindObject(def, objPath);
            return RunPatchesConditional(obj != null, caseTrue, caseFalse, null);
        }
    }
}