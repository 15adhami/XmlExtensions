using System.Xml;

namespace XmlExtensions
{
    public abstract class PatchOperationExtendedAttribute : PatchOperationExtendedPathed
    {
        public string attribute;

        protected override bool PreCheck(XmlDocument xml)
        {
            if (!base.PreCheck(xml)) { return false; }
            if (attribute == null)
            {
                NullError("attribute");
                return false;
            }
            return true;
        }

        protected override void SetException()
        {
            exceptionVals = new string[] { attribute, xpath };
            exceptionFields = new string[] { "attribute", "xpath" };
        }
    }
}