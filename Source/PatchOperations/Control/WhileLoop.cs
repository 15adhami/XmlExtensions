﻿using System.Xml;
using Verse;
using XmlExtensions.Boolean;

namespace XmlExtensions
{
    internal class WhileLoop : PatchOperationExtended
    {
        public BooleanBase condition;
        public XmlContainer apply;

        private int err = 1;

        protected override void SetException()
        {
            CreateExceptions(err.ToString(), "iteration");
        }

        protected override bool Patch(XmlDocument xml)
        {
            if (condition == null)
            {
                NullError("condition");
                return false;
            }
            if (apply == null)
            {
                NullError("apply");
                return false;
            }
            bool b = false;
            if (!condition.Evaluate(ref b, xml))
            {
                Error("Failed to evaluate the condition");
                return false;
            }
            while (b)
            {
                if (!RunPatches(apply, xml))
                {
                    return false;
                }
                err++;
                if (err >= 10000)
                {
                    Error("Loop limit reached");
                    return false;
                }
                if (!condition.Evaluate(ref b, xml))
                {
                    Error("Failed to evaluate the condition");
                    return false;
                }
            }
            return true;
        }
    }
}