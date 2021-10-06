using System;
using System.Collections.Generic;
using System.Xml;
using Verse;
using XmlExtensions.Boolean;

namespace XmlExtensions
{
    public class EvaluateBoolean : PatchOperationValue
    {
        public BooleanBase condition;

        protected override void SetException()
        {
            exceptionVals = new string[] { storeIn };
            exceptionFields = new string[] { "storeIn" };
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            if (condition == null)
            {
                NullError("condition");
                return false;
            }
            bool b = false;
            if (!condition.Evaluate(ref b, xml))
            {
                Error("Failed to evaluate <condition>");
                return false;
            }
            vals.Add(b.ToString());
            return true;
        }
    }

}
