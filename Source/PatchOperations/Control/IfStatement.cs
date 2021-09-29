using System.Xml;
using Verse;
using XmlExtensions.Boolean;
using System;

namespace XmlExtensions
{
    public class IfStatement : PatchOperationExtendedPathed
    {
        protected BooleanBase condition = null;
        protected XmlContainer caseTrue = null;
        protected XmlContainer caseFalse = null;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                int errNum = 0;
                bool flag = false;
                try
                {
                    bool b = false;
                    if (!condition.evaluate(ref b, xml))
                    {
                        PatchManager.errors.Add("XmlExtensions.IfStatement: Failed to evaluate <condition>");
                        return false;
                    }
                    flag = b;
                }
                catch
                {
                    PatchManager.errors.Add("XmlExtensions.IfStatement: Error in evaluating the condition");
                    return false;
                }
                if (flag)
                {
                    if (this.caseTrue != null)
                    {
                        if (!Helpers.runPatchesInXmlContainer(this.caseTrue, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.IfStatement: Error in <caseTrue> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                }
                else
                {
                    if (this.caseFalse != null)
                    {
                        if (!Helpers.runPatchesInXmlContainer(this.caseFalse, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.IfStatement: Error in <caseFalse> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.IfStatement: " + e.Message);
                return false;
            }
        }

    }
}

