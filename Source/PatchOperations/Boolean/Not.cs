using System;
using System.Xml;

namespace XmlExtensions.Boolean
{
    public class Not : BooleanBase
    {
        protected BooleanBase condition = null;

        protected override bool evaluation(ref bool b, XmlDocument xml)
        {
            try
            {
                bool b1 = false;
                if (!condition.evaluate(ref b1, xml))
                {
                    PatchManager.errors.Add(this.GetType().ToString() + ": Failed to evaluate <condition>");
                    return false;
                }
                b = !b1;
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add(this.GetType().ToString() + ": " + e.Message);
                return false;
            }
        }
    }
}
