using System;
using System.Collections.Generic;
using System.Xml;

namespace XmlExtensions.Boolean
{
    public class Xor : BooleanBase
    {
        protected BooleanBase condition1 = null;
        protected BooleanBase condition2 = null;
        public List<BooleanBase> conditions;

        protected override bool evaluation(ref bool b, XmlDocument xml)
        {
            try
            {
                if (conditions == null)
                {
                    bool b1 = false;
                    if (!condition1.evaluate(ref b1, xml))
                    {
                        PatchManager.errors.Add(this.GetType().ToString() + ": Failed to evaluate <condition1>");
                        return false;
                    }
                    bool b2 = false;
                    if (!condition2.evaluate(ref b2, xml))
                    {
                        PatchManager.errors.Add(this.GetType().ToString() + ": Failed to evaluate <condition2>");
                        return false;
                    }
                    b = (b1 && !b2) || (!b1 && b2);
                    return true;
                }
                else
                {
                    bool b1 = false;
                    b = false;
                    int num = 0;
                    foreach (BooleanBase condition in conditions)
                    {
                        num++;
                        if (!condition.evaluate(ref b1, xml))
                        {
                            PatchManager.errors.Add(this.GetType().ToString() + ": Failed to evaluate the condition at position=" + num.ToString());
                            return false;
                        }
                        if (b1)
                            b = !b;
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                PatchManager.errors.Add(this.GetType().ToString() + ": " + e.Message);
                return false;
            }
        }
    }
}
