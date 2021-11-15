using System.Xml;

namespace XmlExtensions
{
    internal abstract class PatchOperationValuePathed : PatchOperationValue
    {
        public string xpath;
        protected XmlNode node;

        protected override void SetException()
        {
            CreateExceptions(xpath, "xpath");
        }

        protected override bool PreCheck(XmlDocument xml)
        {
            if (xpath == null)
            {
                NullError("xpath");
                return false;
            }
            if (node == null)
            {
                // TODO: Upgrade to Helper
                node = Helpers.SelectSingleNode(xpath, xml);
                if (node == null)
                {
                    XPathError();
                    return false;
                }
            }
            return true;
        }
    }
}