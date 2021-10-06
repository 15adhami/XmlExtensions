using System;
using System.Xml;

namespace XmlExtensions.Boolean
{
    public class Not : BooleanBase
    {
        protected BooleanBase condition = null;

        protected override bool Evaluation(ref bool b, XmlDocument xml)
        {
            try
            {
                bool b1 = false;
                if (!condition.Evaluate(ref b1, xml))
                {
                    PatchManager.errors.Add(GetType().ToString() + ": Failed to evaluate <condition>");
                    return false;
                }
                b = !b1;
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add(GetType().ToString() + ": " + e.Message);
                return false;
            }
        }
    }
}
