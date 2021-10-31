using System.Collections.Generic;
using System.Xml;

namespace XmlExtensions
{
    internal class GetAttribute : PatchOperationValuePathed
    {
        public string attribute;

        protected override void SetException()
        {
            CreateExceptions(storeIn, "storeIn", attribute, "attribute", xpath, "xpath");
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