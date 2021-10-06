using System.Xml;
using Verse;
using System;

namespace XmlExtensions
{
    public class Conditional : PatchOperationExtended
    {
        public XmlContainer caseTrue;
        public XmlContainer caseFalse;
        public string xpath;

        protected override void SetException()
        {
            exceptionVals = new string[] { xpath };
            exceptionFields = new string[] { "xpath" };
        }

        protected override bool Patch(XmlDocument xml)
        {
            return RunPatchesConditional(xml.SelectSingleNode(xpath) != null, caseTrue, caseFalse, xml);
        }
    }
}

