using System.Collections.Generic;
using System.Xml;

namespace XmlExtensions.Boolean
{
    public class Xor : BooleanBase
    {
        protected BooleanBase condition1 = null;
        protected BooleanBase condition2 = null;
        public List<BooleanBase> conditions;

        protected override bool Evaluation(ref bool b, XmlDocument xml)
        {
            if (conditions == null)
            {
                bool b1 = false;
                if (!condition1.Evaluate(ref b1, xml))
                {
                    EvaluationError("condition1");
                    return false;
                }
                bool b2 = false;
                if (!condition2.Evaluate(ref b2, xml))
                {
                    EvaluationError("condition2");
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
                    if (!condition.Evaluate(ref b1, xml))
                    {
                        EvaluationError(num);
                        return false;
                    }
                    if (b1)
                        b = !b;
                }
                return true;
            }
        }
    }
}