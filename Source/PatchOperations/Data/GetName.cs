using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class GetName : PatchOperationValuePathed
    {
        protected override void SetException()
        {
            exceptionVals = new string[] { storeIn, xpath };
            exceptionFields = new string[] { "storeIn", "xpath" };
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            vals.Add(node.Name.ToString());
            return true;
        }
    }

}
