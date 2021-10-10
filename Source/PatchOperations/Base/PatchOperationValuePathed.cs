using System.Xml;

namespace XmlExtensions
{
    public abstract class PatchOperationValuePathed : PatchOperationValue
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
                node = xml.SelectSingleNode(xpath);
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