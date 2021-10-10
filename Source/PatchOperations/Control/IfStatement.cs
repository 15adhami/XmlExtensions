using System.Xml;
using Verse;
using XmlExtensions.Boolean;

namespace XmlExtensions
{
    public class IfStatement : PatchOperationExtended
    {
        protected BooleanBase condition = null;
        protected XmlContainer caseTrue = null;
        protected XmlContainer caseFalse = null;

        protected override bool Patch(XmlDocument xml)
        {
            bool flag = false;
            try
            {
                if (!condition.Evaluate(ref flag, xml))
                {
                    Error("Failed to evaluate <condition>");
                    return false;
                }
            }
            catch
            {
                Error("Failed to evaluate <condition>");
                return false;
            }
            return RunPatchesConditional(flag, caseTrue, caseFalse, xml);
        }
    }
}