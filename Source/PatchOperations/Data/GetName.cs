using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class GetName : PatchOperationDataPathed
    {
        protected override void SetException()
        {
            CreateExceptions(storeIn, "storeIn", xpath, "xpath");
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            vals.Add(node.Name.ToString());
            return true;
        }
    }

}
