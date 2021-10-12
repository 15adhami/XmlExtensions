using Verse;

namespace XmlExtensions
{
    public class DefDatabaseConditional : DefDatabaseOperation
    {
        public string defType;
        public string defName;
        public string path;
        XmlContainer caseFalse;
        XmlContainer caseTrue;

        protected override bool DoPatch()
        {
            object def = GetDef(defType, defName);
            if (def == null)
            {
                Error("null def");
                return false;
            }
            object obj = FindObject(def, path);
            return RunPatchesConditional(obj != null, caseTrue, caseFalse, null);
        }
    }
}