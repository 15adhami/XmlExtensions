using System.Xml;

namespace XmlExtensions
{
    public abstract class PatchOperationExtendedAttribute : PatchOperationExtended
    {
        public string attribute;
        public string xpath;

        protected override bool Patch(XmlDocument xml)
        {
            if (attribute == null)
            {
                PatchManager.errors.Add(this.GetType().ToString() + "(xpath=" + xpath + "): attribute is null");
                return false;
            }
            if (xpath == null)
            {
                PatchManager.errors.Add(this.GetType().ToString() + "(attribute=" + attribute + "): xpath is null");
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