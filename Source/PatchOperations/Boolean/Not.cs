using System.Xml;

namespace XmlExtensions.Boolean
{
    internal class Not : BooleanBase
    {
        protected BooleanBase condition = null;

        protected override bool Evaluation(ref bool b, XmlDocument xml)
        {
            bool b1 = false;
            if (!condition.Evaluate(ref b1, xml))
            {
                EvaluationError("condition");
                return false;
            }
            b = !b1;
            return true;
        }
    }
}