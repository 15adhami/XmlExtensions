﻿using System.Xml;
using Verse;
using System;

namespace XmlExtensions
{
    public class TryCatch : PatchOperationExtended
    {
        public XmlContainer tryApply;
        public XmlContainer catchApply;

        protected override bool Patch(XmlDocument xml)
        {
            if (!RunPatches(tryApply, "tryApply", xml))
            {
                PatchManager.errors.Clear();
                if (catchApply != null)
                {
                    return RunPatches(catchApply, "catchApply", xml);
                }
            }
            return true;
        }
    }
}
