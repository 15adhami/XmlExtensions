using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class Case
    {
        public string value;
        public XmlContainer apply;
    }

    internal class PatchByCase : PatchOperationExtended
    {
        public string value;
        public List<Case> cases;
        public string defaultCase;

        protected override void SetException()
        {
            CreateExceptions(value, "value");
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
            int defaultCaseIndex = 0;
            foreach (Case casePatch in cases)
            {
                c++;
                if (value == casePatch.value)
                {
                    return RunPatches(casePatch.apply, value, xml);
                }
                if (defaultCase == casePatch.value)
                {
                    defaultCaseIndex = c;
                }
                if (c == cases.Count)
                {
                    if (defaultCase != null)
                    {
                        return RunPatches(cases[defaultCaseIndex].apply, defaultCase, xml);
                    }
                }
            }
            return true;
        }
    }
}