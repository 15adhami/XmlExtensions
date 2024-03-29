﻿using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class Conditional : PatchOperationExtended
    {
        public XmlContainer caseTrue;
        public XmlContainer caseFalse;
        public string xpath;

        protected override void SetException()
        {
            CreateExceptions(xpath, "xpath");
        }

        protected override bool Patch(XmlDocument xml)
        {
            return RunPatchesConditional(xml.SelectSingleNode(xpath) != null, caseTrue, caseFalse, xml);
        }
    }
}