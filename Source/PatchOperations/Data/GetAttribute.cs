﻿using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class GetAttribute : PatchOperationValuePathed
    {
        public string attribute;

        protected override void SetException()
        {
            exceptionVals = new string[] { storeIn, attribute, xpath };
            exceptionFields = new string[] { "storeIn", "attribute", "xpath" };
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            XmlAttribute xattribute;
            xattribute = node.Attributes[attribute];
            if (xattribute == null)
            {
                Error("Failed to find attribute");
                return false;
            }
            vals.Add(xattribute.Value);
            return true;
        }
    }

}
