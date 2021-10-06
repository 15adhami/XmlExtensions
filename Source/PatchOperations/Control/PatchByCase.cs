using System.Xml;
using System.Collections.Generic;
using System;
using Verse;

namespace XmlExtensions
{
    public class Case
    {
        public string value;
        public XmlContainer apply;
    }

    public class PatchByCase : PatchOperationExtended
    {
        public string value;
        public List<Case> cases;

        protected override void SetException()
        {
            exceptionVals = new string[] { value };
            exceptionFields = new string[] { "value" };
        }

        protected override bool Patch(XmlDocument xml)
        {
            if (value == null)
            {
                NullError("value");
                return false;
            }
            if (cases == null)
            {
                NullError("cases");
                return false;
            }
            int c = 0;
            foreach (Case casePatch in cases)
            {
                c++;
                if (value == casePatch.value)
                {
                    return RunPatches(casePatch.apply, value, xml);
                }

                // run first case as default case
                if (c == cases.Count)
                {
                    return RunPatches(cases[0].apply, value, xml);
                }
            }
            return true;
        }

    }
}

