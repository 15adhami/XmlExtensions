﻿using Verse;

namespace XmlExtensions
{
    internal class DefDatabaseConditional : DefDatabaseOperation
    {
        public string defType;
        public string defName;
        public string objPath;
        XmlContainer caseFalse;
        XmlContainer caseTrue;

        protected override bool DoPatch()
        {
            if (defType != null)
            {
                return RunPatchesConditional(SelectObjects(defType + "/[defName=\"" + defName + "\"]/" + objPath).Count > 0, caseTrue, caseFalse, null);
            }
            return RunPatchesConditional(SelectObjects(objPath).Count > 0, caseTrue, caseFalse, null);
        }
    }
}